namespace RetroTK.IO;

public class SerializableLevelChunk
{
	public required SerializableVector2i ChunkPosition { get; set; }
	public required SerializableChunk Chunk { get; set; }
}
