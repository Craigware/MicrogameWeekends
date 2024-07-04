using Godot;

[GlobalClass]
public partial class Entity : CharacterBody3D 
{
    [Export] private int maxHealth;
    private int health;
    public int Health {
        get { return health; }
        set {
            if (value <= 0) Die();
            health = value;
        }
    }

    public override void _Ready() {
        Health = maxHealth;
    }

    public override void _Process(double delta) {
        
    }

    public void Die() {
        QueueFree();
    }
}
