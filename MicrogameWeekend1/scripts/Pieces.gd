extends Node2D

class_name Pieces

enum piece_types {
	long,
	square,
	squiggly_left,
	squiggly_right
}

var square = load("res://scenes/pieces/square.tscn") as PackedScene;
var long = load("res://scenes/pieces/long.tscn") as PackedScene;
var squiggly_left = load("res://scenes/pieces/squiggly_left.tscn") as PackedScene;
var squiggly_right = load("res://scenes/pieces/squiggly_right.tscn") as PackedScene;

var fall_timer
var piece_spawner

var next_piece


func _ready() -> void:
	fall_timer = Timer.new()
	fall_timer.wait_time = get_parent().piece_fall_speed
	
	add_child(fall_timer)
	fall_timer.start()

	choose_next_piece()
	summon_piece()


func choose_next_piece():
	var piece = randi_range(0,3)
	
	match piece:
		piece_types.long:
			next_piece = long.instantiate() as Piece
			next_piece.piece_type = piece_types.long
		piece_types.square:
			next_piece = square.instantiate() as Piece
			next_piece.piece_type = piece_types.square
		piece_types.squiggly_left:
			next_piece = squiggly_left.instantiate() as Piece
			next_piece.piece_type = piece_types.squiggly_left
		piece_types.squiggly_right:
			next_piece = squiggly_right.instantiate() as Piece
			next_piece.piece_type = piece_types.squiggly_right
			
	var next_color = randi_range(0,3)
	next_piece.set_color(next_color)

	get_parent().get_node("NextPiece").update_visual(next_piece)
	


func summon_piece():
	print(get_parent().score)
	    
	add_child(next_piece)
	next_piece.position = Vector2(64,16)
	fall_timer.timeout.connect(next_piece.move_piece)
	
	next_piece = null
	choose_next_piece()
