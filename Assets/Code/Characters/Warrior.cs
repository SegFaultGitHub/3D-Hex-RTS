namespace Code.Characters {
    public class Warrior : Player {
        public const int GOLD_COST = 150;
        public const int WOOD_COST = 100;

        public override bool CanSummon(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= GOLD_COST && resourcesManager.Wood >= WOOD_COST;
        }
    }
}
