using RetroTK.Gfx;

namespace Metaballs.Colors;

interface IPalette
{
	int Size { get; }
	RadialColor this[int index] { get; }
}
