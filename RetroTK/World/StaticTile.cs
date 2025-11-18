using RetroTK.Gfx;
using OpenTK.Mathematics;

namespace RetroTK.World;

/// <summary>
/// Represents a tile that will always render as a single image.
/// </summary>
public class StaticTile : ITile
{
	#region Fields

	private readonly IImageRef _image;

	#endregion

	#region Constructors

	public StaticTile(int id, IImageRef image)
	{
		Id = id;
		_image = image;
	}

	#endregion

	#region Properties

	public int Id { get; }

	#endregion

	#region Methods

	public void Render(IRenderingContext rc, Vector2 position)
	{
		_image.Render(rc, position);
	}

	#endregion
}