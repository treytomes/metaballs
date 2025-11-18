using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RetroTK;

public static class KeysExtensions
{
	public static float GetAxis(this Keys @this, Keys negative, Keys positive)
	{
		if (@this == negative)
		{
			return -1f;
		}
		if (@this == positive)
		{
			return 1f;
		}
		return 0f;
	}
}