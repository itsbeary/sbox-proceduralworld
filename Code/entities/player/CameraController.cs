using Sandbox;

public sealed class CameraController : Component
{
	public Angles currentAngle;
	[Property, Group("Speed")] public float FreeCamSpeed = 1f;
	[Property, Group("Speed")] public float FreeCamFastSpeed = 2f;
	[Property, Group("Speed")] public float FreeCamSlowSpeed = 0.5f;


	private RealTimeSince fpsCounterTime = 0f;
	private float fps;

	protected override void OnUpdate()
	{
		// Mouse Movement
		currentAngle += Input.AnalogLook;
		currentAngle.pitch = currentAngle.pitch.Clamp( -89, 89 );
		LocalRotation = new Angles( currentAngle.pitch, currentAngle.yaw, 0f );

		// Keyboard
		Vector3 input = currentAngle.ToRotation() * Input.AnalogMove.Normal;
		if ( Input.Down( "duck" ) )
			input *= FreeCamSlowSpeed;
		else if ( Input.Down( "run" ) )
			input *= FreeCamFastSpeed;
		else
			input *= FreeCamSpeed;

		LocalPosition += input;
		RunFpsCount();
	}

	private float lowestFps = 999;
	private RealTimeSince sinceLowestFrame = 0;

	private float highestFps;
	private RealTimeSince sinceHighestFrame = 0;

	private void RunFpsCount()
	{
		var current = (1 / Time.Delta);
		if ( (fpsCounterTime >= 0.2) )
		{
			fpsCounterTime = 0f;
			fps = (1 / Time.Delta);
		}
		if(current <= lowestFps)
		{
			sinceLowestFrame = 0f;
			lowestFps = current;
		}
		if ( current >= highestFps )
		{
			sinceHighestFrame = 0f;
			highestFps = current;
		}
		Gizmo.Draw.ScreenText( "fixed fps : " + fps, new Vector2( 0, 0 ) );
		Gizmo.Draw.ScreenText( "current fps : " + current, new Vector2( 0, 20 ) );

		Gizmo.Draw.ScreenText( "lowest fps : " + lowestFps + " was " + sinceLowestFrame + "s", new Vector2( 0, 40 ) );
		Gizmo.Draw.ScreenText( "highest fps : " + highestFps + " was " + sinceHighestFrame + "s", new Vector2( 0, 60 ) );
	}
}
