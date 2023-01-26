namespace Code.Characters {
    public class Samurai : Player {
        public const int GOLD_COST = 300;
        public const int WOOD_COST = 200;

        public override bool CanSummon(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= GOLD_COST && resourcesManager.Wood >= WOOD_COST;
        }
    }
}
