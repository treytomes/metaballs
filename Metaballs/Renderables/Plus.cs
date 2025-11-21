using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace Metaballs.Renderables;

class Plus(Vector2 position, int size, RadialColor color)
{
	#region Properties

	public Vector2 Position { get; set; } = position;
	public int Width { get; set; } = size;
	public int Height { get; set; } = size;
	public RadialColor VerticalColor { get; set; } = color;
	public RadialColor HorizontalColor { get; set; } = color;

	private int HorizontalRadius => Height / 2;
	private int VerticalRadius => Height / 2;

	#endregion

	#region Methods

	public void Render(IRenderingContext rc)
	{
		rc.RenderLine(Position - Vector2.UnitX * HorizontalRadius, Position + Vector2.UnitX * HorizontalRadius, HorizontalColor);
		rc.RenderLine(Position - Vector2.UnitY * VerticalRadius, Position + Vector2.UnitY * VerticalRadius, VerticalColor);
	}

	#endregion
}