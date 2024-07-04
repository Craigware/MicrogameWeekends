using Godot;

[GlobalClass]
public partial class Breakable : RigidBody3D {
    [Export] private int maxHealth = 10;
    private int health;
    [Export] bool Respawns = false;
    [Export] float RespawnTime = 1.0f;
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

    public void Die() {
        if (Respawns) {
            Visible = false;
            Timer respawnTimer = new(){
                Autostart=true,
                WaitTime=RespawnTime
            };
            AddChild(respawnTimer);
            respawnTimer.Timeout += () => {
                Visible = true;
                Health = maxHealth;
                respawnTimer.QueueFree();
            };
        } else {
            QueueFree();
        }
    }
}