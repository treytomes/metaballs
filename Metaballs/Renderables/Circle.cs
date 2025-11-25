using RetroTK.Gfx;

namespace Metaballs.Renderables;

class Circle : BaseRenderable
{
	#region Properties

	public int Radius { get; set; }
	public RadialColor OutlineColor { get; set; }

	#endregion

	#region Methods

	public override void Render(IRenderingContext rc)
	{
		if (!IsVisible) return;

		rc.RenderCircle(GlobalPosition, Radius, OutlineColor);
	}

	public override IRenderable Clone()
	{
		return new Circle()
		{
			Position = Position,
			Radius = Radius,
			OutlineColor = OutlineColor,
		};
	}

	#endregion
}