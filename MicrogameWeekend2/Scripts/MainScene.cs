using Godot;
using Manager;
using System;

public partial class MainScene : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetNode<GameState>("/root/GameState").Reset();
	}
}
