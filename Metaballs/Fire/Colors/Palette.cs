using RetroTK.Gfx;

namespace Metaballs.Fire.Colors;

class Palette : IPalette
{
	#region Constants

	protected const int DEFAULT_PALETTE_SIZE = 256;

	#endregion

	#region Fields

	private readonly List<RadialColor> _colors = new();

	#endregion

	#region Constructors

	public Palette(Func<int, RadialColor> generator, int size = DEFAULT_PALETTE_SIZE)
	{
		Size = size;

		for (var n = 0; n < size; n++)
		{
			_colors.Add(generator(n));
		}
	}

	#endregion

	#region Properties

	public int Size { get; }

	public RadialColor this[int index]
	{
		get
		{
			return _colors[index];
		}
	}

	#endregion
}
