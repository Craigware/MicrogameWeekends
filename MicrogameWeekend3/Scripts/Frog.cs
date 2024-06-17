using Godot;
using System;

public partial class Frog : CharacterBody2D
{
    public void Die() {
        var frog = GD.Load<PackedScene>("res://Frog.tscn").Instantiate<Frog>();
        GetParent().AddChild(frog);
        
        int randomX = new Random().Next(0,10) * 16;
        int randomY = new Random().Next(0,10) * 16;


        frog.Position = new(randomX, randomY);
        QueueFree();
    }
}
