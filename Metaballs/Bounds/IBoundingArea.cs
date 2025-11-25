using OpenTK.Mathematics;

namespace Metaballs.Bounds;

interface IBoundingArea
{
	public Vector2 Position { get; }
	bool Contains(Vector2 pnt);
	void MoveTo(Vector2 pnt);

	/// <summary>
	/// Calculate the point on the edge of the bounding area in the direction of <paramref name="pnt"/>.
	/// </summary>
	Vector2 Clamp(Vector2 pnt);

	/// <summary>
	/// Reflect a velocity vector based on the boundary at the given point.
	/// </summary>
	Vector2 ReflectVelocity(Vector2 position, Vector2 velocity);
}
