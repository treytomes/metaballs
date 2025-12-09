using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace Metaballs.Renderables;

/// <summary>
/// A drawable grid component.
/// </summary>
class Grid : BaseRenderable
{
	#region Properties

	public int Width { get; set; }
	public int Height { get; set; }
	public int Resolution { get; set; }
	public RadialColor Color { get; set; }

	#endregion

	#region Methods

	public override void Render(IRenderingContext rc)
	{
		if (!IsVisible) return;

		var gridResolution = 8;
		for (var x = GlobalPosition.X - Width / 2; x <= GlobalPosition.X + Width / 2; x += gridResolution)
		{
			rc.RenderLine(new Vector2(x, 0), new Vector2(x, rc.Height - 1), Color);
		}
		for (var y = GlobalPosition.Y - Height / 2; y < GlobalPosition.Y + Height / 2; y += gridResolution)
		{
			rc.RenderLine(new Vector2(0, y), new Vector2(rc.Width - 1, y), Color);
		}
	}

	public override IRenderable Clone()
	{
		return new Grid()
		{
			Position = Position,
			Width = Width,
			Height = Height,
			Resolution = Resolution,
			Color = Color,
		};
	}

	#endregion
}