using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs;

class BlobCollection<TBlob>
	where TBlob : Blob
{
	#region Fields

	protected readonly List<TBlob> _blobs;
	private List<List<float?>> _samples = new();

	#endregion

	#region Constructors

	public BlobCollection(IEnumerable<TBlob>? blobs = null)
	{
		_blobs = new(blobs ?? Enumerable.Empty<TBlob>());
	}

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

		// Render the line cases.
		for (var sy = 0; sy < _samples.Count - 2; sy++)
		{
			for (var sx = 0; sx < _samples[sy].Count - 2; sx++)
			{
				// if PRINT_SAMPLES then
				// 	x = sx * GRID_RESOLUTION
				// 	y = sy * GRID_RESOLUTION
				// 	sample = calculateSample(x, y)
				// 	sample = floor(sample * 100) / 100
				// 	gfx.print sample, x -16, y - 16, color.silver, "small"
				// end if

				var bl = CalculateSample(sx, sy) ?? 0;
				var br = CalculateSample(sx + 1, sy) ?? 0;
				var tl = CalculateSample(sx, sy + 1) ?? 0;
				var tr = CalculateSample(sx + 1, sy + 1) ?? 0;

				Vector2 a, b, c, d;

				if (MetaballsConfig.Interpolated)
				{
					a = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution * Lerp(1, tl, tr),
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution
					);
					b = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution * Lerp(1, br, tr)
					);
					c = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution * Lerp(1, bl, br),
						sy * MetaballsConfig.GridResolution
					);
					d = new Vector2(
						sx * MetaballsConfig.GridResolution,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution * Lerp(1, bl, tl)
					);
				}
				else
				{
					a = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution / 2,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution
					);
					b = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution / 2
					);
					c = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution / 2,
						sy * MetaballsConfig.GridResolution
					);
					d = new Vector2(
						sx * MetaballsConfig.GridResolution,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution / 2
					);
				}

				bl = bl >= 1 ? 1 : 0;
				br = br >= 1 ? 1 : 0;
				tl = tl >= 1 ? 1 : 0;
				tr = tr >= 1 ? 1 : 0;
				var blobCase = bl + br * 2 + tr * 4 + tl * 8;

				if (blobCase == 0 || blobCase == 15)
					continue;
				else if (blobCase == 1 || blobCase == 14)
					rc.RenderLine(d, c, RadialColor.Green);
				else if (blobCase == 2 || blobCase == 13)
					rc.RenderLine(b, c, RadialColor.Green);
				else if (blobCase == 3 || blobCase == 12)
					rc.RenderLine(d, b, RadialColor.Green);
				else if (blobCase == 4 || blobCase == 11)
					rc.RenderLine(a, b, RadialColor.Green);
				else if (blobCase == 5)
				{
					rc.RenderLine(d, a, RadialColor.Green);
					rc.RenderLine(c, b, RadialColor.Green);
				}
				else if (blobCase == 6 || blobCase == 9)
					rc.RenderLine(c, a, RadialColor.Green);
				else if (blobCase == 7 || blobCase == 8)
					rc.RenderLine(d, a, RadialColor.Green);
				else if (blobCase == 10)
				{
					rc.RenderLine(a, b, RadialColor.Green);
					rc.RenderLine(c, d, RadialColor.Green);
				}
			}
		}

		foreach (var blob in _blobs)
		{
			blob.Render(rc);
		}
	}

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public void Update(GameTime gameTime)
	{
		foreach (var blob in _blobs)
		{
			blob.Update(gameTime);
		}
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

		for (var y = 0; y < rc.Height; y += MetaballsConfig.GridResolution)
		{
			var row = new List<float?>();
			for (var x = 0; x < rc.Width; x += MetaballsConfig.GridResolution)
			{
				row.Add(null);
			}
			_samples.Add(row);
		}
	}

	private float? CalculateSample(int sx, int sy)
	{
		if (_samples[sy][sx] != null) return _samples[sy][sx];
		var x = sx * MetaballsConfig.GridResolution;
		var y = sy * MetaballsConfig.GridResolution;

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