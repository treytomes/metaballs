using OpenTK.Mathematics;

namespace Metaballs.Bounds;

class CircleBoundingArea(Vector2 position, float radius) : IBoundingArea
{
	public Vector2 Position { get; private set; } = position;
	public float Radius { get; } = radius;

	public bool Contains(Vector2 pnt)
	{
		return (Position - pnt).Length <= Radius;
	}

	public void MoveTo(Vector2 pnt)
	{
		Position = pnt;
	}

	/// <summary>
	/// Calculate the point on the edge of the bounding area in the direction of <paramref name="pnt"/>.
	/// </summary>
	public Vector2 Clamp(Vector2 pnt)
	{
		return Position + (pnt - Position).Normalized() * Radius;
	}

	public Vector2 ReflectVelocity(Vector2 pos, Vector2 vel)
	{
		// Direction from center to blob position
		var normal = (pos - Position).Normalized();

		// Standard reflection formula
		var reflected = vel - 2 * Vector2.Dot(vel, normal) * normal;

		return reflected;
	}
}
