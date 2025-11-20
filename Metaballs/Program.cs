using RetroTK;

namespace Metaballs;

class Program
{
	static async Task<int> Main(string[] args)
	{
		return await Bootstrap.Start<MetaballsAppSettings, MapCarvingState>(args);
	}
}