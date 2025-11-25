using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace Metaballs.Renderables;

class Plus : BaseRenderable
{
	#region Properties

	public int Width { get; set; }
	public int Height { get; set; }
	public RadialColor VerticalColor { get; set; }
	public RadialColor HorizontalColor { get; set; }

	public int Size
	{
		get
		{
			return Width;
		}
		set
		{
			Width = value;
			Height = value;
		}
	}

	public RadialColor Color
	{
		get
		{
			return VerticalColor;
		}
		set
		{
			VerticalColor = value;
			HorizontalColor = value;
		}
	}

	private int HorizontalRadius => Width / 2;
	private int VerticalRadius => Height / 2;

	#endregion

	#region Methods

	public override void Render(IRenderingContext rc)
	{
		if (!IsVisible) return;

		rc.RenderLine(GlobalPosition - Vector2.UnitX * HorizontalRadius, GlobalPosition + Vector2.UnitX * HorizontalRadius, HorizontalColor);
		rc.RenderLine(GlobalPosition - Vector2.UnitY * VerticalRadius, GlobalPosition + Vector2.UnitY * VerticalRadius, VerticalColor);
	}

	public override IRenderable Clone()
	{
		return new Plus()
		{
			Position = Position,
			Width = Width,
			Height = Height,
			VerticalColor = VerticalColor,
			HorizontalColor = HorizontalColor,
		};
	}

	#endregion
}