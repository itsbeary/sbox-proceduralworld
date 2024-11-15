public sealed class RustFreeCamera : Component
{
	Angles currentAngle;
	[Property] public float WalkSpeed { get; set; } = 20f;
	[Property] public float RunSpeed { get; set;  } = 40f;

	protected override void OnUpdate()
	{
		currentAngle += Input.AnalogLook;
		currentAngle.pitch = currentAngle.pitch.Clamp( -89, 89 );
		LocalRotation = new Angles( currentAngle.pitch, currentAngle.yaw, 0f );

		LocalPosition += LocalRotation * ((Input.Down("Run") ? RunSpeed : WalkSpeed) * Input.AnalogMove);
		if ( Input.Down( "jump" ) )
			LocalPosition += new Vector3(0,0, 5f);
		if(Input.Down("Duck"))
			LocalPosition -= new Vector3( 0, 0, 5f);
	}
}
