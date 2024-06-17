using Godot;
using System;

public partial class HUD : Control
{  
	public override void _Process(double delta)
	{
        GetNode<RichTextLabel>("CurrentItteration").Text = GetNode<SnakeBrain>("/root/SnakeBrain").CurrentItteration.ToString();
        GetNode<RichTextLabel>("SnakeLength").Text = GetNode<Snake>("/root/Main/Snake").SnakeLength.ToString();
	}
}
