using Godot;
using Manager;

namespace Controller
{
    public partial class BowSkeleton : Enemy 
    {
        public Weapons Weapon = Weapons.Bow;

        public override void Action() {
            Vector2 playerPos = FindPlayerPos();
            // If player is in certain range start walking to player
            if (Position.DistanceTo(playerPos) > 72) {
                
            } else if (Position.Y != playerPos.Y) {
                Position = playerPos;
            } else {
                Shoot(playerPos);
            }

            
            //TODO Implement pathfinding, then in pathfinding make it consume stamina
            //TODO Then can have different stamina values per enemy            

            ConsumeStamina(1);
        }

        public Vector2 FindPlayerPos() {
            Player player = GetParent().GetNode<Player>("Player");
            return player.Position;
        }

        public void Shoot(Vector2 targetPos) {
            GameState gameState = GetNode<GameState>("/root/GameState");
            
            Arrow arrow = GD.Load<PackedScene>("res://Scenes/Arrow.tscn").Instantiate<Arrow>();

            arrow.Direction = Position.DirectionTo(targetPos) * gameState.TileSize;
            arrow.Position = Position;
            arrow.EntityType = Projectile.EntityTypes.Enemy;
            gameState.SpawnEntity(arrow);
        }
    }
}