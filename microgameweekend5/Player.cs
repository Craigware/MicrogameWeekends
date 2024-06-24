using Entity;
using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 150.0f;
	public const float JumpVelocity = -100.0f;
 
    [Export] public float MaxJumpMultiplier = 4f;
    float jumpForce = 1f;

    [Export] public float MaxCoyoteTime = 1f;
    float coyoteTime = 0f;

    int facing;

    bool IsStunned = false;
    float StunTimer = 1f;

    Entity.Entity HeldEntity;
    Sproutling HeldPlant;

	public override void _PhysicsProcess(double delta) {
		if (Position.Y > 500) {
            Die();
        }

        if (IsStunned) {
            StunTimer -= 1f * (float)delta;
            if (StunTimer <= 0f) {
                IsStunned = false;
                StunTimer = 1f;
            }
        }

        Vector2 velocity = Velocity;
        if (GetLastSlideCollision() != null && GetLastSlideCollision().GetCollider() is Entity.Entity e) {
            IsStunned = true;
            GetNode<SoundEffects>("/root/SoundEffects").PlaySoundEffect("wah");
            velocity.X = Position.DirectionTo(e.Position).X * -1 * Speed;
            velocity.Y -= 200f;
        }

        if (Input.IsActionPressed("Jump") || Input.IsActionPressed("Up")) {
            jumpForce = Math.Clamp(jumpForce + 0.1f, 1f, MaxJumpMultiplier);
        }

		if (!IsOnFloor()) {
			velocity += GetGravity() * (float)delta;
            coyoteTime = Math.Clamp(coyoteTime - 0.1f, 0f, MaxCoyoteTime);
        } else {
            coyoteTime = MaxCoyoteTime;
        }

		if ((Input.IsActionJustReleased("Jump") || Input.IsActionJustReleased("Up") ) && (IsOnFloor() || coyoteTime != 0f)) {
			velocity.Y = JumpVelocity * jumpForce;
            jumpForce = 1f;
		}

		float direction = Input.GetAxis("Left", "Right");
		if (direction != 0 && !IsStunned) {
			velocity.X = direction * Speed;
            facing = (int) direction;
            if (direction < 0) {
                GetNode<Sprite2D>("Sprite").FlipH = true;
            } else GetNode<Sprite2D>("Sprite").FlipH = false;
		} else if (!IsStunned){
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
        if (HeldEntity != null) {
            HeldEntity.Position = Position + new Vector2(0,-16);
        }

        if (HeldPlant != null) {
            HeldPlant.Position = Position + new Vector2(0,-16);
        } 
	}

    public override void _Input(InputEvent @event) {
        if (Input.IsActionJustPressed("Plant")) Plant();
        if (Input.IsActionJustPressed("Pickup")) {
            if (HeldEntity != null || HeldPlant != null) {
                Drop();
            } else {
                PickUp();
            }
        }    
    }

    private void Plant() { 
        if (!IsOnFloor()) return;
        ShapeCast2D shapeCast = new() {
            Shape = new CircleShape2D() {
                Radius = 20
            },
            TargetPosition = Vector2.Zero,
            CollideWithAreas = true,
            CollisionMask = 1
        };

        AddChild(shapeCast);
        shapeCast.ForceShapecastUpdate();

        if (shapeCast.GetCollisionCount() != 0) {            
            for (int i = 0; i < shapeCast.GetCollisionCount(); i++) {
                if (shapeCast.GetCollider(i) is Area2D) {
                    return;
                }
            }
        }

        RemoveChild(shapeCast);

        GetNode<SoundEffects>("/root/SoundEffects").PlaySoundEffect("pickup");
        Plant planted = GD.Load<PackedScene>("res://Strawberry.tscn").Instantiate<Plant>();
        planted.Position = Position;
        GetParent().AddChild(planted);
    }

    private void PickUp() {
        var raycast = GetNode<RayCast2D>("RayCast2D");
        if (facing == 1) {
            raycast.TargetPosition = new Vector2(16, 0);
        } else {
            raycast.TargetPosition = new Vector2(-16, 0);
        }

        if (raycast.GetCollider() is Entity.Entity e) {
            GetNode<SoundEffects>("/root/SoundEffects").PlaySoundEffect("pickup");
            e.IsPickedUp = true;
            HeldEntity = e;
            e.CollisionLayer = 2;
            e.CollisionMask = 2;
        }

        if (raycast.GetCollider() is Sproutling s) {
            GetNode<SoundEffects>("/root/SoundEffects").PlaySoundEffect("pickup");
            s.IsPickedUp = true;
            HeldPlant = s;
            s.CollisionLayer = 2;
            s.CollisionMask = 2;
        }
    }

    private void Drop() {
        GetNode<SoundEffects>("/root/SoundEffects").PlaySoundEffect("pickup");
        var raycast = GetNode<RayCast2D>("RayCast2D");
        if (facing == 1) {
            raycast.TargetPosition = new Vector2(16, 0);
        } else {
            raycast.TargetPosition = new Vector2(-16, 0);
        }
 
        if (HeldEntity != null) {
            HeldEntity.Position = Position + GetNode<RayCast2D>("RayCast2D").TargetPosition + Vector2.Up;
            HeldEntity.CollisionLayer = 1;
            HeldEntity.CollisionMask = 1;
            HeldEntity.IsPickedUp = false;
            HeldEntity = null;
        }

        if (HeldPlant != null) {
            HeldPlant.Position = Position + GetNode<RayCast2D>("RayCast2D").TargetPosition + Vector2.Up;
            HeldPlant.CollisionLayer = 1;
            HeldPlant.CollisionMask = 1;
            HeldPlant.IsPickedUp = false;
            HeldPlant = null;
        }
    }

    public void Die() {
        GetNode<SoundEffects>("/root/SoundEffects").PlaySoundEffect("yeouch");
        var gameOver = GD.Load<PackedScene>("res://GameOver.tscn").Instantiate();
        GetParent().GetNode("UI").AddChild(gameOver);
        GetParent().GetNode<Camera>("Camera").target = null;
        QueueFree();
    }
}
