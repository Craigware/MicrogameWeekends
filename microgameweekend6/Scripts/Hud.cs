using Godot;
using System;

public partial class Hud : Control
{
    TextureRect cursor;
    RichTextLabel fps;

	public override void _Ready() {
        cursor = GetNode<TextureRect>("Cursor");
        fps = GetNode<RichTextLabel>("FPS");
    }

    public override void _Process(double delta) {
        fps.Text = Engine.GetFramesPerSecond().ToString() + " Fps";
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion e) 
            cursor.Position = e.Position - cursor.Texture.GetSize() * cursor.Scale / 2;
    }
}
