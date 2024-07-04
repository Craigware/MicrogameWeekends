using Godot;
using System;

public partial class Sun : DirectionalLight3D
{	
	public override void _Process(double delta)
	{
        RotationDegrees = RotationDegrees + new Vector3(1,0,0) * (float)delta;
	}
}
