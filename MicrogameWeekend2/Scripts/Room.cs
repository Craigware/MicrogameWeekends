using Godot;

namespace Controller
{
    public partial class Room : Node2D
    {
        public enum RoomTypes {
            Empty    = 0,
            Entrance = 1,
            Battle   = 2, 
            Exit     = 3
        }

        [Export] private Vector2 RoomSize;
        [Export] public RoomTypes RoomType;
        [Export] public Vector2 EntranceLocation;

        [Export] public Godot.Collections.Array<Vector2> DoorLocations;
        [Export] public Godot.Collections.Array<Vector2> EnemySpawnLocations;

        public Room NorthRoom = null;
        public Room SouthRoom = null;
        public Room EastRoom = null;
        public Room WestRoom = null;
    }
}