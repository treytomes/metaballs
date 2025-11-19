using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs;

class Blob
{
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

	public virtual bool IsActive { get; } = true;

	public bool DrawCircles { get; init; }
	public Vector2 Position { get; private set; }
	public int Radius { get; private set; }
	public virtual RadialColor Color { get; } = RadialColor.Red;
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
		if (DrawCircles)
		{
			rc.RenderCircle(Position, Radius, Color);
		}
	}

	public void Update(GameTime gameTime)
	{
		if (IsActive)
		{
			Position += Velocity * (float)gameTime.ElapsedTime.TotalSeconds;
		}
	}

	public bool Contains(Vector2 pnt)
	{
		var distance = pnt - Position;
		return distance.LengthSquared <= Radius * Radius;
	}

	public void SetRadius(int value)
	{
		Console.WriteLine("value=" + value);
		if (value < 1) value = 1;
		Radius = value;
	}

	public void MoveBy(Vector2 delta)
	{
		Position += delta;
	}

	public void MoveTo(Vector2 position)
	{
		Position = position;
	}

	#endregion
}
