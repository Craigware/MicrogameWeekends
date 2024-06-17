using Godot;
using Manager;

namespace Controller
{
    public partial class Player : Ally {
        enum inventoryItems {
            Sword,
            Bow,
            Bomb
        }

        private Vector2 facing = new(0, 1);
        
        private inventoryItems currentItem = inventoryItems.Sword; 
        private Godot.Collections.Dictionary<string, int> inventory = new()
        {
            {"Sword", 1},
            {"Bow", 0},
            {"Bomb", 0}
        };

        public override void _Input(InputEvent @event) {           
            if (GetNode<GameState>("/root/GameState").CurrentTurn != GameState.TurnTypes.Ally) return;

            if (Input.GetVector("Left", "Right", "Up", "Down") != Vector2.Zero) {
                Move(new Vector2(
                    Input.GetActionStrength("Right") - Input.GetActionStrength("Left"),
                    Input.GetActionStrength("Down") - Input.GetActionStrength("Up")
                ));
            }

            if (Input.IsActionJustPressed("Action")) {
                Action();
            }
        }

        public override void Move(Vector2 direction) {
            facing = direction;    
            Position += direction * GetNode<GameState>("/root/GameState").TileSize; // 16 = tilesize, probs should put somewhere
        
            ConsumeStamina(1);
        }

        public override void Die() {
            GetNode<GameState>("/root/GameState").Die();
        }

        public override void Action() {
            switch(currentItem) {
                case inventoryItems.Sword:
                    Area2D swordCollision = new();
                    CollisionShape2D swordColShape = new() {
                        Position = new Vector2(GetNode<GameState>("/root/GameState").TileSize/2,-GetNode<GameState>("/root/GameState").TileSize/2),
                        Shape = new RectangleShape2D() {
                            Size = new Vector2(GetNode<GameState>("/root/GameState").TileSize, GetNode<GameState>("/root/GameState").TileSize)
                        }
                    };

                    swordCollision.AddChild(swordColShape);
                    
                    swordCollision.Position = facing * GetNode<GameState>("/root/GameState").TileSize;
                    AddChild(swordCollision);

                    break;
                case inventoryItems.Bow:
                    // Probably going to instantiate an arrow in the world
                    break;
                case inventoryItems.Bomb:
                    // Probably going to instantiate a bomb in the world
                    break;
            }
        }
    }
}