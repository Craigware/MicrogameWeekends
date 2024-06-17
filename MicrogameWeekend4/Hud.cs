using Godot;
using System;

public partial class Hud : Control
{
	public override void _Process(double delta)
	{
        GetNode<ProgressBar>("VBoxContainer/Health").Value = GetNode<Hamster>("/root/Main/Hamster").Health;
        GetNode<ProgressBar>("VBoxContainer/Sickness").Value = GetNode<Hamster>("/root/Main/Hamster").Sickness;
        GetNode<ProgressBar>("VBoxContainer/Tiredness").Value = GetNode<Hamster>("/root/Main/Hamster").Tiredness;
        GetNode<ProgressBar>("VBoxContainer/Happyness").Value = GetNode<Hamster>("/root/Main/Hamster").Happyness;
        GetNode<ProgressBar>("VBoxContainer/Hunger").Value = GetNode<Hamster>("/root/Main/Hamster").Hunger;
    }
}
