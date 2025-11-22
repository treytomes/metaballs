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
	}

	#endregion

	#region Properties

	public Vector2 Position { get; private set; }
	public Vector2 Velocity { get; set; } = Vector2.Zero;
	public float Speed { get; set; }
	public float Friction { get; set; }

	#endregion

	#region Methods

	public void Render(IRenderingContext rc)
	{
		_blobs.Render(rc);
	}

	public void Update(GameTime gameTime)
	{
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
		});
		_blobs.Update(gameTime);

		Position += Velocity * Speed * (float)gameTime.ElapsedTime.TotalSeconds;

		// After the critter moves in its chosen direction, they get pulled back a bit by their own inertia towards the center of mass.
		var centerOfMass = _blobs.CenterOfMass;
		var inertia = Speed / 2;
		var massDistance = centerOfMass - Position;
		var massDirection = massDistance.Normalized();
		Position += massDirection * inertia * (float)gameTime.ElapsedTime.TotalSeconds;
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
		// Note: This is more of a "mouse-following" behavior.
		var distance = e.Position - Position;
		var direction = distance.Normalized();
		Velocity = direction;
		Speed = 0.5f * distance.Length;

		return _blobs.MouseMove(e);
	}

	public bool MouseDown(MouseButtonEventArgs e)
	{
		return _blobs.MouseDown(e);
	}

	public bool MouseUp(MouseButtonEventArgs e)
	{
		return _blobs.MouseUp(e);
	}

	public bool MouseWheel(MouseWheelEventArgs e)
	{
		return _blobs.MouseWheel(e);
	}

	public bool KeyDown(KeyboardKeyEventArgs e)
	{
		return _blobs.KeyDown(e);
	}

	public bool KeyUp(KeyboardKeyEventArgs e)
	{
		return _blobs.KeyUp(e);
	}

	public bool TextInput(TextInputEventArgs e)
	{
		return _blobs.TextInput(e);
	}

	#endregion
}