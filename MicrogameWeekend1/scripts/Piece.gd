extends Node2D 
class_name Piece

enum piece_colors {
	blue,
	red,
	yellow,
	orange
}

var on_floor: bool = false
var landed_once: bool = false
var exploded = false
var color: piece_colors
var piece_type

func _ready() -> void:
	pass

func set_color(_color):
	var sprite = get_node("Sprite") as Sprite2D
	color = _color
	match _color:
		piece_colors.blue:
			sprite.modulate = Color.NAVY_BLUE
		piece_colors.red:
			sprite.modulate = Color.RED
		piece_colors.yellow:
			sprite.modulate = Color.YELLOW
		piece_colors.orange:
			sprite.modulate = Color.ORANGE

func _physics_process(delta: float) -> void:
	if on_floor: return
	if landed_once: return
	var player_pos_x = get_node("/root/Main/Player").position.x as int;
	var col = get_node("CollisionCheck") as Area2D
	var cornercol = get_node("CornerCollision") as Area2D

	var areacol = col.get_overlapping_areas()
	areacol.append_array(cornercol.get_overlapping_areas())

	var bodycol = col.get_overlapping_bodies()
	bodycol.append_array(cornercol.get_overlapping_bodies())

	if position.x != player_pos_x - (player_pos_x % 8):
		if position.x < player_pos_x:
			for area in areacol:
				if area.global_position.x > position.x:
					return
			for body in bodycol:
				if body.position.x > position.x:
					return
			position.x += 8

		if position.x > player_pos_x:
			for area in areacol:
				if area.global_position.x < position.x:
					return
			for body in bodycol:
				if body.position.x < position.x:
					return
			position.x -= 8

func explode(exploded_count = 1):
	if exploded: return
	get_parent().get_parent().score += 100 * exploded_count
	exploded = true
	
	var col = get_node("CollisionCheck") as Area2D

	if col.get_overlapping_areas():
		for area in col.get_overlapping_areas():
			if abs(area.global_position.x - position.x) > 8 && (area.global_position.y < position.y || area.global_position.y > position.y):
				pass
			elif area.get_parent().color == color:
				area.get_parent().explode()
	
	queue_free()

func landed():
	if !landed_once:
		get_parent().summon_piece()
	on_floor = true
	landed_once = true

func move_piece():
	var col = get_node("CollisionCheck") as Area2D

	for area in col.get_overlapping_areas():
		print("whatda")
		if area.global_position.y >= position.y:
			if !on_floor:
				landed()
			return

	for body in col.get_overlapping_bodies():
		if body.global_position.y > 150 || position.y >= 150:
			if !on_floor:
				landed()
			print(body.global_position)
			return

	
	on_floor = false
	position.y += 8;
	pass