using RetroTK.Gfx;
using RetroTK.Services;
using RetroTK.World;

namespace Metaballs;

class TileRepo : ITileRepo
{
	#region Constants

	public const int GRASS_ID = 1;
	public const int DIRT_ID = 2;
	public const int ROCK_ID = 3;

	#endregion

	#region Fields

	private Dictionary<int, ITile> _tiles = new();

	#endregion

	#region Methods

	public void Load(IResourceManager resources)
	{
		var image = resources.Load<Image>("oem437_8.png");
		var bmp = new Bitmap(image);
		var tiles = new GlyphSet<Bitmap>(bmp, 8, 8);

		_tiles[GRASS_ID] = new StaticTile(GRASS_ID, new BitmapRef(tiles[176], RadialPalette.GetIndex(0, 4, 0), RadialPalette.GetIndex(0, 2, 0)));
		_tiles[DIRT_ID] = new StaticTile(DIRT_ID, new BitmapRef(tiles[176], RadialPalette.GetIndex(1, 2, 0), RadialPalette.GetIndex(1, 1, 0)));
		_tiles[ROCK_ID] = new StaticTile(ROCK_ID, new BitmapRef(tiles[178], RadialPalette.GetIndex(3, 3, 3), RadialPalette.GetIndex(1, 1, 1)));
	}

	public ITile Get(int id)
	{
		return _tiles[id];
	}

	public ITile Get(TileRef tileRef)
	{
		return _tiles[tileRef.TileId];
	}

	#endregion
}