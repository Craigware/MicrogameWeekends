using Godot;

namespace Entity
{
    public partial class Sproutling : CharacterBody2D
    {
        public const float Speed = 50.0f;
        [Export] public string Drop;
        Sprite2D sprite;
        Vector2 movementDirection = new Vector2(1, 0);
        public bool IsPickedUp = false;

        public Sproutling(
            Texture2D texture,
            string drop
        ) {
            Drop = drop;
            sprite = new(){
                Texture = texture
            };
            AddChild(sprite);
            CollisionShape2D collision = new() {
                Shape = new RectangleShape2D() {
                    Size = new(17, 17)
                }
            };

            AddChild(collision);
        }

        public override void _PhysicsProcess(double delta) {
            if (Position.Y > 500) {
                Die();
            }

            if (!IsPickedUp) Path(delta);
        }

        public void Die() {
            QueueFree();
        }

        public void Path(double delta) {
            if (IsPickedUp) return;

            Vector2 vel = new();
            if (!IsOnFloor()) {
                vel += GetGravity() * (float)delta * 10.0f;
            }
            if (GetLastSlideCollision() != null && GetLastSlideCollision().GetCollider() is Player or Sproutling) {
                CharacterBody2D body = (CharacterBody2D) GetLastSlideCollision().GetCollider();
                movementDirection *= -1;
            }

            if (GetLastSlideCollision() != null && GetLastSlideCollision().GetCollider() is Entity e) {
                e.Feed(Drop);
                Die();
            }

            vel.X = movementDirection.X * Speed;
            Velocity = vel;
            MoveAndSlide();
        }
    }
}