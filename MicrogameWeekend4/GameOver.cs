using Godot;
using System;

public partial class GameOver : Control
{
    public override void _Input(InputEvent @event) {
        if (Input.IsActionJustPressed("R")) {
            GetTree().ChangeSceneToFile("res://Main.tscn");
        }
    }
}
