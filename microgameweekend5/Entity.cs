using System;
using Godot;

namespace Entity {
    [GlobalClass]
    public partial class Entity : CharacterBody2D 
    {
        float Speed = 200f;
        [Export] string EntityName;
        [Export] public EntityStats Stats;
        [Signal] public delegate void EntityDiedEventHandler();
        [Signal] public delegate void EntityAteEventHandler(); 
        public bool IsPickedUp;

        public override void _Ready() {
            Timer hungerTimer = new() {
                WaitTime = 5,
                Autostart = true
            };

            Timer healthTimer = new() {
                WaitTime = 5,
                Autostart = true
            };

            hungerTimer.Timeout += () => { Stats.Hunger = Math.Clamp(Stats.Hunger -1, 0, Stats.MaxHunger); };
            healthTimer.Timeout += () => {
                if (Stats.Hunger <= 0) {
                    Stats.Health -= 1;
                    if (Stats.Health <= 0) {
                        Die();
                    }
                }
             };

            AddChild(hungerTimer);
            AddChild(healthTimer);
        }

        public override void _PhysicsProcess(double delta) {
            if (Position.Y > 500) {
                Die();
            }

            if (IsPickedUp) return;

            var movementChance = new Random().Next(0, 1000);
            Vector2 velocity = Velocity;
            
            if (movementChance <= 20) {
                if (movementChance <= 10 && IsOnFloor()) {
                    velocity.Y = -200f;
                }
                if (movementChance % 2 == 0) {
                    velocity.X = 1 * Speed;
                    GetNode<Sprite2D>("Sprite2D").FlipH = false;
                } else {
                    velocity.X = -1 * Speed;
                    GetNode<Sprite2D>("Sprite2D").FlipH = true;
                }
            } else {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, 50);
            }
 
            if (!IsOnFloor()) {
                velocity += GetGravity() * (float)delta;
            }

            Velocity = velocity;
            MoveAndSlide();
        }

        public void Feed(string drop) {
            EmitSignal(nameof(EntityAte));
            switch(drop) {
                case "strawberry":
                    Stats.Hunger += 2;
                break;
            }
        }

        public void Die() {
            EmitSignal(nameof(EntityDied));
            QueueFree();
        }
    }
}