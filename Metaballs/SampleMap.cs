using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace Metaballs;

class SampleMap
{
	#region Constants

	/// <summary>
	/// Bayer 4x4 dithering matrix.
	/// </summary>
	private static readonly int[,] BAYER_MATRIX = new int[,] {
		{  0, 12,  3, 15 },
		{  8,  4, 11,  7 },
		{  2, 14,  1, 13 },
		{ 10,  6,  9,  5 }
	};

	#endregion

	#region Fields

	private readonly MetaballsSettings _settings;
	private float[,] _data;

	#endregion

	#region Constructors

	public SampleMap(MetaballsSettings settings, int width, int height)
	{
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		Width = width;
		Height = height;
		_data = new float[height, width];
		Array.Clear(_data);
	}

	#endregion

	#region Properties

	public RadialColor OutlineColor { get; set; } = RadialColor.Yellow;
	public RadialColor PrimaryFillColor { get; set; } = RadialColor.Gray.Lerp(RadialColor.Black, 0.5f);
	public RadialColor SecondaryFillColor { get; set; } = RadialColor.Gray.Lerp(RadialColor.Black, 0.75f);

	public bool IsOutlined { get; set; } = true;
	public bool IsFilled { get; set; } = true;

	public int Width { get; }
	public int Height { get; }

	public float this[int y, int x]
	{
		get
		{
			if (y < 0 || y >= Height || x < 0 || x >= Width) return 0;
			return _data[y, x];
		}
		set
		{
			if (y < 0 || y >= Height || x < 0 || x >= Width) return;
			_data[y, x] = value;
		}
	}

	private bool Interpolated => _settings.Interpolated;
	private float GridResolution => _settings.GridResolution;

	#endregion

	#region Methods

	public void Clear()
	{
		Array.Clear(_data);
	}

	public void Render(IRenderingContext rc)
	{
		RenderBayerField(rc, PrimaryFillColor, SecondaryFillColor);
		RenderOutline(rc, OutlineColor);
	}

	private void RenderBayerField(IRenderingContext rc, RadialColor primaryColor, RadialColor secondaryColor)
	{
		// Note: I figured out the inverse Bayer matrix trick while trying to figure out how to render a heat field.
		Parallel.For(0, Height, y =>
			Parallel.For(0, Width, x =>
			{
				var pos = new Vector2(x, y);

				// Calculate dithering base value.
				var intensity = _data[y, x];

				// Don't fill areas outside the blob range.
				if (intensity < 1.0f) return;

				// This will discount sample values that fall outside our range.
				intensity -= 1f;

				intensity = RetroTK.MathHelper.Clamp(intensity, 0.0f, 1.0f);

				// Note: Applying a sin function to the intensity seems to give a nicer drop-off appearance.
				intensity = (float)Math.Sin(intensity);

				// Get the appropriate threshold from the Bayer matrix (0-15, normalized to 0.0-1.0)
				var bayerX = RetroTK.MathHelper.Modulus(Math.Abs((int)pos.X), 4);
				var bayerY = RetroTK.MathHelper.Modulus(Math.Abs((int)pos.Y), 4);
				var threshold = BAYER_MATRIX[bayerY, bayerX] / 16.0f;
				var color = secondaryColor.Lerp(primaryColor, intensity);
				if (intensity >= threshold)
				{
					rc.SetPixel(pos, color);
				}
				else
				{
					// The fill color for anything below the dithering threshold.
					rc.SetPixel(pos, secondaryColor.Lerp(primaryColor, intensity * 0.5f));
				}
			})
		);
	}

	private void RenderOutline(IRenderingContext rc, RadialColor outlineColor)
	{
		// Render the outline.
		Parallel.For(0, Height - 2, sy =>
			Parallel.For(0, Width - 2, sx =>
			{
				RenderOutlineSegments(rc, sx, sy, outlineColor);
			})
		);
	}

	private void RenderOutlineSegments(IRenderingContext rc, int sx, int sy, RadialColor outlineColor)
	{
		var bl = _data[sy, sx];
		var br = _data[sy, sx + 1];
		var tl = _data[sy + 1, sx];
		var tr = _data[sy + 1, sx + 1];

		Vector2 a, b, c, d;

		// Note: If the grid resolution is 1, there's really no gain in interpolation.
		if (Interpolated && GridResolution != 1)
		{
			a = new Vector2(
				sx * GridResolution + GridResolution * Lerp(1, tl, tr),
				sy * GridResolution + GridResolution
			);
			b = new Vector2(
				sx * GridResolution + GridResolution,
				sy * GridResolution + GridResolution * Lerp(1, br, tr)
			);
			c = new Vector2(
				sx * GridResolution + GridResolution * Lerp(1, bl, br),
				sy * GridResolution
			);
			d = new Vector2(
				sx * GridResolution,
				sy * GridResolution + GridResolution * Lerp(1, bl, tl)
			);
		}
		else
		{
			a = new Vector2(
				sx * GridResolution + GridResolution / 2,
				sy * GridResolution + GridResolution
			);
			b = new Vector2(
				sx * GridResolution + GridResolution,
				sy * GridResolution + GridResolution / 2
			);
			c = new Vector2(
				sx * GridResolution + GridResolution / 2,
				sy * GridResolution
			);
			d = new Vector2(
				sx * GridResolution,
				sy * GridResolution + GridResolution / 2
			);
		}

		bl = bl >= 1 ? 1 : 0;
		br = br >= 1 ? 1 : 0;
		tl = tl >= 1 ? 1 : 0;
		tr = tr >= 1 ? 1 : 0;
		var blobCase = bl + br * 2 + tr * 4 + tl * 8;

		if (blobCase == 0 || blobCase == 15)
		{
			// skip
		}
		else if (blobCase == 1 || blobCase == 14)
		{
			RenderSegment(rc, d, c, outlineColor);
		}
		else if (blobCase == 2 || blobCase == 13)
		{
			RenderSegment(rc, b, c, outlineColor);
		}
		else if (blobCase == 3 || blobCase == 12)
		{
			RenderSegment(rc, d, b, outlineColor);
		}
		else if (blobCase == 4 || blobCase == 11)
		{
			RenderSegment(rc, a, b, outlineColor);
		}
		else if (blobCase == 5)
		{
			RenderSegment(rc, d, a, outlineColor);
			RenderSegment(rc, c, b, outlineColor);
		}
		else if (blobCase == 6 || blobCase == 9)
		{
			RenderSegment(rc, c, a, outlineColor);
		}
		else if (blobCase == 7 || blobCase == 8)
		{
			RenderSegment(rc, d, a, outlineColor);
		}
		else if (blobCase == 10)
		{
			RenderSegment(rc, a, b, outlineColor);
			RenderSegment(rc, c, d, outlineColor);
		}
	}

	private static void RenderSegment(IRenderingContext rc, Vector2 from, Vector2 to, RadialColor color)
	{
		if (from == to)
		{
			rc.SetPixel(from, color);
		}
		else
		{
			rc.RenderLine(from, to, color);
		}
	}

	private static float Lerp(float x, float x0, float x1, float y0 = 0, float y1 = 1)
	{
		if (x0 == x1)
		{
			return 0.0f;
		}
		return y0 + ((y1 - y0) * (x - x0)) / (x1 - x0);
	}

	#endregion
}