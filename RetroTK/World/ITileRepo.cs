using RetroTK.Services;

namespace RetroTK.World;

public interface ITileRepo
{
	void Load(IResourceManager resources);
	ITile Get(int id);
	ITile Get(TileRef tileRef);
}