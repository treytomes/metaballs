using RetroTK.Gfx;

namespace Metaballs.Colors;

class MetaballsPalette : Palette
{
	// Note: Changing the palette size has no affect on the math.
	// Increasing the palette adds in some whites, which makes a neat glowing effect.
	public MetaballsPalette(int size = DEFAULT_PALETTE_SIZE + 16)
		: base(x => RadialColor.FromHSL((byte)(x / 3), 255, (byte)Math.Min(255, x * 2)), size)
	{
	}
}