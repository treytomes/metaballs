namespace RetroTK;

public class AppSettings
{
	public string AssetRoot { get; set; } = string.Empty;
	public bool Debug { get; set; }
	public WindowSettings Window { get; set; } = new();
	public VirtualDisplaySettings VirtualDisplay { get; set; } = new();
}
