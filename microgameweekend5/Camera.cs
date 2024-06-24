using Godot;
using System;

public partial class Camera : Camera2D
{
    public Node2D target;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        target = GetParent().GetNode<Node2D>("Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (target == null) return;

        var updatedPos = target.Position;

        if (target.Position.Y >= 375) {
            updatedPos.Y = 375;
        }

        Position = updatedPos;

	}
}
