using System.Collections;
using Metaballs.Bounds;
using Metaballs.Renderables;
using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs;

class BlobCollection<TBlob> : IEnumerable<TBlob>
	where TBlob : Blob
{
	#region Constants

	private const bool SHOW_CENTER_OF_MASS = false;

	#endregion

	#region Fields

	private readonly MetaballsSettings _settings;
	protected readonly List<TBlob> _blobs;
	private readonly SampleMap _samples;
	private Plus _plus = new()
	{
		Size = 6,
		Color = RadialColor.Red,
	};

	#endregion

	#region Constructors

	public BlobCollection(MetaballsSettings settings, int width, int height, IEnumerable<TBlob>? blobs = null)
	{
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		_samples = new SampleMap(settings, width, height);
		_blobs = [.. blobs ?? Enumerable.Empty<TBlob>()];

		OutlineColor = new RadialColor(5, 0, 5);
		PrimaryColor = new RadialColor(0, 5, 0);
		SecondaryColor = new RadialColor(0, 2, 0);

		_plus.IsVisible = SHOW_CENTER_OF_MASS;
	}

	#endregion

	#region Properties

	public Vector2 CenterOfMass
	{
		get
		{
			return _plus.Position;
		}
		set
		{
			_plus.Position = value;
		}
	}

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

	public RadialColor PrimaryColor
	{
		get
		{
			return _samples.PrimaryFillColor;
		}
		set
		{
			_samples.PrimaryFillColor = value;
		}
	}

	public RadialColor SecondaryColor
	{
		get
		{
			return _samples.SecondaryFillColor;
		}
		set
		{
			_samples.SecondaryFillColor = value;
		}
	}

	public bool IsOutlined
	{
		get
		{
			return _samples.IsOutlined;
		}
		set
		{
			_samples.IsOutlined = value;
		}
	}

	public bool IsFilled
	{
		get
		{
			return _samples.IsFilled;
		}
		set
		{
			_samples.IsFilled = value;
		}
	}

	public bool ShowCenterOfMass { get; set; } = true;

	#endregion

	#region Methods

	public IEnumerator<TBlob> GetEnumerator()
	{
		return ((IEnumerable<TBlob>)_blobs).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_blobs).GetEnumerator();
	}

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

		// Render the samples.
		_samples.Render(rc);

		// Render the blob circles.
		RenderBlobs(rc);

		if (ShowCenterOfMass) _plus.Render(rc);
	}

	private void RenderBlobs(IRenderingContext rc)
	{
		Parallel.ForEach(_blobs, b => b.Render(rc));
	}

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public virtual void Update(GameTime gameTime, IBoundingArea? bounds = null)
	{
		Parallel.ForEach(_blobs, blob => blob.Update(gameTime, bounds));
		CenterOfMass = CalculateCenterOfMass();
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

	protected Vector2 CalculateCenterOfMass()
	{
		var centerOfMass = Vector2.Zero;
		if (_blobs.Count == 0) return Vector2.Zero;
		var totalRadius = _blobs.Sum(x => x.Radius);
		foreach (var blob in _blobs)
		{
			centerOfMass += blob.Radius * blob.Position;
		}
		centerOfMass /= totalRadius;
		return centerOfMass;
	}

	#endregion
}