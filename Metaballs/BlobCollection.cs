using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs;

class BlobCollection<TBlob>
	where TBlob : Blob
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
	protected readonly List<TBlob> _blobs;
	private List<List<float?>> _samples = new();

	#endregion

	#region Constructors

	public BlobCollection(MetaballsSettings settings, IEnumerable<TBlob>? blobs = null)
	{
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		_blobs = [.. blobs ?? Enumerable.Empty<TBlob>()];
	}

	#endregion

	#region Properties

	public RadialColor OutlineColor { get; set; } = new RadialColor(5, 1, 5);
	public RadialColor PrimaryColor { get; set; } = new RadialColor(0, 5, 0);
	public RadialColor SecondaryColor { get; set; } = new RadialColor(0, 2, 0);

	private bool Interpolated => _settings.Interpolated;
	private int GridResolution => _settings.GridResolution;

	#endregion

	#region Methods

	public void Add(TBlob blob)
	{
		if (_blobs.Contains(blob))
		{
			return;
		}
		_blobs.Add(blob);
	}

	public void Remove(TBlob blob)
	{
		if (!_blobs.Contains(blob))
		{
			return;
		}
		_blobs.Remove(blob);
	}

	public void Clear()
	{
		_blobs.Clear();
	}

	public void Render(IRenderingContext rc)
	{
		ResetSamples(rc);

		// Note: We're using Parallel to increase performance at the cost of the CPU.  Try not to start a fire!

		// Pre-calculate the samples.
		CalculateSamples();

		RenderBayerField(rc);

		// Render the outline.
		RenderOutline(rc);

		// Render the blob circles.
		RenderBlobs(rc);
	}

	private void RenderBayerField(IRenderingContext rc)
	{
		// Note: I figured out the inverse Bayer matrix trick while trying to figure out how to render a heat field.
		Parallel.For(0, rc.Height, y =>
			Parallel.For(0, rc.Width, x =>
			{
				var pos = new Vector2(x, y);

				// Calculate dithering base value.
				var intensity = _samples[y][x] ?? 0f;

				// Don't fill areas outside the blob range.
				if (intensity < 1.0f) return;

				// This will discount sample values that fall outside our range.
				intensity -= 1f;

				intensity = RetroTK.MathHelper.Clamp(intensity, 0.0f, 1.0f);

				// Note: Applying a sin function to the intensity seems to give a nicer drop-off appearance.
				// intensity = (float)Math.Sin(intensity);

				// Get the appropriate threshold from the Bayer matrix (0-15, normalized to 0.0-1.0)
				var bayerX = RetroTK.MathHelper.Modulus(Math.Abs((int)pos.X), 4);
				var bayerY = RetroTK.MathHelper.Modulus(Math.Abs((int)pos.Y), 4);
				var threshold = BAYER_MATRIX[bayerY, bayerX] / 16.0f;
				var color = SecondaryColor.Lerp(PrimaryColor, intensity);
				if (intensity >= threshold)
				{
					rc.SetPixel(pos, color);
				}
				else
				{
					// The fill color for anything below the dithering threshold.
					rc.SetPixel(pos, SecondaryColor.Lerp(PrimaryColor, intensity * 0.5f));
				}
			})
		);
	}

	private void RenderBlobs(IRenderingContext rc)
	{
		Parallel.ForEach(_blobs, b => b.Render(rc));
	}

	private void RenderOutline(IRenderingContext rc)
	{
		Parallel.For(0, _samples.Count - 2, sy =>
			Parallel.For(0, _samples[sy].Count - 2, sx =>
			{
				RenderSegments(rc, sx, sy);
			})
		);
	}

	private void RenderSegments(IRenderingContext rc, int sx, int sy)
	{
		var bl = CalculateSample(sx, sy) ?? 0;
		var br = CalculateSample(sx + 1, sy) ?? 0;
		var tl = CalculateSample(sx, sy + 1) ?? 0;
		var tr = CalculateSample(sx + 1, sy + 1) ?? 0;

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
			RenderSegment(rc, d, c, OutlineColor);
		}
		else if (blobCase == 2 || blobCase == 13)
		{
			RenderSegment(rc, b, c, OutlineColor);
		}
		else if (blobCase == 3 || blobCase == 12)
		{
			RenderSegment(rc, d, b, OutlineColor);
		}
		else if (blobCase == 4 || blobCase == 11)
		{
			RenderSegment(rc, a, b, OutlineColor);
		}
		else if (blobCase == 5)
		{
			RenderSegment(rc, d, a, OutlineColor);
			RenderSegment(rc, c, b, OutlineColor);
		}
		else if (blobCase == 6 || blobCase == 9)
		{
			RenderSegment(rc, c, a, OutlineColor);
		}
		else if (blobCase == 7 || blobCase == 8)
		{
			RenderSegment(rc, d, a, OutlineColor);
		}
		else if (blobCase == 10)
		{
			RenderSegment(rc, a, b, OutlineColor);
			RenderSegment(rc, c, d, OutlineColor);
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

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public void Update(GameTime gameTime)
	{
		Parallel.ForEach(_blobs, blob => blob.Update(gameTime));
	}

	private float Lerp(float x, float x0, float x1, float y0 = 0, float y1 = 1)
	{
		if (x0 == x1)
		{
			return 0.0f;
		}
		return y0 + ((y1 - y0) * (x - x0)) / (x1 - x0);
	}

	private void ResetSamples(IRenderingContext rc)
	{
		_samples = new List<List<float?>>();

		for (var y = 0; y < rc.Height; y += GridResolution)
		{
			var row = new List<float?>();
			for (var x = 0; x < rc.Width; x += GridResolution)
			{
				row.Add(null);
			}
			_samples.Add(row);
		}
	}

	private void CalculateSamples()
	{
		Parallel.For(0, _samples.Count - 2, sy =>
			Parallel.For(0, _samples[sy].Count - 2, sx =>
			{
				CalculateSample(sx, sy);
			})
		);
	}

	private float? CalculateSample(int sx, int sy)
	{
		if (_samples[sy][sx] != null) return _samples[sy][sx];
		var x = sx * GridResolution;
		var y = sy * GridResolution;

		var sample = 0f;
		foreach (var b in _blobs)
		{
			sample += (float)Math.Pow(b.Radius, 2) / ((float)Math.Pow(x - b.Position.X, 2) + (float)Math.Pow(y - b.Position.Y, 2));
		}
		_samples[sy][sx] = sample;
		return sample;
	}

	#endregion
}