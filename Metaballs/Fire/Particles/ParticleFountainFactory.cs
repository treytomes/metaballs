using Metaballs.Fire.Brushes;
using OpenTK.Mathematics;

namespace Metaballs.Fire.Particles;

// Note: Now that we have multiple types of fountains, a factory can make things a bit cleaner.
class ParticleFountainFactory
{
	public ParticleFountain CreateDrawingFountain(Vector2? position = null)
	{
		return new(position ?? Vector2.Zero, new()
		{
			SpawnRate = TimeSpan.FromMilliseconds(100),
			Velocity = new Vector2(0, 2f),
			Acceleration = new Vector2(0, 8f),
			LifeSpan = TimeSpan.FromSeconds(5),
			Scale = 2f,
			Brush = new BlobbyCircleMetaballsBrush()
			{
				Flicker = 0.7f,
				Blend = 0.3f,
			},
			Noise = 0.5f,
		});
	}

	// Note: The tile consumption fountain is smaller and faster than the pen.
	public ParticleFountain CreateTileBurningFountain(Vector2? position = null)
	{
		return new(position ?? Vector2.Zero, new()
		{
			SpawnRate = TimeSpan.FromMilliseconds(10),
			Velocity = new Vector2(0, 2f),
			Acceleration = new Vector2(0, 8f),
			LifeSpan = TimeSpan.FromSeconds(7),
			Scale = 1f,
			Brush = new BlobbyCircleMetaballsBrush()
			{
				Flicker = 0.7f,
				Blend = 0.3f,
			},
			Noise = 0.5f,
		});
	}
}
