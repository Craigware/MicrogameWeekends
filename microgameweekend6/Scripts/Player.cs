using System;
using Godot;

public partial class Player : CharacterBody3D
{
    [Export] int maxHealth;
    private int health;
    public int Health;
    [Export] float CameraRotationSpeed = 0.5f;
    [Export] float MovementSpeed = 5.0f;
    [Export] float SprintMultiplier = 2.0f;
    Vector3 facing = Vector3.Forward;

    public override void _Ready() { 
        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
    }

    public override void _Process(double delta) {
        GetNode<MeshInstance3D>("Gun").RotationDegrees = GetNode<MeshInstance3D>("Gun").RotationDegrees.MoveToward(Vector3.Zero, 500f * (float) delta);
    }

    public override void _PhysicsProcess(double delta) {
        if (GetViewport().GetMousePosition().X > DisplayServer.WindowGetSize().X - 100) {
            RotateY(-CameraRotationSpeed * (float)delta);
        } else if (GetViewport().GetMousePosition().X <= 100) {
            RotateY(CameraRotationSpeed * (float)delta);
        }
        facing.X = (float)Math.Sin(Rotation.Y);
        facing.Z = (float)Math.Cos(Rotation.Y);
        Vector3 velocity = Velocity;
        Vector2 inputVec = Input.GetVector("Right", "Left", "Backward", "Forward");
        if (inputVec != Vector2.Zero) {
            Vector3 side = new(
                (float)Math.Sin(Rotation.Y+90),
                0,
                (float)Math.Cos(Rotation.Y+90)
            );
            velocity = (side * inputVec.X + inputVec.Y * facing) * MovementSpeed;
        }
        if (inputVec == Vector2.Zero) {
            velocity = velocity.MoveToward(Vector3.Zero, MovementSpeed);
        }
        if (!IsOnFloor()) {
            velocity.Y = -10f;
        }
        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event) {
        if (Input.IsActionJustPressed("Close")) GetTree().Quit();
        if (Input.IsActionJustPressed("Shoot")) Shoot();
    }

    public void Shoot() {
        const float rayLen = 1000f;
        var gun = GetNode<MeshInstance3D>("Gun");
        gun.RotationDegrees = new(-90, 0, 0);
        var spaceState = GetWorld3D().DirectSpaceState;
        var cam = GetNode<Camera3D>("/root/Main/Player/Camera3D");
        var from = cam.ProjectRayOrigin(GetViewport().GetMousePosition());
        var to = cam.ProjectRayNormal(GetViewport().GetMousePosition()) * rayLen;
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var res = spaceState.IntersectRay(query);
        if (res.ContainsKey("collider")) {
            if ((Node)res["collider"] is Breakable b) {
                b.Health--;
            }
            if ((Node)res["collider"] is Entity e) {
                e.Health--;
            }
            Node3D collider = (Node3D)res["collider"];
            Vector3 normal = (Vector3)res["normal"];
            Decal bulletHole = new() {
                TextureAlbedo=GD.Load<Texture2D>("res://Assets/hole.png"),
            };
            Timer fadeTimer = new() {
                WaitTime=5,
                Autostart=true
            };
            fadeTimer.Timeout += () => {
                fadeTimer.GetParent().QueueFree();
                fadeTimer.QueueFree();
            };
            if (normal.X != 0) 
                bulletHole.RotationDegrees = new(0,0,90);
            if (normal.Z != 0) 
                bulletHole.RotationDegrees = new(90,0,0);
            collider.AddChild(bulletHole);
            bulletHole.AddChild(fadeTimer);
            bulletHole.GlobalPosition=(Vector3)res["position"];
            bulletHole.Scale = new(0.1f,0.1f,0.1f);
        }
    }

    public void Die() {
        GetTree().ReloadCurrentScene();
    }
}