using Metaballs;
using Metaballs.Brushes;
using OpenTK.Mathematics;
using RetroTK;

namespace Metaballs.Particles;


// Note: Class, not struct.  Otherwise the references won't update like they should.
class Particle
{
	#region Constructors

	public Particle(Vector2 position, Vector2 velocity, Vector2 acceleration, TimeSpan lifeSpan, float scale, IFireBrush brush)
	{
		Position = position;
		Velocity = velocity;
		Acceleration = acceleration;
		LifeSpan = lifeSpan;
		Scale = scale;
		Brush = brush;
	}

	#endregion

	#region Properties

	public Vector2 Position { get; private set; }
	public Vector2 Velocity { get; private set; }
	public Vector2 Acceleration { get; private set; }
	public TimeSpan LifeSpan { get; private set; }
	public float Scale { get; private set; }
	public IFireBrush Brush { get; private set; }

	public bool IsAlive => LifeSpan.TotalSeconds > 0;

	#endregion

	#region Methods

	public void Update(GameTime gameTime)
	{
		Velocity += Acceleration * (float)gameTime.ElapsedTime.TotalSeconds;
		Position += Velocity * (float)gameTime.ElapsedTime.TotalSeconds;
		LifeSpan -= gameTime.ElapsedTime;
	}

	public void Render(FireBuffer buffer)
	{
		Brush.Draw(buffer, (int)Position.X, (int)Position.Y, (int)(LifeSpan.TotalSeconds * Scale));
	}

	#endregion
}
