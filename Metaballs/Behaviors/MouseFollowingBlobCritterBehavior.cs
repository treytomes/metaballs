using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK;

namespace Metaballs.Behaviors;

class MouseFollowingBlobCritterBehavior : BlobCritterBehavior
{
	private Vector2? _trackingPosition = null;

	private const float MaxAcceleration = 600f;
	private const float MaxSpeed = 800f;

	/// <summary>
	/// The speed with which the critter will adjust to the mouse position.
	/// </summary>
	private const float Responsiveness = 0.5f;
	// private const float Responsiveness = 20f;

	public MouseFollowingBlobCritterBehavior(BlobCritter owner)
		: base(owner)
	{
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		if (!_trackingPosition.HasValue)
			return;

		float dt = (float)gameTime.ElapsedTime.TotalSeconds;

		// --- Compute desired velocity ---
		var toMouse = _trackingPosition.Value - Owner.Position;
		float dist = toMouse.Length;

		if (dist < 1f)
			return;

		var direction = toMouse / dist;

		// Speed proportional to distance, but capped
		float targetSpeed = MathF.Min(dist * 4f, MaxSpeed);

		Vector2 targetVelocity = direction * targetSpeed;

		// --- Smooth velocity change ---
		// dV/dt = (targetVelocity - currentVelocity) * Responsiveness
		Vector2 desiredDelta = (targetVelocity - Owner.Velocity) * Responsiveness;

		// clamp acceleration magnitude
		float desiredAccelLength = desiredDelta.Length;
		if (desiredAccelLength > MaxAcceleration)
			desiredDelta = desiredDelta / desiredAccelLength * MaxAcceleration;

		Owner.Velocity += desiredDelta * dt;

		// Update critter speed property for outside use (optional)
		Owner.Speed = 4;  //Owner.Velocity.Length;
	}

	public override bool MouseMove(MouseMoveEventArgs e)
	{
		_trackingPosition = e.Position;
		return true;
	}
}

