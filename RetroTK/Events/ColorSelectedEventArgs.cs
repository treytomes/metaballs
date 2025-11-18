using RetroTK.Gfx;

namespace RetroTK.Events;

/// <summary>
/// Event args for color selection events
/// </summary>
class ColorSelectedEventArgs : EventArgs
{
	public RadialColor Color { get; }

	public ColorSelectedEventArgs(RadialColor color)
	{
		Color = color;
	}
}