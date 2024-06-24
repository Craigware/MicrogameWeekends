using Godot;
using System;

public partial class SoundEffects : AudioStreamPlayer2D
{
    AudioStream pickup = GD.Load<AudioStream>("res://Sounds/Menu_Sound.wav");
    AudioStream scoreUp = GD.Load<AudioStream>("res://Sounds/Pickup.wav");
    AudioStream wah = GD.Load<AudioStream>("res://Sounds/WAAAH1.wav");
    AudioStream baha = GD.Load<AudioStream>("res://Sounds/WAAAH2.wav");
    AudioStream yeoch = GD.Load<AudioStream>("res://Sounds/WAAAH3.wav");

    AudioStream music = GD.Load<AudioStream>("res://Sounds/MGW_Main.wav");

    public override void _Ready() {
        Stream = music;
        Playing = true;
    }

    public void PlaySoundEffect(string effect) {
        var newPlayer = new AudioStreamPlayer2D();
        AddChild(newPlayer);
        switch(effect) {
            case "pickup":
                newPlayer.Stream = pickup;
                newPlayer.Playing = true;
                break;
            case "scoreup":
                newPlayer.Stream = scoreUp;
                newPlayer.Playing = true;
                break;
            case "wah":
                var randomChance = new Random().Next(0,100);
                if (randomChance <= 33) {
                    newPlayer.Stream = wah;
                } else if (randomChance <= 66) {
                    newPlayer.Stream = baha;
                } else {
                    newPlayer.Stream = yeoch;
                }
                
                newPlayer.Playing = true;
                break;
            case "yeouch":
                newPlayer.Stream = yeoch;
                newPlayer.Playing = true;
                break;
        }

        newPlayer.Finished += () => { newPlayer.QueueFree(); };
    }
}
