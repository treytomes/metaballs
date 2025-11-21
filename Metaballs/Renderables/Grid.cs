using System.Drawing;
using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace Metaballs.Renderables;

/// <summary>
/// A drawable grid component.
/// </summary>
class Grid(int resolution, RadialColor color, Rectangle bounds)
{
	#region Properties

	public int Resolution { get; set; } = resolution;
	public RadialColor Color { get; set; } = color;
	public Rectangle Bounds { get; set; } = bounds;

	#endregion

	#region Methods

	public void Render(IRenderingContext rc)
	{
		var gridResolution = 8;
		for (var x = Bounds.Left; x <= Bounds.Right; x += gridResolution)
		{
			rc.RenderLine(new Vector2(x, 0), new Vector2(x, rc.Height - 1), Color);
		}
		for (var y = Bounds.Top; y < Bounds.Bottom; y += gridResolution)
		{
			rc.RenderLine(new Vector2(0, y), new Vector2(rc.Width - 1, y), Color);
		}
	}

	#endregion
}