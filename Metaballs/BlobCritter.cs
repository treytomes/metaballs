using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK;

namespace Metaballs;

// Note: A stick blob collection will pull all of it's blobs towards the center of mass.
// Each blob will need a velocity.  There should also be some kind of spring constant.

class BlobCritter : BlobCollection<Blob>
{
	#region Fields

	#endregion

	#region Constructors

	public BlobCritter(MetaballsSettings settings, int width, int height, Vector2 position, CreateBlobCritterProps props)
		: base(settings, width, height)
	{
		Position = position;
		Speed = props.MinSpeed + Random.Shared.NextSingle() * (props.MaxSpeed - props.MinSpeed);
		Friction = props.MinFriction + Random.Shared.NextSingle() * (props.MaxFriction - props.MinFriction);

		var factory = new BlobFactory(settings);
		var numBlobs = Random.Shared.Next(props.MinNumBlobs, props.MaxNumBlobs + 1);
		for (var n = 0; n < numBlobs; n++)
		{
			_blobs.Add(factory.CreateRadialBlob(Position, props.BlobProps));
		}
	}

	#endregion

	#region Properties

	public Vector2 Velocity { get; set; } = Vector2.Zero;
	public float Speed { get; set; }
	public float Friction { get; set; }

	#endregion

	#region Methods

	public override void Update(GameTime gameTime)
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
			blob.Update(gameTime);
		});

		Position += Velocity * Speed * (float)gameTime.ElapsedTime.TotalSeconds;

		// After the critter moves in its chosen direction, they get pulled back a bit by their own inertia towards the center of mass.
		var centerOfMass = CalculateCenterOfMass();
		var inertia = Speed / 2;
		var massDistance = centerOfMass - Position;
		var massDirection = massDistance.Normalized();
		Position += massDirection * inertia * (float)gameTime.ElapsedTime.TotalSeconds;
	}

	public void MouseMove(MouseMoveEventArgs e)
	{
		// Note: This is more of a "mouse-following" behavior.
		var distance = e.Position - Position;
		var direction = distance.Normalized();
		Velocity = direction;
		Speed = (float)(distance.Length / 10.0f);
	}

	#endregion
}