using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK;

namespace Metaballs;

// Note: A stick blob collection will pull all of it's blobs towards the center of mass.
// Each blob will need a velocity.  There should also be some kind of spring constant.

class StickyBlobCollection : BlobCollection<Blob>
{
	#region Fields

	// private bool _isCenterOfMassCalculated = false;

	#endregion

	#region Constructors

	public StickyBlobCollection(MetaballsSettings settings, int width, int height, IEnumerable<Blob>? blobs = null)
		: base(settings, width, height, blobs)
	{
		const int NUM_BLOBS = 3;
		const int MIN_RADIUS = 8;
		const int MAX_RADIUS = 16;

		Position = new Vector2(150, 150);

		for (var n = 0; n < NUM_BLOBS; n++)
		{
			var radius = Random.Shared.Next(MIN_RADIUS, MAX_RADIUS);
			var angle = Random.Shared.Next(0, 360) * Math.PI / 180.0f;
			var position = new Vector2(radius * (float)Math.Cos(angle), radius * (float)Math.Sin(angle));
			_blobs.Add(new Blob(Position + position, radius)
			{
				DrawOutline = true,
			});
		}

		// var fact = new BlobFactory(settings);
		// for (var n = 0; n < settings.NumBlobs; n++)
		// {
		// 	_blobs.Add(fact.CreateRandomBlob(new System.Drawing.Rectangle(0, 0, width, height)));
		// }
	}

	#endregion

	#region Properties

	public Vector2 Velocity { get; set; } = Vector2.Zero;
	public float Speed { get; set; } = 10f;
	public float Friction { get; set; } = 1000f;
	// public bool AllowFloatingCenterOfMass { get; set; } = true;

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
			var springForce = distFromTarget * distFromTarget * 1.5f; // * 0.1f;

			blob.Velocity += (float)gameTime.ElapsedTime.TotalSeconds * direction * springForce * blob.Radius / Friction;
			blob.Update(gameTime);
		});

		// if (!_isCenterOfMassCalculated)
		// {
		// 	Position = CalculateCenterOfMass();
		// 	_isCenterOfMassCalculated = true;
		// }

		Position += Velocity * Speed * (float)gameTime.ElapsedTime.TotalSeconds;
	}

	public void MouseMove(MouseMoveEventArgs e)
	{
		var distance = e.Position - Position;
		var direction = distance.Normalized();
		Velocity = direction;
		Speed = (float)(distance.Length / 10.0f);
	}

	#endregion
}