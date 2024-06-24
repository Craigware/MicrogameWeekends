using Godot;

namespace Entity 
{
    [GlobalClass]
    public partial class PlantStats : Resource 
    {
        [Export] public int MaxGrowthStages;
        [Export] public int HungerRestore;
        [Export] public int GrowthSpeed;
    }
}