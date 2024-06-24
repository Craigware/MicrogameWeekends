using Godot;
using System;

public partial class GameState : Node2D
{
    Godot.Collections.Array<Entity.Entity> AliveAnimals = new();
    public int Score = 0;

    public override void _Ready() {
        CallDeferred(nameof(SpawnAnimals));
    }

    public void SpawnAnimals() {
        for (int i = 0; i < GetChildCount(); i++) {
            if (GetChild(i) is Entity.Entity e) {
                AliveAnimals.Add(e);
                e.Stats.Hunger = e.Stats.MaxHunger;
                e.Stats.Health = 5;
                e.Stats.Age = 0;
                e.EntityDied += () => { EntityDied(e); };
                e.EntityAte += () => { Score += 10; GetNode<SoundEffects>("/root/SoundEffects").PlaySoundEffect("scoreup"); };
            }
        }
    }

    public void EntityDied(Entity.Entity e) {
        AliveAnimals.Remove(e);
        if (AliveAnimals.Count == 0) {
            AllAnimalsDead();
        }
    }

    public void AllAnimalsDead() {
        GetNode<Player>("Player").Die();
    }
}
