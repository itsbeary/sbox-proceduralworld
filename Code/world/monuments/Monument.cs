using System;

public class Monument : Component
{
	// The box collider for the monument
	[Property] public string Name { get; set; }
	[Property] public BoxCollider monumentCollider;
}
