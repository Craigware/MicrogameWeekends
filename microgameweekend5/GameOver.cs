using Godot;
using System;

public partial class GameOver : Control
{
    public override void _Ready() {
        GetNode<RichTextLabel>("VBoxContainer/Score").Text = "[center]Score = " + GetParent().GetParent<GameState>().Score;
    }

    public override void _Input(InputEvent @event) {
        if (Input.IsActionJustPressed("Restart")) {
            GetTree().ReloadCurrentScene();
        }
    }
}
