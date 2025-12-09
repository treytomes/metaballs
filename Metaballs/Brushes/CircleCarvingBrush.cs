using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs.Brushes;

class CircleCarvingBrush : ICarvingBrush
{
	#region Constants

	private const float DEFAULT_INTENSITY = 2f;
	private const int DEFAULT_RADIUS = 8;
	private const int MIN_RADIUS = 1;

	#endregion

	#region Fields

	private float _intensity;
	private int _radius;
	private int _radiusSquared;

	#endregion

	#region Constructors

	public CircleCarvingBrush(float intensity = DEFAULT_INTENSITY, int radius = DEFAULT_RADIUS)
	{
		_intensity = intensity;
		_radius = radius;
		_radiusSquared = _radius * _radius;
	}

	#endregion

	#region Properties

	public float Intensity
	{
		get
		{
			return _intensity;
		}
		set
		{
			_intensity = value;
		}
	}

	public int Radius
	{
		get
		{
			return _radius;
		}
		set
		{
			if (value <= 0) value = MIN_RADIUS;
			_radius = value;
			_radiusSquared = _radius * _radius;
		}
	}

	#endregion

	#region Methods

	public void Render(IRenderingContext rc, Vector2 position)
	{
		rc.RenderCircle(position, Radius, RadialColor.Red);
	}

	public void Carve(GameTime gameTime, SampleMap samples, Vector2 position)
	{
		var baseFactor = (float)gameTime.ElapsedTime.TotalSeconds * Intensity;

		Parallel.For(-Radius, Radius + 1, dy =>
		{
			Parallel.For(-Radius, Radius + 1, dx =>
			{
				var distSquared = dy * dy + dx * dx;
				if (distSquared > _radiusSquared) return;

				var sampleFactor = baseFactor * (Radius - (float)Math.Sqrt(distSquared));
				var x = dx + (int)position.X;
				var y = dy + (int)position.Y;
				var newSample = samples[y, x] + sampleFactor;
				if (newSample < 0) newSample = 0;
				samples[y, x] = newSample;
			});
		});
	}

	#endregion
}