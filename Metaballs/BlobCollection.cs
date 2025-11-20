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
	private readonly int _width;
	private readonly int _height;
	protected readonly List<TBlob> _blobs;
	private readonly SampleMap _samples;

	#endregion

	#region Constructors

	public BlobCollection(MetaballsSettings settings, int width, int height, IEnumerable<TBlob>? blobs = null)
	{
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		_samples = new SampleMap(settings, width, height);
		_width = width;
		_height = height;
		_blobs = [.. blobs ?? Enumerable.Empty<TBlob>()];

		OutlineColor = new RadialColor(5, 0, 5);
	}

	#endregion

	#region Properties

	public RadialColor OutlineColor
	{
		get
		{
			return _samples.OutlineColor;
		}
		set
		{
			_samples.OutlineColor = value;
		}
	}

	public RadialColor PrimaryColor { get; set; } = new RadialColor(0, 5, 0);
	public RadialColor SecondaryColor { get; set; } = new RadialColor(0, 2, 0);

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
		_samples.Clear();

		// Note: We're using Parallel to increase performance at the cost of the CPU.  Try not to start a fire!

		// Pre-calculate the samples.
		CalculateSamples();

		RenderBayerField(rc);

		// Render the outline.
		_samples.Render(rc);

		// Render the blob circles.
		RenderBlobs(rc);
	}

	private void RenderBayerField(IRenderingContext rc)
	{
		// Note: I figured out the inverse Bayer matrix trick while trying to figure out how to render a heat field.
		Parallel.For(0, _height, y =>
			Parallel.For(0, _width, x =>
			{
				var pos = new Vector2(x, y);

				// Calculate dithering base value.
				var intensity = _samples[y, x];

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

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public void Update(GameTime gameTime)
	{
		Parallel.ForEach(_blobs, blob => blob.Update(gameTime));
	}

	private void CalculateSamples()
	{
		Parallel.For(0, _samples.Height - 2, sy =>
			Parallel.For(0, _samples.Width - 2, sx =>
				CalculateSample(sx, sy)
			)
		);
	}

	private float? CalculateSample(int sx, int sy)
	{
		var x = sx * _settings.GridResolution;
		var y = sy * _settings.GridResolution;

		var sample = 0f;
		foreach (var b in _blobs)
		{
			sample += (float)Math.Pow(b.Radius, 2) / ((float)Math.Pow(x - b.Position.X, 2) + (float)Math.Pow(y - b.Position.Y, 2));
		}
		_samples[sy, sx] = sample;
		return sample;
	}

	#endregion
}