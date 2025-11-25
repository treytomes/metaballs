using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK;

namespace Metaballs.Behaviors;

class MouseFollowingBlobCritterBehavior : BlobCritterBehavior
{
	#region Constants

	private const float MAX_ACCELERATION = 100f;
	private const float MAX_SPEED = 400f;

	/// <summary>
	/// The speed with which the critter will adjust to the mouse position.
	/// </summary>
	private const float RESPONSIVENESS = 0.5f;

	private const float SPEED_DAMPING = 0.1f;

	#endregion

	#region Fields

	private Vector2? _trackingPosition = null;

	#endregion

	#region Constructors

	public MouseFollowingBlobCritterBehavior(BlobCritter owner)
		: base(owner)
	{
	}

	#endregion

	#region Methods

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		if (!_trackingPosition.HasValue)
			return;

		float dt = (float)gameTime.ElapsedTime.TotalSeconds;

		// --- Compute desired velocity. ---
		var toMouse = _trackingPosition.Value - Owner.Position;
		float dist = toMouse.Length;

		if (dist < 1f)
			return;

		var direction = toMouse / dist;

		// Speed proportional to distance, but capped.
		var targetSpeed = MathF.Min(dist * 4f, MAX_SPEED);

		var targetVelocity = direction * targetSpeed;

		// --- Smooth velocity change. ---
		// dV/dt = (targetVelocity - currentVelocity) * RESPONSIVENESS
		var desiredDelta = (targetVelocity - Owner.Velocity) * RESPONSIVENESS;

		// Clamp acceleration magnitude.
		var desiredAccelLength = desiredDelta.Length;
		if (desiredAccelLength > MAX_ACCELERATION)
			desiredDelta = desiredDelta / desiredAccelLength * MAX_ACCELERATION;

		Owner.Velocity += desiredDelta * dt;

		// Update critter speed property for outside use (optional).
		Owner.Speed = Owner.Velocity.Length * SPEED_DAMPING;
	}

	public override bool MouseMove(MouseMoveEventArgs e)
	{
		_trackingPosition = e.Position;
		return true;
	}

	#endregion
}
