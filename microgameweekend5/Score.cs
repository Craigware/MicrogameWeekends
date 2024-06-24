using Godot;
using System;

public partial class Score : RichTextLabel
{	
	public override void _Process(double delta)
	{
        Text = "Score - " + GetTree().Root.GetNode<GameState>("Main").Score.ToString();
	}
}
