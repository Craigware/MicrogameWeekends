using Godot;

namespace Entity 
{
    [GlobalClass, Icon("")]
    public partial class Plant : Node2D
    {
        [Export] PlantStats Stats;
        [Export] Texture2D[] GrowthTextures;
        [Export] int GrowthStage;

        [Export] string PlantName;
        [Export] string Drop;
        
        Sprite2D sprite;

        public Plant() {}
        public Plant(
            PlantStats stats,
            Texture2D[] growthTextures,
            string plantName,
            string plantDrop
        ) {
            Stats = stats;
            GrowthTextures = growthTextures;
            GrowthStage = 0;
            PlantName = plantName;
            Drop = plantDrop;
        }

        public void CreateTimer() {
            Timer growthTimer = new() {
                WaitTime = Stats.GrowthSpeed,
                Autostart = true
            };
            growthTimer.Timeout += Grow;   
            AddChild(growthTimer);
        }

        public override void _Ready() {
            CallDeferred(nameof(CreateTimer));
 
            sprite = new(){
                Texture = GrowthTextures[0]
            };
            AddChild(sprite);

            GD.Print(PlantName, GrowthTextures.Length);
        }

        public void Grow() {
            GD.Print("GROWTH!");
 
            GrowthStage += 1;
            sprite.Texture = GrowthTextures[GrowthStage];
 
            if (GrowthStage == GrowthTextures.Length - 1) {
                GD.Print("Maxed");
                Sproutling sproutling = new(drop: Drop, texture: GrowthTextures[GrowthStage]);                                
                sproutling.Position = Position;
                GetParent().AddChild(sproutling);
                QueueFree();
                return;
            };

        }
    }
}