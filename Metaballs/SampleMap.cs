namespace Metaballs;

class SampleMap
{
	#region Fields

	private float[,] _data;

	#endregion

	#region Constructors

	public SampleMap(int width, int height)
	{
		Width = width;
		Height = height;
		_data = new float[height, width];
		Array.Clear(_data);
	}

	#endregion

	#region Properties

	public int Width { get; }
	public int Height { get; }

	public float this[int y, int x]
	{
		get
		{
			if (y < 0 || y >= Height || x < 0 || x >= Width) return 0;
			return _data[y, x];
		}
		set
		{
			if (y < 0 || y >= Height || x < 0 || x >= Width) return;
			_data[y, x] = value;
		}
	}

	#endregion

	#region Methods

	public void Clear()
	{
		Array.Clear(_data);
	}

	#endregion
}