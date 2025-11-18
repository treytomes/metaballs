using Metaballs.Colors;
using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs;

class FireBuffer
{
	#region Fields

	// Note: We're going to set up double-buffering.  This will help us multithread the whole thing later.
	private readonly float[][,] _dataBuffers;
	private int _bufferIndex = 0;

	private int _breadth = 1;
	private int _depth = 1;

	// Hue goes from 0 to 85: red to yellow.
	// Saturation is always the maximum: 255.
	// Lightness is 0..255 for x=0..128, and 255 for x=128..255.
	private readonly IPalette _palette = new FirePalette();
	private readonly EventTrigger _flameUpdateTrigger;

	#endregion

	#region Constructors

	/// <param name="simulationSpeed">Measured in milliseconds between flame updates.</param>
	public FireBuffer(int width, int height, TimeSpan simulationSpeed)
	{
		_flameUpdateTrigger = new(simulationSpeed);
		Width = width;
		Height = height;

		_dataBuffers = new float[2][,];
		_dataBuffers[0] = new float[height, width];
		Array.Clear(_dataBuffers[0]);
		_dataBuffers[1] = new float[height, width];
		Array.Clear(_dataBuffers[1]);
	}

	#endregion

	#region Properties

	public float this[int y, int x]
	{
		get
		{
			return GetActiveBuffer()[y, x];
		}
		set
		{
			if (value < 0) value = 0;
			if (value > 1.0f) value = 1.0f;
			GetActiveBuffer()[y, x] = value;
		}
	}

	public int Width { get; }
	public int Height { get; }

	// Note: Floating-point fire was getting to big.  Tweaking these numbers got it back under control.
	// public float Factor0 { get; set; } = 31f;
	// public float Factor1 { get; set; } = 130f;

	public float SmoothingScale { get; set; } = 31f / 130f;

	// Note: I played with modifying the size of the area being summed up, but I don't like the results yet.
	// I imagine Factor0 and Factor1 should also be modified if breadth and depth change.
	public int Breadth
	{
		get
		{
			return _breadth;
		}
		set
		{
			if (value < 0) value = 0;
			_breadth = value;
		}
	}

	public int Depth
	{
		get
		{
			return _depth;
		}
		set
		{
			if (value < 0) value = 0;
			_depth = value;
		}
	}

	#endregion

	#region Methods

	public void Render(IRenderingContext rc)
	{
		var activeBuffer = GetActiveBuffer();

		// Set the drawing buffer to the fire buffer, using the palette colors.
		for (var y = 0; y < rc.Height; y++)
		{
			for (var x = 0; x < rc.Width; x++)
			{
				var paletteIndex = (int)(activeBuffer[y, x] * (_palette.Size - 1));
				var color = _palette[paletteIndex];
				rc.SetPixel(new Vector2(x, y), color);
			}
		}
	}

	public void Update(GameTime gameTime)
	{
		// Note: Now that we're drawing with the particle fountain, we can simulate this at full speed.
		_flameUpdateTrigger.Update(gameTime);
		if (!_flameUpdateTrigger.IsTriggered)
		{
			return;
		}
		_flameUpdateTrigger.Reset();

		var activeBuffer = GetActiveBuffer();
		var backBuffer = GetBackBuffer();

		// Randomize the bottom row of the fire buffer.
		for (var x = 0; x < Width; x++)
		{
			backBuffer[Height - 1, x] = Random.Shared.NextSingle();
		}

		// Do the fire calculations for every pixel.
		// Note: Since the buffer isn't reading and writing from itself (because of back-buffering), we can run each pixel in parallel.
		Parallel.For(0, Height - 1, y =>
			Parallel.For(0, Width, x =>
			{
				// Note: In one way the math is neater, but giving the horizontal access more influence is causing banding.
				// Increase the smoothing scale by about 50% to see good screen coverage.
				// var sum = Enumerable.Range(x - Breadth, x + Breadth - (x - Breadth) + 1)
				// 	.Sum(h => Enumerable.Range(y + Depth, Depth)
				// 		.Sum(v => activeBuffer[v % Height, (h + Width) % Width])
				// 	);

				var horizontalSum = Enumerable.Range(x - Breadth, x + Breadth - (x - Breadth) + 1)
					.Sum(h => activeBuffer[(y + 1) % Height, (h + Width) % Width]);
				var verticalSum = Enumerable.Range(y + 1 + Depth, Depth)
					.Sum(v => activeBuffer[v % Height, x % Width]);
				var sum = horizontalSum + verticalSum;

				backBuffer[y, x] = Math.Min(1.0f, sum * SmoothingScale);
			}));

		Flip();
	}

	private float[,] GetActiveBuffer()
	{
		return _dataBuffers[_bufferIndex];
	}

	private float[,] GetBackBuffer()
	{
		return _dataBuffers[1 - _bufferIndex];
	}

	private void Flip()
	{
		_bufferIndex = 1 - _bufferIndex;
	}

	#endregion
}
