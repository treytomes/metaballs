using OpenTK.Mathematics;

namespace RetroTK.Gfx;

public interface IImageRef
{
	void Render(IRenderingContext rc, Vector2 position);
}