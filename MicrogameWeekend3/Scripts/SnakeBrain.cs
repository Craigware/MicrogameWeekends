using Godot;

public enum Outcome {
    Neutral,
    Success,
    Failure,
    HitWall,
    HitSelf
}

public partial class SnakeBrain : Node 
{
    public int CurrentItteration = 0;
    public Godot.Collections.Dictionary<Vector2, Godot.Collections.Dictionary<Vector2, Godot.Collections.Array<Godot.Collections.Dictionary<string, int>>>> Brain = new();
}