using RetroTK;

namespace Metaballs;

// Note: I wanted to encapsulate the logic used to frame-limit the flame update.
// It's reusable now.  Also a bit more verbose.
public class EventTrigger
{
	#region Fields

	private TimeSpan _eventTime;
	private TimeSpan _elapsedTime = TimeSpan.Zero;

	#endregion

	#region Constructors

	/// <param name="timeInMilliseconds">The number of milliseconds to wait before triggering.</param>
	public EventTrigger(TimeSpan eventTime)
	{
		_eventTime = eventTime;
	}

	#endregion

	#region Properties

	public bool IsTriggered { get; private set; }

	#endregion

	#region Methods

	public void Update(GameTime gameTime)
	{
		if (_eventTime == TimeSpan.Zero)
		{
			IsTriggered = true;
			return;
		}

		if (IsTriggered)
		{
			return;
		}

		_elapsedTime += gameTime.ElapsedTime;
		if (_elapsedTime > _eventTime)
		{
			IsTriggered = true;
		}
	}

	public void Reset()
	{
		IsTriggered = false;
		_elapsedTime = TimeSpan.Zero;
	}

	#endregion
}
