using Godot;
using Manager;
using System;

public partial class GameOver : Control
{
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("Action")) {
            GetTree().ChangeSceneToFile("res://Scenes/node_2d.tscn");
        }

    }
}
