using RetroTK;
using RetroTK.Gfx;
using RetroTK.Services;
using RetroTK.States;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
using Metaballs.Fire.Particles;
using Metaballs.Fire;

namespace Metaballs;

/// <summary>
/// Arrow keys and WASD can change the fire parameters.  Mouse wheel changes the pen size.  Left click to draw.
/// Space bar to write text.
/// 
/// I had to slow the flame simulation way down in order for the drawing to be visible.
/// It might look nicer to setup some kind of particle effect.
/// </summary>
/// <remarks>
/// Metaballs demo from the algorithm described here: https://lodev.org/cgtutor/fire.html
/// </remarks>
class FireSimState : GameState
{
	#region Fields

	private readonly FireBuffer _fire;

	private bool _isMouseDown = false;
	private Vector2 _mousePosition = Vector2.Zero;
	private readonly ParticleFountain _drawingFountain;

	private GlyphSet<Bitmap>? _tiles = null;
	private ConsumableTileSet _consumables = new(new ParticleFountainFactory());

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="TileMapTestState"/> class.
	/// </summary>
	/// <param name="resources">Resource manager for loading assets.</param>
	/// <param name="rc">Rendering context for drawing.</param>
	public FireSimState(IResourceManager resources, IRenderingContext rc)
		: base(resources, rc)
	{
		_fire = new(rc.Width, rc.Height, TimeSpan.FromMilliseconds(10));

		var fountainFactory = new ParticleFountainFactory();
		_drawingFountain = fountainFactory.CreateDrawingFountain();
	}

	#endregion

	#region Methods

	/// <summary>
	/// Loads resources and initializes the state.
	/// </summary>
	public override void Load()
	{
		base.Load();

		var image = Resources.Load<Image>("oem437_8.png");
		var bmp = new Bitmap(image);
		_tiles = new GlyphSet<Bitmap>(bmp, 8, 8);
	}

	/// <summary>
	/// Unloads resources and cleans up the state.
	/// </summary>
	public override void Unload()
	{
		base.Unload();
	}

	/// <summary>
	/// Called when this state becomes the active state.
	/// </summary>
	public override void AcquireFocus()
	{
		base.AcquireFocus();
	}

	/// <summary>
	/// Called when this state is no longer the active state.
	/// </summary>
	public override void LostFocus()
	{
		base.LostFocus();
	}

	/// <summary>
	/// Renders the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public override void Render(GameTime gameTime)
	{
		RC.Clear();
		_drawingFountain.Render(_fire);
		_fire.Render(RC);

		if (_tiles != null)
		{
			_consumables.Render(RC, _tiles, _fire);
		}


		base.Render(gameTime);
	}

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public override void Update(GameTime gameTime)
	{
		_drawingFountain.Update(gameTime);
		_fire.Update(gameTime);
		if (_tiles != null)
		{
			_consumables.Update(gameTime, _tiles);
		}
		base.Update(gameTime);
	}

	/// <summary>
	/// Handles key down events.
	/// </summary>
	/// <param name="e">Key event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool KeyDown(KeyboardKeyEventArgs e)
	{
		return base.KeyDown(e);
	}

	/// <summary>
	/// Handles key up events.
	/// </summary>
	/// <param name="e">Key event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool KeyUp(KeyboardKeyEventArgs e)
	{
		// Note: You can get some neat affects by modifying the breadth/depth followed by the smoothing scale.
		// The smoother scale has less of an effect as the other numbers increase.
		// The banding effect is interesting.

		var delta = e.Key.GetAxis(Keys.A, Keys.D);
		if (delta != 0)
		{
			_fire.SmoothingScale += delta * 0.001f;
			Console.WriteLine($"_fire.SmoothingScale: {_fire.SmoothingScale}");
		}
		delta = e.Key.GetAxis(Keys.Left, Keys.Right);
		if (delta != 0)
		{
			_fire.Breadth += (int)delta;
			Console.WriteLine($"_fire.Breadth: {_fire.Breadth}");
		}
		delta = e.Key.GetAxis(Keys.Down, Keys.Up);
		if (delta != 0)
		{
			_fire.Depth += (int)delta;
			Console.WriteLine($"_fire.Depth: {_fire.Depth}");
		}

		switch (e.Key)
		{
			case Keys.Space:
				_consumables.WriteString("Hello, world! :-D", new Vector2(100, 100), RadialColor.White, TimeSpan.FromMilliseconds(100));
				break;
		}
		return base.KeyUp(e);
	}

	public override bool MouseWheel(MouseWheelEventArgs e)
	{
		_drawingFountain.Scale += (int)e.OffsetY;
		if (_drawingFountain.Scale < 1)
		{
			_drawingFountain.Scale = 1;
		}
		return base.MouseWheel(e);
	}

	/// <summary>
	/// Handles mouse move events.
	/// </summary>
	/// <param name="e">Mouse move event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseMove(MouseMoveEventArgs e)
	{
		_mousePosition = e.Position;
		if (_isMouseDown)
		{
			_drawingFountain.MoveTo(_mousePosition);
		}
		return base.MouseMove(e);
	}

	/// <summary>
	/// Handles mouse down events.
	/// </summary>
	/// <param name="e">Mouse button event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseDown(MouseButtonEventArgs e)
	{
		if (e.Button == MouseButton.Left)
		{
			_isMouseDown = true;
			_drawingFountain.MoveTo(_mousePosition);
			_drawingFountain.IsActive = true;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Handles mouse up events.
	/// </summary>
	/// <param name="e">Mouse button event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseUp(MouseButtonEventArgs e)
	{
		if (e.Button == MouseButton.Left)
		{
			_isMouseDown = false;
			_drawingFountain.IsActive = false;
			return true;
		}
		return false;
	}

	#endregion
}