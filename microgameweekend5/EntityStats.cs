using Godot;

namespace Entity 
{
    [GlobalClass]
    public partial class EntityStats : Resource 
    {
        [Export] public int Health;
        [Export] public int Age;
        [Export] public int Hunger;
        [Export] public int MaxHunger;
    }
}