using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace RetroTK.World;

public interface ITile
{
	int Id { get; }
	void Render(IRenderingContext rc, Vector2 position);
}