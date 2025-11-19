using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace Metaballs;

class EventBlob : Blob
{
	#region Constructors

	public EventBlob(Vector2 position, int radius)
		: base(position, radius)
	{
	}

	public EventBlob(Blob blob)
		: this(blob.Position, blob.Radius)
	{
		DrawCircles = blob.DrawCircles;
	}

	#endregion

	#region Properties

	public bool HasMouseHover { get; private set; }
	public bool HasMouseFocus { get; private set; }

	public RadialColor MouseFocusColor { get; init; } = RadialColor.Red;
	public RadialColor MouseHoverColor { get; init; } = RadialColor.Yellow;
	public RadialColor BaseColor { get; init; } = RadialColor.Gray;

	public override RadialColor Color
	{
		get
		{
			if (HasMouseFocus) return MouseFocusColor;
			if (HasMouseHover) return MouseHoverColor;
			return BaseColor;
		}
	}

	public override bool IsActive => !HasMouseFocus;

	#endregion

	#region Methods

	public void AcquireMouseHover()
	{
		HasMouseHover = true;
	}

	public void LoseMouseHover()
	{
		HasMouseHover = false;
	}

	public void AcquireMouseFocus()
	{
		HasMouseFocus = true;
	}

	public void LoseMouseFocus()
	{
		HasMouseFocus = false;
	}

	#endregion
}
