using System;
using Controller;
using Godot;
using Model;

namespace Manager
{
    public partial class GameState : Node {

        public int TileSize = 16;

        public enum TurnTypes {
            Ally  = 0,
            Enemy = 1
        }

        public TurnTypes CurrentTurn;
        public Godot.Collections.Array<Entity> Entities = new();
        
        private int enemyCount = 0;
        private int enemiesDepleted = 0;

        [Signal] public delegate void EnemyTurnStartedEventHandler();

        Godot.Collections.Array<Room> rooms = new();

        public void Reset() {
            CurrentTurn = TurnTypes.Ally;
            GenerateWorld(7,3);
            SpawnEntities();

            GD.Print(Entities);
        }

        public void Die() {
            foreach (Entity e in Entities) {
                CallDeferred(nameof(e.Die));
            }

            rooms = new();
            Entities = new();
            
            GetTree().ChangeSceneToFile("res://Scenes/GameOver.tscn");
        }

        public void TurnEnded() {

            if (CurrentTurn == TurnTypes.Ally) {
                foreach (Entity e in Entities) {
                    if (e is Projectile p) {
                        if (p.EntityType == Projectile.EntityTypes.Ally) p.Action();
                    }
                }   
                CurrentTurn = TurnTypes.Enemy;
            } else {
                foreach (Entity e in Entities) {
                    if (e is Projectile p) {
                        if (p.EntityType == Projectile.EntityTypes.Enemy) p.Action();
                    }
                }
                CurrentTurn = TurnTypes.Ally;
            }

            TurnStarted();
        }
        
        public void TurnStarted() {
            if (CurrentTurn == TurnTypes.Ally) {
                foreach(Entity e in Entities) {
                    if (e is Ally ally) {
                        ally.GainStamina(ally.MaxStamina);
                    }
                }
            } else {
                int enemyCount = 0;
                foreach(Entity e in Entities) {
                    if (e is Enemy enemy) {
                        enemyCount++;
                        enemy.GainStamina(enemy.MaxStamina);
                    }
                }

                if (enemyCount == 0) {
                    TurnEnded();
                } else {
                    enemiesDepleted = 0;
                    EmitSignal(SignalName.EnemyTurnStarted);
                }
            }   
        }

        public void EnemyStaminaDepleted() {
            enemiesDepleted++;
            GD.Print("?");
            if (enemiesDepleted == enemyCount) {
                TurnEnded();
            }
        }

        public void GenerateWorld(int chunkSize, int mapSize) {
            Vector2 randomVec2() {
                return new Vector2(
                    new Random().Next(0,mapSize),
                    new Random().Next(0,mapSize)
                );
            }

            TileMapLayer entranceRoom = GD.Load<PackedScene>("res://Scenes/EntryRoom.tscn").Instantiate<TileMapLayer>();
            TileMapLayer exitRoom = GD.Load<PackedScene>("res://Scenes/ExitRoom.tscn").Instantiate<TileMapLayer>();


            var roomContainer = GetTree().Root.GetChild(1).GetNode("Rooms");
            Godot.Collections.Array<Room> openRooms = new();
            
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    var newRoom = new Room()
                    {
                        Position = new Vector2(x * chunkSize * TileSize, -(y * chunkSize * TileSize)),
                        Name = "Chunk - " + x + ", " + y 
                    };

                    roomContainer.AddChild(newRoom);
                    openRooms.Add(newRoom);
                }
            }

            Vector2 entranceChunkPos = randomVec2(); 
            Room entrance = roomContainer.GetNode<Room>("Chunk - " + entranceChunkPos.X + ", " + entranceChunkPos.Y );
            
            Vector2 exitChunkPos = randomVec2();
            while(exitChunkPos == entranceChunkPos) {
                exitChunkPos = randomVec2();
            }
            Room exit = roomContainer.GetNode<Room>("Chunk - " + exitChunkPos.X + ", " + exitChunkPos.Y);

            exit.RoomType = Room.RoomTypes.Exit;
            entrance.RoomType = Room.RoomTypes.Entrance;

            exit.AddChild(exitRoom);
            entrance.AddChild(entranceRoom);
            rooms.Add(entrance);
            rooms.Add(exit);
            openRooms.Remove(exit);
            openRooms.Remove(entrance);

            int randomRoomAmount = new Random().Next(0, openRooms.Count);
            for (int i = 0; i < randomRoomAmount; i++) {
                TileMapLayer randomRoomTM = GD.Load<PackedScene>("res://Scenes/RandomRoom.tscn").Instantiate<TileMapLayer>();
                int randomRoomIndex = new Random().Next(1, openRooms.Count);
                Room randomRoom = openRooms[randomRoomIndex];
                randomRoom.RoomType = Room.RoomTypes.Battle;
                randomRoom.Name = "ROOM!";

                randomRoom.AddChild(randomRoomTM);
                rooms.Add(randomRoom);
                openRooms.Remove(randomRoom);
            }
        }

        public void ConnectRooms(Room currentRoom, Godot.Collections.Array<Room> rooms) {
            while (
            currentRoom.NorthRoom == null ||
            currentRoom.SouthRoom == null || 
            currentRoom.EastRoom == null ||
            currentRoom.WestRoom == null) {
                Room currentRoomDoor;

                if (currentRoom.NorthRoom == null) currentRoomDoor = currentRoom.NorthRoom;
                if (currentRoom.EastRoom == null)  currentRoomDoor = currentRoom.EastRoom;
                if (currentRoom.SouthRoom == null) currentRoomDoor = currentRoom.SouthRoom;
                if (currentRoom.WestRoom == null) currentRoomDoor = currentRoom.WestRoom;


                for (int i = 0; i < rooms.Count; i++) {
                    if (
                        currentRoom.NorthRoom != rooms[i] &&
                        currentRoom.SouthRoom != rooms[i] &&
                        currentRoom.WestRoom != rooms[i] &&
                        currentRoom.EastRoom != rooms[i]) {
                        
                        if ((currentRoom.RoomType == Room.RoomTypes.Entrance && rooms[i].RoomType == Room.RoomTypes.Exit) ||
                            (currentRoom.RoomType == Room.RoomTypes.Exit && rooms[i].RoomType == Room.RoomTypes.Entrance)) break;

                            Room connectedRoomDoor;

                            if (rooms[i].NorthRoom == null) connectedRoomDoor = rooms[i].NorthRoom;
                            if (rooms[i].EastRoom == null)  connectedRoomDoor = rooms[i].EastRoom;
                            if (rooms[i].SouthRoom == null) connectedRoomDoor = rooms[i].SouthRoom;
                            if (rooms[i].WestRoom == null) connectedRoomDoor = rooms[i].WestRoom;
 
                        currentRoomDoor = rooms[i];
                        connectedRoomDoor = currentRoom;
                    }
                }

                //CREATE A WAY TO BREAK OUT OF THIS WHILE LOOP BY COMPARING
                //OLD CONNECTIONS TO NEW CONNECTIONS if same then break
            }
        }

        public void SpawnEntity(Entity entity) {
            var entityContainer = GetTree().Root.GetChild(1).GetNode("Entities");

            Entities.Add(entity);
            if (entity is Enemy) {
                EnemyTurnStarted += entity.Action;
                GD.Print("!!!");
                entity.StaminaDeplated += EnemyStaminaDepleted;
                enemyCount++;
            }

            entityContainer.AddChild(entity);
        }

        public void SpawnEntities() {
            var entityContainer = GetTree().Root.GetChild(1).GetNode("Entities");
            
            Player player = GD.Load<PackedScene>("res://Scenes/Player.tscn").Instantiate<Player>();
            Enemy testEnemy = GD.Load<PackedScene>("res://Scenes/BowSkeleton.tscn").Instantiate<Enemy>();
            
            
            Entities.Add(player);
            
            foreach (Room room in rooms) {
                if (room.RoomType == Room.RoomTypes.Entrance) {
                    player.Position = room.Position + new Vector2(3*TileSize,-3*TileSize);
                }
            }
            

            SpawnEntity(testEnemy);

            player.StaminaDeplated += TurnEnded;
            entityContainer.AddChild(player);
        }
    }
}