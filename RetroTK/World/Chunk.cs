using RetroTK.IO;
using OpenTK.Mathematics;

namespace RetroTK.World;

/// <summary>
/// Represents a fixed-size chunk of tiles in the game world.
/// </summary>
public class Chunk
{
	#region Fields

	/// <summary>
	/// The two-dimensional array of tile references that make up this chunk.
	/// </summary>
	private readonly TileRef[,] _tiles;

	/// <summary>
	/// The two-dimensional array of metadata for this chunk.
	/// </summary>
	private readonly ulong[,] _data;

	#endregion

	#region Properties

	/// <summary>
	/// Gets the size of this chunk (width and height in tiles).
	/// </summary>
	public int Size => _tiles.GetLength(0);

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="Chunk"/> class with the specified size.
	/// </summary>
	/// <param name="size">The size of the chunk (width and height in tiles).</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when size is less than 1.</exception>
	public Chunk(int size)
	{
		if (size < 1)
			throw new ArgumentOutOfRangeException(nameof(size), "Chunk size must be at least 1");

		_tiles = new TileRef[size, size];
		_data = new ulong[size, size];

		// Initialize all tiles to Empty.
		Clear();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Chunk"/> class from a serializable chunk.
	/// </summary>
	/// <param name="chunk">The serializable chunk to create this chunk from.</param>
	/// <exception cref="ArgumentNullException">Thrown when chunk is null.</exception>
	/// <exception cref="ArgumentException">Thrown when the chunk size doesn't match the tiles array dimensions.</exception>
	public Chunk(SerializableChunk chunk)
	{
		if (chunk == null)
			throw new ArgumentNullException(nameof(chunk));

		if (chunk.Size != chunk.Tiles.GetLength(0) || chunk.Size != chunk.Tiles.GetLength(1))
			throw new ArgumentException("Chunk size does not match tiles array dimensions", nameof(chunk));

		_tiles = new TileRef[chunk.Size, chunk.Size];
		_data = new ulong[chunk.Size, chunk.Size];

		// Copy tiles from serializable chunk
		for (var x = 0; x < chunk.Size; x++)
		{
			for (var y = 0; y < chunk.Size; y++)
			{
				_tiles[x, y] = chunk.Tiles[x, y];
				_data[x, y] = chunk.Data[x, y];
			}
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// Converts this chunk to a serializable representation.
	/// </summary>
	/// <returns>A serializable representation of this chunk.</returns>
	public SerializableChunk ToSerializable()
	{
		var size = Size;
		var tiles = new TileRef[size, size];
		var data = new ulong[size, size];

		// Copy tiles to the serializable chunk.
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				tiles[x, y] = _tiles[x, y];
				data[x, y] = _data[x, y];
			}
		}

		return new SerializableChunk()
		{
			Size = size,
			Tiles = tiles,
			Data = data,
		};
	}

	/// <summary>
	/// Gets the tile at the specified position within the chunk.
	/// </summary>
	/// <param name="position">The position vector within the chunk.</param>
	/// <returns>The tile reference at the specified position, or TileRef.Empty if out of bounds.</returns>
	public TileRef GetTile(Vector2 position)
	{
		return GetTile((int)position.X, (int)position.Y);
	}

	/// <summary>
	/// Gets the tile at the specified coordinates within the chunk.
	/// </summary>
	/// <param name="x">The X coordinate within the chunk.</param>
	/// <param name="y">The Y coordinate within the chunk.</param>
	/// <returns>The tile reference at the specified coordinates, or TileRef.Empty if out of bounds.</returns>
	public TileRef GetTile(int x, int y)
	{
		if (!IsInBounds(x, y))
			return TileRef.Empty;

		return _tiles[x, y];
	}

	/// <summary>
	/// Sets the tile at the specified position within the chunk.
	/// </summary>
	/// <param name="position">The position vector within the chunk.</param>
	/// <param name="tileId">The ID of the tile to set.</param>
	/// <returns>True if the tile was set; false if the position was out of bounds.</returns>
	public bool SetTile(Vector2 position, int tileId)
	{
		return SetTile((int)position.X, (int)position.Y, tileId);
	}

	/// <summary>
	/// Sets the tile at the specified coordinates within the chunk.
	/// </summary>
	/// <param name="x">The X coordinate within the chunk.</param>
	/// <param name="y">The Y coordinate within the chunk.</param>
	/// <param name="tileId">The ID of the tile to set.</param>
	/// <returns>True if the tile was set; false if the coordinates were out of bounds.</returns>
	public bool SetTile(int x, int y, int tileId)
	{
		if (!IsInBounds(x, y))
			return false;

		// Since TileRef is immutable, we create a new instance
		_tiles[x, y] = new TileRef(tileId);
		return true;
	}

	/// <summary>
	/// Sets a tile metadata at the specified chunk position.
	/// </summary>
	/// <param name="position">The position in chunk coordinates.</param>
	/// <param name="data">The data value to set.</param>
	/// <returns>True if the tile metadata  was set; false if the position was invalid.</returns>
	public bool SetData(Vector2 position, ulong data)
	{
		return SetData((int)position.X, (int)position.Y, data);
	}

	/// <summary>
	/// Sets a tile metadata at the specified chunk coordinates.
	/// </summary>
	/// <param name="x">The X coordinate in chunk space.</param>
	/// <param name="y">The Y coordinate in chunk space.</param>
	/// <param name="data">The data value to set.</param>
	/// <returns>True if the tile metadata was set; false if the coordinates were invalid.</returns>
	public bool SetData(int x, int y, ulong data)
	{
		if (!IsInBounds(x, y))
			return false;

		_data[x, y] = data;
		return true;
	}

	/// <summary>
	/// Gets a tile metadata at the specified chunk position.
	/// </summary>
	/// <param name="position">The position in chunk coordinates.</param>
	/// <returns>The tile metadata at the specified position.</returns>
	public ulong GetData(Vector2 position)
	{
		return GetData((int)position.X, (int)position.Y);
	}

	/// <summary>
	/// Gets a tile metadata at the specified world coordinates.
	/// </summary>
	/// <param name="x">The X coordinate in chunk space.</param>
	/// <param name="y">The Y coordinate in chunk space.</param>
	/// <returns>The tile metadata at the specified coordinates.</returns>
	public ulong GetData(int x, int y)
	{
		if (!IsInBounds(x, y))
			return 0;

		return _data[x, y];
	}

	/// <summary>
	/// Determines whether the specified coordinates are within the bounds of this chunk.
	/// </summary>
	/// <param name="x">The X coordinate to check.</param>
	/// <param name="y">The Y coordinate to check.</param>
	/// <returns>True if the coordinates are within bounds; otherwise, false.</returns>
	public bool IsInBounds(int x, int y)
	{
		return x >= 0 && x < Size && y >= 0 && y < Size;
	}

	/// <summary>
	/// Clears all tiles in this chunk, setting them to TileRef.Empty.
	/// </summary>
	public void Clear()
	{
		int size = Size;
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				_tiles[x, y] = TileRef.Empty;
				_data[x, y] = 0;
			}
		}
	}

	/// <summary>
	/// Returns a count of non-empty tiles in this chunk.
	/// </summary>
	/// <returns>The number of non-empty tiles.</returns>
	public int CountNonEmptyTiles()
	{
		int count = 0;
		int size = Size;

		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				if (!_tiles[x, y].IsEmpty)
					count++;
			}
		}

		return count;
	}

	/// <summary>
	/// Determines whether this chunk is completely empty (contains only empty tiles).
	/// </summary>
	/// <returns>True if all tiles are empty; otherwise, false.</returns>
	public bool IsEmpty()
	{
		return CountNonEmptyTiles() == 0;
	}

	#endregion
}