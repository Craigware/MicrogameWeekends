using Controller;
using Godot;
using System;

public partial class PlayerHud : Control
{
	Player player;
	public override void _Ready()
	{
        player = GetParent<Player>();
        GetNode<ProgressBar>("Health").MaxValue = player.MaxHealth;
        GetNode<ProgressBar>("Stamina").MaxValue = player.MaxStamina;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        GetNode<ProgressBar>("Health").Value = player.currentHealth;
        GetNode<ProgressBar>("Stamina").Value = player.currentStamina;
	}
}
