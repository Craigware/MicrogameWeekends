using Godot;
using System;

public partial class Snake : CharacterBody2D
{
    public int SnakeLength;
    private SnakeBrain brain;
    Godot.Collections.Array<Vector2> lastInputPosition = new();
    Godot.Collections.Array<Vector2> inputDirections = new();
    Godot.Collections.Array<StaticBody2D> bodySegments = new();
    Godot.Collections.Array<StaticBody2D> waitingForSpawn = new();
    Vector2 movingDirection;

    bool wait = true;

    //TODO
    // // Snake Decide next movement
    // // Snake needs to be able to decide where he can mode nexst
    // // Snake should not be able to go direction back into himself
    // // Snake should try and avoid crashing
    // // Snake should try and catch frog

    //* Snake needs to grow in size
    // // Snake needs to restart game on collision

    public override void _Ready() {
        brain = GetNode<SnakeBrain>("/root/SnakeBrain");
        lastInputPosition.Add(Position);
        lastInputPosition.Add(new(Position.X, Position.Y));
        
        inputDirections.Add(Vector2.Zero);
        inputDirections.Add(Vector2.Zero);
    }

    public override void _Process(double delta) {
        
        
        if (Position % 16 == Vector2.Zero) {
            
            MoveBody();
            UpdatePositionList();
            Grow();

            if (!brain.Brain.ContainsKey(lastInputPosition[0])) {
                brain.Brain[lastInputPosition[0]] = new();
            }

            GenerateRandomDirection();
            var eyesight = GetNode<Area2D>("Eyesight");

            if (movingDirection == new Vector2(1,0)) {
                eyesight.RotationDegrees = -90;                
            } else if (movingDirection == new Vector2(-1,0)) {
                eyesight.RotationDegrees = 90;
            } else if (movingDirection == new Vector2(0,-1)) {
                eyesight.RotationDegrees = 180;
            } else {
                eyesight.RotationDegrees = 0;
            }
        }
      
        Move(movingDirection);
    }
 
    public void GenerateRandomDirection() {
        Vector2 direction;
        var randomAxis = new Random().Next(-1,1);
        var randomDir = new Random().Next(-1,2);

        // If no direction was generated
        if (randomDir == 0) {
            GenerateRandomDirection();
            return;
        }

        if (randomAxis == 0) {
            direction = new(randomDir, 0);
        } else {
            direction = new(0, randomDir);
        }

        // If the snakes new direction is backwards onto itself
        if (direction * -1 == movingDirection) {
            GenerateRandomDirection();
            return;
        }

        if (!brain.Brain[lastInputPosition[0]].ContainsKey(direction)) {
            brain.Brain[lastInputPosition[0]][direction] = new()
            {
                new() {
                    { "Outcome", 0 },
                    { "SnakeLength", 0 }
                }
            };
        }

        var Eyesight = GetNode<Area2D>("Eyesight");
        var seen = Eyesight.GetOverlappingBodies();

        foreach (var thing in seen) {
            if (thing is Frog frog) {
                var dirToFrog = Position.DirectionTo(frog.Position).Round();

                if (dirToFrog.X != 0) {
                    direction = new(dirToFrog.X, 0);
                } else {
                    direction = new(0, dirToFrog.Y);
                }

                if (!brain.Brain[lastInputPosition[0]].ContainsKey(direction)) {
                    brain.Brain[lastInputPosition[0]][direction] = new()
                    {
                        new() {
                            { "Outcome", 0 },
                            { "SnakeLength", 0 }
                        }
                    };
                }

                if (Position == frog.Position) {
                    QueueGrow();
                    frog.Die();
                }
            }
        }       

        int failureCount = 0;
        foreach (var outcome in brain.Brain[lastInputPosition[0]][direction]) {
            if ((Outcome) outcome["Outcome"] == Outcome.HitWall ) {
                failureCount++;
            }

            if ((Outcome) outcome["Outcome"] == Outcome.HitSelf && SnakeLength >= outcome["SnakeLength"]) {
                failureCount++;
            }
        }

        if (failureCount == 0) {
            UpdateDirList();
            movingDirection = direction;
            return; 
        } else {
            GenerateRandomDirection();
            return;
        }
   }

    public void Move(Vector2 direction) {
        var collision = MoveAndCollide(direction);
    
        if (collision != null && collision.GetCollider() is StaticBody2D s) {
            brain.Brain[lastInputPosition[0]][direction][^1]["Outcome"] = (int) Outcome.HitWall;
            brain.CurrentItteration++;
            GetTree().ReloadCurrentScene();
        }
    }
    
    public void UpdateDirList() {
        Vector2 swapDir = inputDirections[0];
        for (int i = 1; i < inputDirections.Count; i++) {
            (swapDir, inputDirections[i]) = (inputDirections[i], swapDir);
        }
        inputDirections[0] = movingDirection;
    }

    public void UpdatePositionList() {
        Vector2 swapDir = lastInputPosition[0];
        for (int i = 1; i < lastInputPosition.Count; i++) {
            (swapDir, lastInputPosition[i]) = (lastInputPosition[i], swapDir);
        }
        lastInputPosition[0] = Position;
    }

    public void MoveBody() {
        for (int i = 0; i < bodySegments.Count; i++) {
            bodySegments[i].Position = lastInputPosition[i+1];
        }
    }

    public void QueueGrow() {
        var newSegment = GD.Load<PackedScene>("res://BodySegment.tscn").Instantiate<StaticBody2D>();
        inputDirections.Add(new(0,0));
        bodySegments.Add(newSegment);
        newSegment.Position = lastInputPosition[^1];
        GD.Print(lastInputPosition[^1]);
        lastInputPosition.Add(new(0,0));
        newSegment.Name = "BODYSEGMENT";
        waitingForSpawn.Add(newSegment);
    }

    public void Grow() {
        foreach (var newSegment in waitingForSpawn) {
            if (Position.DistanceTo(newSegment.Position) > 16) {
                GetNode("/root/Main/BodySegments").AddChild(newSegment);
                waitingForSpawn.Remove(newSegment);
                SnakeLength++;
            }
        }
    }
}
