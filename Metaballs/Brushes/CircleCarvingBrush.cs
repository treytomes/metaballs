using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace Metaballs.Brushes;

class CircleCarvingBrush : ICarvingBrush
{
	#region Constants

	private const float DEFAULT_INTENSITY = 4f;
	private const int DEFAULT_RADIUS = 8;
	private const int MIN_RADIUS = 1;

	#endregion

	#region Fields

	private float _intensity;
	private int _radius;

	#endregion

	#region Constructors

	public CircleCarvingBrush(float intensity = DEFAULT_INTENSITY, int radius = DEFAULT_RADIUS)
	{
		_intensity = intensity;
		_radius = radius;
	}

	#endregion

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
		}
	}

	public void Render(IRenderingContext rc, Vector2 position)
	{
		rc.RenderCircle(position, Radius, RadialColor.Red);
	}

	public void Carve(SampleMap samples, Vector2 position)
	{
		for (var dy = -Radius; dy <= Radius; dy++)
		{
			for (var dx = -Radius; dx <= Radius; dx++)
			{
				var dist = (float)Math.Sqrt(dy * dy + dx * dx);
				if (dist > Radius) continue;

				var sampleFactor = Intensity * (Radius - dist);
				var x = dx + (int)position.X;
				var y = dy + (int)position.Y;
				var newSample = samples[y, x] + sampleFactor;
				if (newSample < 0) newSample = 0;
				samples[y, x] = newSample;
			}
		}
	}
}