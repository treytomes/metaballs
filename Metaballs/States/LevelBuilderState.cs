using RetroTK.Gfx;
using RetroTK.Services;
using RetroTK.States;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK.Events;
using RetroTK.UI;
using RetroTK.World;
using RetroTK;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Metaballs.Metaballs.States;

class LevelBuilderState : GameState
{
	#region Constants

	private const int TILE_SIZE = 8;
	private const int CHUNK_SIZE = 64;
	private const int GRID_SPACING = 16;
	private const byte GRID_INTENSITY = 3;
	private const float BASE_CAMERA_SPEED = 8 * TILE_SIZE; // 8 tiles per second
	private const float FAST_CAMERA_MULTIPLIER = 4.0f;

	#endregion

	#region Fields

	private Label _cameraLabel;
	private Label _mouseLabel;
	private Button _sampleButton;
	private IDisposable? _buttonClickSubscription;
	private Vector2 _mousePosition = Vector2.Zero;

	private Camera _camera;
	private TileRepo _tiles;
	private ILevel _level;
	private bool _isDraggingCamera = false;
	private bool _isDraggingCursor = false;
	private Vector2 _cameraDelta = Vector2.Zero;
	private bool _cameraFastMove;

	private MapCursor _mapCursor;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="TileMapTestState"/> class.
	/// </summary>
	/// <param name="resources">Resource manager for loading assets.</param>
	/// <param name="rc">Rendering context for drawing.</param>
	public LevelBuilderState(IResourceManager resources, IRenderingContext rc)
		: base(resources, rc)
	{
		_camera = new Camera();
		_tiles = new TileRepo();
		_level = new Level(CHUNK_SIZE, TILE_SIZE);
		_mapCursor = new MapCursor();

		// Initializes UI elements for this state.
		_cameraLabel = new Label(resources, rc, $"Camera:({(int)_camera.Position.X},{(int)_camera.Position.Y})", new Vector2(0, 0), new RadialColor(5, 5, 5), new RadialColor(0, 0, 0));
		UI.Add(_cameraLabel);

		_mouseLabel = new Label(resources, rc, "Mouse:(0,0)", new Vector2(0, 8), new RadialColor(5, 5, 5), new RadialColor(0, 0, 0));
		UI.Add(_mouseLabel);

		_sampleButton = new Button(resources, rc, ButtonStyle.Raised);
		_sampleButton.Position = new Vector2(32, 32);
		_sampleButton.Content = new Label(resources, rc, "> Button <", new Vector2(0, 0), new RadialColor(0, 0, 0));
		UI.Add(_sampleButton);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Loads resources and initializes the state.
	/// </summary>
	public override void Load()
	{
		base.Load();

		try
		{
			_tiles.Load(Resources);

			// Try to load the level from file, or create a new one if it fails
			try
			{
				_level = Level.Load("sample.json");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to load level: {ex.Message}. Creating a new level.");
				_level = new Level(CHUNK_SIZE, TILE_SIZE);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error loading resources: {ex.Message}");
		}
	}

	/// <summary>
	/// Unloads resources and cleans up the state.
	/// </summary>
	public override void Unload()
	{
		_buttonClickSubscription?.Dispose();
		_buttonClickSubscription = null;

		// Save the level before unloading.
		try
		{
			if (_level.HasUnsavedChanges)
			{
				_level.Save("sample.json");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to save level: {ex.Message}");
		}

		_level.Dispose();

		base.Unload();
	}

	/// <summary>
	/// Called when this state becomes the active state.
	/// </summary>
	public override void AcquireFocus()
	{
		base.AcquireFocus();

		// Subscribe to button click events using Rx
		_buttonClickSubscription = _sampleButton.ClickEvents.Subscribe(OnButtonClicked);
	}

	/// <summary>
	/// Called when this state is no longer the active state.
	/// </summary>
	public override void LostFocus()
	{
		// Dispose of the subscription when losing focus
		_buttonClickSubscription?.Dispose();
		_buttonClickSubscription = null;

		base.LostFocus();
	}

	/// <summary>
	/// Renders the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public override void Render(GameTime gameTime)
	{
		RC.Clear();
		_camera.ViewportSize = RC.ViewportSize;

		// Render grid
		RenderGrid();

		_level.Render(RC, _tiles, _camera);

		// Render the map cursor
		_mapCursor.Render(RC, gameTime, _camera);

		// Render UI elements
		base.Render(gameTime);
	}

	/// <summary>
	/// Renders the background grid.
	/// </summary>
	private void RenderGrid()
	{
		var gridColor = RadialPalette.GetIndex(GRID_INTENSITY, GRID_INTENSITY, 0);
		var gridDeltaX = RetroTK.MathHelper.FloorDiv((int)_camera.Position.X, GRID_SPACING) % GRID_SPACING;
		var gridDeltaY = RetroTK.MathHelper.FloorDiv((int)_camera.Position.Y, GRID_SPACING) % GRID_SPACING;

		for (var y = -GRID_SPACING; y < RC.Height + GRID_SPACING; y += GRID_SPACING)
		{
			RC.RenderHLine(0, RC.Width - 1, y - gridDeltaY, gridColor);
		}

		for (var x = -GRID_SPACING; x < RC.Width + GRID_SPACING; x += GRID_SPACING)
		{
			RC.RenderVLine(x - gridDeltaX, 0, RC.Height - 1, gridColor);
		}
	}

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		// Update camera position based on movement delta
		var cameraSpeed = BASE_CAMERA_SPEED * (_cameraFastMove ? FAST_CAMERA_MULTIPLIER : 1.0f);
		_camera.ScrollBy(_cameraDelta * (float)gameTime.ElapsedTime.TotalSeconds * cameraSpeed);

		// Update camera position display
		_cameraLabel.Text = StringProvider.From($"Camera:({(int)_camera.Position.X},{(int)_camera.Position.Y})");
	}

	/// <summary>
	/// Handles key down events.
	/// </summary>
	/// <param name="e">Key event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool KeyDown(KeyboardKeyEventArgs e)
	{
		_cameraFastMove = e.Shift;

		switch (e.Key)
		{
			case Keys.Escape:
				Leave();
				return true;
			case Keys.W:
				_cameraDelta = new Vector2(_cameraDelta.X, -1);
				return true;
			case Keys.S:
				_cameraDelta = new Vector2(_cameraDelta.X, 1);
				return true;
			case Keys.A:
				_cameraDelta = new Vector2(-1, _cameraDelta.Y);
				return true;
			case Keys.D:
				_cameraDelta = new Vector2(1, _cameraDelta.Y);
				return true;
		}

		return base.KeyDown(e);
	}

	/// <summary>
	/// Handles key up events.
	/// </summary>
	/// <param name="e">Key event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool KeyUp(KeyboardKeyEventArgs e)
	{
		_cameraFastMove = e.Shift;

		switch (e.Key)
		{
			case Keys.W:
			case Keys.S:
				_cameraDelta = new Vector2(_cameraDelta.X, 0);
				return true;
			case Keys.A:
			case Keys.D:
				_cameraDelta = new Vector2(0, _cameraDelta.Y);
				return true;
		}

		return base.KeyUp(e);
	}

	/// <summary>
	/// Handles mouse move events.
	/// </summary>
	/// <param name="e">Mouse move event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseMove(MouseMoveEventArgs e)
	{
		_mousePosition = e.Position;

		// Update mouse position display
		_mouseLabel.Text = StringProvider.From($"Mouse:({(int)e.Position.X},{(int)e.Position.Y})");

		// Handle camera dragging
		if (_isDraggingCamera)
		{
			_camera.ScrollBy(-e.Delta);
		}

		// Get the tile position under the cursor.
		var worldPos = _camera.ScreenToWorld(_mousePosition);

		// Floor the division result, not the world position.
		var tileX = (int)Math.Floor(worldPos.X / TILE_SIZE);
		var tileY = (int)Math.Floor(worldPos.Y / TILE_SIZE);

		var tilePos = new Vector2(tileX, tileY);

		if (_isDraggingCursor)
		{
			// Example: Toggle between grass and water when clicking.
			var currentTile = _level.GetTile(tileX, tileY);
			var newTileId = currentTile.IsEmpty || currentTile.TileId == TileRepo.DIRT_ID
				? TileRepo.GRASS_ID
				: TileRepo.ROCK_ID;

			_level.SetTile(tileX, tileY, newTileId);
		}

		// Update map cursor position.
		_mapCursor.MoveTo(tilePos * TILE_SIZE);

		return base.MouseMove(e);
	}

	/// <summary>
	/// Handles mouse down events.
	/// </summary>
	/// <param name="e">Mouse button event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseDown(MouseButtonEventArgs e)
	{
		if (base.MouseDown(e))
		{
			// Give the UI a swing at the event first.
			return true;
		}

		if (e.Button == MouseButton.Middle)
		{
			_isDraggingCamera = true;
			return true;
		}
		else if (e.Button == MouseButton.Left)
		{
			_isDraggingCursor = true;
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
		if (base.MouseUp(e))
		{
			return true;
		}

		if (e.Button == MouseButton.Middle)
		{
			_isDraggingCamera = false;
			return true;
		}
		else if (e.Button == MouseButton.Left)
		{
			_isDraggingCursor = false;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Handles button click events.
	/// </summary>
	/// <param name="e">Button click event arguments.</param>
	private void OnButtonClicked(ButtonClickedEventArgs e)
	{
		Console.WriteLine("Button pressed.");

		// Example functionality: Toggle between showing procedural terrain and saved level
		// This would require additional state tracking and rendering logic
	}

	#endregion
}