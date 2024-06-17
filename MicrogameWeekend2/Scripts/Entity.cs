using Godot;
using Manager;
using System;

namespace Model 
{
    public interface IEntity
    {
        public void Die();
        public void Action();
        public void Heal(int amount);
        public void Damage(int amount);
        public void DebugeState(string message);
        public void GainStamina(int amount);
        public void ConsumeStamina(int amount);
        public void Move(Vector2 direction);
    }

    public partial class Entity : Area2D, IEntity
    {
        [Export] public int MaxHealth;
        [Export] public int MaxStamina;

        [Signal] public delegate void StaminaDeplatedEventHandler();
        
        public int currentHealth;
        public int currentStamina;

        public override void _Ready() {
            currentHealth = MaxHealth;
            currentStamina = MaxStamina;            
        }

        public virtual void Move(Vector2 direction) {}
        public virtual void Action() {}

        public virtual void Die() {
            // play death animation or something
            GameState gameState = GetNode<GameState>("/root/GameState");
            gameState.Entities.Remove(this);
            QueueFree();
        }

        public virtual void GainStamina(int amount) {
            if (currentStamina + amount > MaxStamina) {
                currentStamina = MaxStamina;
            } else {
                currentStamina += amount;
            }
        }

        public virtual void ConsumeStamina(int amount) {
            if (currentStamina - amount <= 0) {
                currentStamina = 0;
                EmitSignal(SignalName.StaminaDeplated);              
            } else {
                currentStamina -= amount;
            }
        }

        public virtual void Heal(int amount) {
            if (currentHealth + amount <= MaxHealth) {
                currentHealth += amount;
            } else {
                currentHealth = MaxHealth;
            }
        }

        public virtual void Damage(int amount) {
            if (currentHealth - amount > 0) {
                currentHealth -= amount;
            } else {
                Die();
            }
        }

        public void DebugeState(string message) {
            GD.Print(Name + " - " + this.GetType() + " - " + message);
        }
    }
}
