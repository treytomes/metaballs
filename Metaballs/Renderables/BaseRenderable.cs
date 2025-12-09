using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace Metaballs.Renderables;

abstract class BaseRenderable : IRenderable
{
	#region Properties

	public IRenderable? Parent { get; set; } = null;
	public Vector2 Position { get; set; } = Vector2.Zero;

	public Vector2 GlobalPosition
	{
		get
		{
			if (Parent == null)
			{
				return Position;
			}
			return Parent.GlobalPosition + Position;
		}
	}

	public bool IsVisible { get; set; } = true;

	#endregion

	#region Methods

	public abstract void Render(IRenderingContext rc);

	public abstract IRenderable Clone();

	object ICloneable.Clone()
	{
		return Clone();
	}

	#endregion
}
