using RetroTK.Gfx;

namespace Metaballs.Fire.Colors;

interface IPalette
{
	int Size { get; }
	RadialColor this[int index] { get; }
}
