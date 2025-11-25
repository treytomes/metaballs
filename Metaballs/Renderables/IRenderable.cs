using OpenTK.Mathematics;
using RetroTK.IO;
using RetroTK.Gfx;

namespace Metaballs.Renderables;

interface IRenderable : ICloneable<IRenderable>
{
	IRenderable? Parent { get; set; }
	Vector2 Position { get; set; }
	Vector2 GlobalPosition { get; }
	bool IsVisible { get; set; }
	void Render(IRenderingContext rc);
}
