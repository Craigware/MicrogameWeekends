extends CharacterBody2D

@export var SPEED = 100.0
var bomb_scene = load("res://scenes/bomb.tscn") as PackedScene

var inventory = []

func _process(delta: float) -> void:
	var direction := Input.get_axis("Left", "Right")
	if direction:
		velocity.x = direction * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)

	move_and_slide()

func _input(event: InputEvent) -> void:
	if Input.is_action_just_pressed("SpeedUp"):
		get_parent().speed_fall()
	if Input.is_action_just_released("SpeedUp"):
		get_parent().normal_fall()
	if Input.is_action_just_pressed("Bomb"):
		place_bomb()

func place_bomb():
	var bomb = bomb_scene.instantiate() as Bomb
	var color = randi_range(0,3)
	bomb.set_color(color)
	get_parent().add_child(bomb)
	bomb.position = Vector2(position.x, position.y - 8)
	pass
