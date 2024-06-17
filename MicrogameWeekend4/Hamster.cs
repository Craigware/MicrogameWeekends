using Godot;
using System;

public partial class Hamster : Sprite2D
{
    public enum Consumables {
        Food,
        Monster,
        Medicine
    }

    public enum States {
        Sleeping,
        Training,
        Waiting,
        Eating,
        Dead
    }

    States State = States.Waiting;
    int TurnsTillStateChange = 0;

    public int Health = 100;
    public int Sickness = 0;
    public int Happyness = 100;
    public int Tiredness = 0;
    public int Hunger = 100;

    int InteractionsWithinSecond = 0;


    Timer StateTimer = new() { WaitTime = 1 };

    public override void _Ready() {
        StateTimer.Timeout += UpdateState;
        AddChild(StateTimer);
        StateTimer.Start();
    }

    public void UpdateHealth(int amount) { 
        Health += amount;
        if (Health <= 0) {
            Die();
            State = States.Dead;
        }
    }

    public void UpdateHunger(int amount) { Hunger += amount; }
    public void UpdateTiredness(int amount) { Tiredness += amount; }
    public void UpdateSickness(int amount) { Sickness += amount; }
    public void UpdateHappyness(int amount) { Happyness = Mathf.Clamp(Happyness + amount, 0, 100); }

    public void Eat(Consumables consumable) {
        if (State == States.Training || State == States.Sleeping) return;
        switch (consumable) {
            case Consumables.Food:
                State = States.Eating;
                UpdateHunger(20);
                TurnsTillStateChange = 2;
                break;
            case Consumables.Monster:
                State = States.Eating;
                TurnsTillStateChange = 1;
                UpdateTiredness(-20);
                UpdateHappyness(5);
                UpdateSickness(5);
                break;
            case Consumables.Medicine:
                State = States.Eating;
                UpdateHealth(5);
                UpdateTiredness(20);
                UpdateSickness(-20);
                TurnsTillStateChange = 1;
                break;
        }
    }

    public void EatMedicine() {
        Eat(Consumables.Medicine);
    }

    public void EatMonster() {
        Eat(Consumables.Monster);
    }

    public void EatFood() {
        Eat(Consumables.Food);
    }

    public void Interact() {
        InteractionsWithinSecond++;
        if (InteractionsWithinSecond > 3) {
            UpdateHappyness(-10);
        } else {
            UpdateHappyness(10);            
        }        
    }

    public void StartTraining() {
        State = States.Training;
    }

    public void Die() {
        GetTree().ChangeSceneToFile("res://GameOver.tscn");
    }

    public void UpdateState() {
        //* Sleeping related
        if (Tiredness >= 100) {
            State = States.Sleeping;
        }

        if (State == States.Sleeping) {
            UpdateTiredness(-10);
        } else {
            UpdateTiredness(5);
        }
        if (Tiredness <= 0 && State == States.Sleeping) {
            State = States.Waiting;
        }

        //* Sickness related
        if (Health != 100) {
            UpdateSickness(5);
        }

        if (Sickness >= 90) {
            UpdateHealth(-15);
            UpdateHappyness(-15);
        } else if (Sickness >= 80) {
            UpdateHealth(-10);
            UpdateHappyness(-15);
        } else if (Sickness >= 60) {
            UpdateHealth(-5);
            UpdateHappyness(-10);
        }

        //* Happyness related
        if (State == States.Eating) {
            UpdateHappyness(10);
            UpdateTiredness(5);
            if (TurnsTillStateChange == 0) {
                State = States.Waiting;
            }
        } else {
            UpdateHunger(-5);
        }

        if (State == States.Waiting) {
            UpdateHappyness(-5);
        }

        if (State == States.Training) {
            UpdateHappyness(10);
        }

        //* Hunger related
        if (Hunger <= 10) {
            UpdateHealth(-20);
        } else if (Hunger <= 20) {
            UpdateHealth(-10);
        } else if (Hunger <= 30) {
            UpdateHealth(-5);
        }

        if (Hunger >= 130) {
            UpdateHealth(-20);
        } else if (Hunger >= 120) {
            UpdateHealth(-10);
        } else if (Hunger >= 110) {
            UpdateHealth(-5);
        }


        //* Training related
        if (State == States.Training) {
            UpdateHappyness(5);
            if (TurnsTillStateChange == 0) {
                State = States.Waiting;
            }
        }
        GD.Print(State);
        if (State == States.Sleeping) {
            Texture = GD.Load<Texture2D>("res://texts/sleeping.png");
        } else if (State == States.Training) {
            Texture = GD.Load<Texture2D>("res://texts/Waiting.png");
        } else if (State == States.Eating) {
            Texture = GD.Load<Texture2D>("res://texts/eating.png");
        } else if (State == States.Waiting) {
            Texture = GD.Load<Texture2D>("res://texts/Waiting.png");
        } else if (State == States.Dead ) {
            Texture = GD.Load<Texture2D>("res://texts/dead.png");
        }

        if (TurnsTillStateChange > 0) TurnsTillStateChange--;
        InteractionsWithinSecond = 0;
    }
}
