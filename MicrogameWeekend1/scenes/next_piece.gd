extends Sprite2D

func update_visual(piece: Piece):
	texture = piece.get_node("Sprite").texture
	if piece.piece_type == 3:
		flip_h = true
	else:
		flip_h = false
	
	modulate = piece.get_node("Sprite").modulate