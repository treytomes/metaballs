using RetroTK.World;

namespace RetroTK.IO;

public class SerializableChunk
{
	public int Size { get; set; }
	public required TileRef[,] Tiles { get; set; }
	public required ulong[,] Data { get; set; }
}
