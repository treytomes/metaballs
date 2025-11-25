using System.Reflection.Metadata;
using Metaballs.Behaviors;
using Metaballs.Props;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK;
using RetroTK.Events;
using RetroTK.Gfx;

namespace Metaballs;

// Note: A stick blob collection will pull all of it's blobs towards the center of mass.
// Each blob will need a velocity.  There should also be some kind of spring constant.

class BlobCritter : IEventHandler
{
	#region Fields

	private readonly EventBlobCollection _blobs;
	private readonly List<BlobCritterBehavior> _behaviors = new();

	#endregion

	#region Constructors

	public BlobCritter(MetaballsSettings settings, int width, int height, Vector2 position, CreateBlobCritterProps props)
	{
		_blobs = new(settings, width, height);

		Position = position;
		Speed = props.MinSpeed + Random.Shared.NextSingle() * (props.MaxSpeed - props.MinSpeed);
		Friction = props.MinFriction + Random.Shared.NextSingle() * (props.MaxFriction - props.MinFriction);

		var factory = new BlobFactory(settings);
		var numBlobs = Random.Shared.Next(props.MinNumBlobs, props.MaxNumBlobs + 1);
		for (var n = 0; n < numBlobs; n++)
		{
			_blobs.Add(new EventBlob(factory.CreateRadialBlob(Position, props.BlobProps)));
		}

		BaseRadius = _blobs.Max(x => x.Radius) * 2;
		MaxRadius = BaseRadius * (1 + props.StretchScale);

		_behaviors.Add(new MouseFollowingBlobCritterBehavior(this));
	}

	#endregion

	#region Properties

	public Vector2 Position { get; private set; }
	public Vector2 Velocity { get; set; } = Vector2.Zero;
	public float Speed { get; set; }
	public float Friction { get; set; }

	private float BaseRadius { get; set; }
	private float MaxRadius { get; set; }

	#endregion

	#region Methods

	public void Render(IRenderingContext rc)
	{
		_blobs.Render(rc);
	}

	public void Update(GameTime gameTime)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.Update(gameTime);
		}

		Parallel.ForEach(_blobs, blob =>
		{
			// Calculate the acceleration for this frame.

			var distVector = Position - blob.Position;
			// var distance = distVector.Length;
			var direction = distVector.Normalized();

			var targetPosition = Position - direction * blob.Radius;

			var distFromTarget = (targetPosition - blob.Position).Length;

			// The distance should make a bigger difference on acceleration as the blob gets further away.
			var springForce = distFromTarget * distFromTarget * 1.5f / 10f; // * 0.1f;

			blob.Velocity += (float)gameTime.ElapsedTime.TotalSeconds * direction * springForce * blob.Radius / Friction;

			// Note: This code doesn't work.  I need to give each Blob some kind of IBoundingArea.
			// if ((Position - blob.Position).Length > MaxRadius)
			// {
			// 	blob.Velocity = -blob.Velocity;
			// }

			// if (Left < 0 || Right >= rc.Width)
			// {
			// 	Velocity = new Vector2(-Velocity.X, Velocity.Y);
			// }
			// if (Top < 0 || Bottom >= rc.Height)
			// {
			// 	Velocity = new Vector2(Velocity.X, -Velocity.Y);
			// }
		});
		_blobs.Update(gameTime);

		Position += Velocity * Speed * (float)gameTime.ElapsedTime.TotalSeconds;

		// Bounds off the screen walls?
		// if (Position.X < 0 || Position.X >= rc.Width)
		// {
		// 	Velocity = new Vector2(-Velocity.X, Velocity.Y);
		// }
		// if (Top < 0 || Bottom >= rc.Height)
		// {
		// 	Velocity = new Vector2(Velocity.X, -Velocity.Y);
		// }

		// After the critter moves in its chosen direction, they get pulled back a bit by their own inertia towards the center of mass.
		// var centerOfMass = _blobs.CenterOfMass;
		// var inertia = Speed / 2;
		// var massDistance = centerOfMass - Position;
		// var massDirection = massDistance.Normalized();
		// Position += massDirection * inertia * (float)gameTime.ElapsedTime.TotalSeconds;
	}

	public void Add(EventBlob blob)
	{
		_blobs.Add(blob);
	}

	public void Remove(EventBlob blob)
	{
		_blobs.Remove(blob);
	}

	public bool MouseMove(MouseMoveEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.MouseMove(e);
		}

		return _blobs.MouseMove(e);
	}

	public bool MouseDown(MouseButtonEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.MouseDown(e);
		}

		return _blobs.MouseDown(e);
	}

	public bool MouseUp(MouseButtonEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.MouseUp(e);
		}

		return _blobs.MouseUp(e);
	}

	public bool MouseWheel(MouseWheelEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.MouseWheel(e);
		}

		return _blobs.MouseWheel(e);
	}

	public bool KeyDown(KeyboardKeyEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.KeyDown(e);
		}

		return _blobs.KeyDown(e);
	}

	public bool KeyUp(KeyboardKeyEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.KeyUp(e);
		}

		return _blobs.KeyUp(e);
	}

	public bool TextInput(TextInputEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.TextInput(e);
		}

		return _blobs.TextInput(e);
	}

	#endregion
}