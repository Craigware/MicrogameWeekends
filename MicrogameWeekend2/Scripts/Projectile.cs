using Godot;
using Model;

namespace Controller
{
    public partial class Projectile : Entity 
    {
        public Vector2 Direction;
    
        public enum EntityTypes {
            Ally    = 0,
            Enemy   = 1,
            Neutral = 2
        }

        [Export] public EntityTypes EntityType;

        public override void _Ready() {
            AreaEntered += ObjectHit;
        }

        public override void Action() {
            Position += Direction;
        }
    
        public virtual void ObjectHit(Node2D collider) {
            if (collider is Ally or Player && EntityType == EntityTypes.Enemy) {
                Ally ally = (Ally) collider;
                ally.Damage(10);
                Die();
            }
        }
    }
}