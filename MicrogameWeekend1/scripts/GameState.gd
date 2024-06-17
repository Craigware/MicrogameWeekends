extends Node2D

var score = 0
var stage = 0

var piece_fall_speed = 1
var piece_spawn_rate = 5
var piece_controller

func _ready() -> void:
    piece_controller = get_node("Pieces")

func speed_fall():
    piece_controller.fall_timer.wait_time /= 10

func normal_fall():
    piece_controller.fall_timer.wait_time = piece_fall_speed


