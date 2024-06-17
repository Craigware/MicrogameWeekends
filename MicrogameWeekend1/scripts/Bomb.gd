extends Node2D

class_name Bomb

var color: Piece.piece_colors
var active : bool = false

func set_color(_color):
	var sprite = get_node("Sprite") as Sprite2D
	color = _color
	match _color:
		Piece.piece_colors.blue:
			sprite.modulate = Color.NAVY_BLUE
		Piece.piece_colors.red:
			sprite.modulate = Color.RED
		Piece.piece_colors.yellow:
			sprite.modulate = Color.YELLOW
		Piece.piece_colors.orange:
			sprite.modulate = Color.ORANGE

func explode():
	var raycast = get_node("RayCast2D") as RayCast2D
	var col = raycast.get_collider() as Area2D
	
	if col != null:
		if color == col.get_parent().color:
			col.get_parent().explode()
	
	queue_free()
