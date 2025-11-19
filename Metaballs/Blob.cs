using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs;

class Blob
{
	#region Fields

	#endregion

	#region Constructors

	public Blob(Vector2 position, int radius)
	{
		Position = position;
		Radius = radius;

		Velocity = new Vector2(
			(Random.Shared.NextSingle() * 3 - 2) * Random.Shared.NextSingle() * 8 + 8,
			(Random.Shared.NextSingle() * 3 - 2) * Random.Shared.NextSingle() * 8 + 8
		);
	}

	#endregion

	#region Properties

	public Vector2 Position { get; private set; }
	public int Radius { get; private set; }
	public RadialColor Color { get; private set; } = RadialColor.Red;
	public Vector2 Velocity { get; private set; }

	public float Left => Position.X - Radius;
	public float Right => Position.X + Radius;
	public float Top => Position.Y - Radius;
	public float Bottom => Position.Y + Radius;
	public float Width => Radius * 2;
	public float Height => Radius * 2;

	#endregion

	#region Methods

	public void Render(IRenderingContext rc)
	{
		if (Left < 0 || Right >= rc.Width)
		{
			Velocity = new Vector2(-Velocity.X, Velocity.Y);
		}
		if (Top < 0 || Bottom >= rc.Height)
		{
			Velocity = new Vector2(Velocity.X, -Velocity.Y);
		}
		if (MetaballsConfig.DrawCircles)
		{
			rc.RenderCircle(Position, Radius, Color);
		}
	}

	public void Update(GameTime gameTime)
	{
		Position += Velocity * (float)gameTime.ElapsedTime.TotalSeconds;
	}

	#endregion
}
