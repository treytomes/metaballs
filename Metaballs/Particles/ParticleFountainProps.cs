using Metaballs.Brushes;
using OpenTK.Mathematics;

namespace Metaballs.Particles;

// Note: We could serialize this and use it to spawn different fountain types.
class ParticleFountainProps
{
	public TimeSpan SpawnRate { get; init; }
	public Vector2 Velocity { get; init; }
	public Vector2 Acceleration { get; init; }
	public TimeSpan LifeSpan { get; init; }
	public float Scale { get; set; }
	public required IFireBrush Brush { get; init; }
	public float Noise { get; init; }
}
