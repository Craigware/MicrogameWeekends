using Godot;
using Model;

namespace Controller
{
    public partial class Enemy : Entity
    {
        public enum Weapons {
            Sword,
            Axe,
            Bow,
            Bomb
        }

        public Weapons Wepaon;
        public new int MaxStamina = 1;

        public void PathfindTo(Vector2 position) {
            
        }
    }
}