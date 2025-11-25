using Metaballs.Behaviors;
using Metaballs.Bounds;
using Metaballs.Props;
using Metaballs.Renderables;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK;
using RetroTK.Events;
using RetroTK.Gfx;

namespace Metaballs;

// Note: A sticky blob collection will pull all of it's blobs towards the center of mass.
// Each blob will need a velocity.  There should also be some kind of spring constant.

class BlobCritter : IEventHandler
{
	#region Fields

	private readonly EventBlobCollection _blobs;
	private readonly List<BlobCritterBehavior> _behaviors = new();
	private readonly IBoundingArea _bounds;
	private readonly Plus _plus = new(Vector2.Zero, 6, RadialColor.Yellow);

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

		var baseRadius = _blobs.Max(x => x.Radius) * 2;

		_bounds = new CircleBoundingArea(Position, baseRadius * props.StretchScale);

		_behaviors.Add(new MouseFollowingBlobCritterBehavior(this));
	}

	#endregion

	#region Properties


	public Vector2 Position
	{
		get
		{
			return _plus.Position;
		}
		private set
		{
			_plus.Position = value;
		}
	}

	public Vector2 Velocity { get; set; } = Vector2.Zero;
	public float Speed { get; set; }
	public float Friction { get; set; }

	#endregion

	#region Methods

	public void Render(IRenderingContext rc)
	{
		_blobs.Render(rc);
		_plus.Render(rc);
		rc.RenderCircle(_bounds.Position, (int)(_bounds as CircleBoundingArea)!.Radius, RadialColor.Yellow);
	}

	public void Update(GameTime gameTime)
	{
		float dt = (float)gameTime.ElapsedTime.TotalSeconds;

		// --- Physical constants ---
		const float SpringStiffness = 45f;  // stronger cohesion
		const float SpringDamping = 10f;  // prevents jitter
		const float MassScalar = 1f;
		const float CenterPull = 1.2f; // helps goo stay cohesive

		// --- Apply spring forces to each blob in parallel ---
		Parallel.ForEach(_blobs, blob =>
		{
			float mass = blob.Radius * MassScalar;

			// Desired "rest" position of the blob
			Vector2 toBlob = blob.Position - Position;
			float dist = toBlob.Length;

			Vector2 direction;
			if (dist > 0f)
				direction = toBlob / dist;
			else
				direction = Vector2.UnitX; // arbitrary but safe fallback

			// Desired distance from center
			float targetDist = blob.Radius * 0.5f;

			// Signed displacement
			float displacement = dist - targetDist;

			// Hooke’s law spring force
			float springForceMag = SpringStiffness * displacement;

			// Velocity along spring axis (for damping)
			float radialVel = Vector2.Dot(blob.Velocity - Velocity, direction);
			float dampingForceMag = -SpringDamping * radialVel;

			// Combined force (scalar)
			float totalForceMag = springForceMag + dampingForceMag;

			// Convert scalar → vector
			Vector2 force = direction * totalForceMag;

			// Apply acceleration
			blob.Velocity += (force / mass) * dt;
		});

		// --- Update blob positions + clamp to bounding area ---
		Position += Velocity * Speed * dt;
		_bounds.MoveTo(Position);
		_blobs.Update(gameTime, _bounds); // your reflection logic applies here

		// --- Optional: pull center toward the blob cluster ---
		// This prevents the center from drifting and makes the critter act cohesive.
		Vector2 com = _blobs.CenterOfMass;
		Vector2 comOffset = com - Position;

		if (comOffset.LengthSquared > 0.0001f)
		{
			Vector2 comDir = comOffset.Normalized();
			Velocity += comDir * CenterPull * dt;
		}

		// --- Apply critter-wide friction ---
		Velocity *= (1f - Friction * dt);

		// Update behaviors (mouse chasing, wandering, etc.)
		foreach (var behavior in _behaviors)
			behavior.Update(gameTime);
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