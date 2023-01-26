namespace Code.Interactable {
    public class Barracks : Shop {
        public const int GOLD_COST = 200;
        public const int WOOD_COST = 200;

        public override bool CanBuild(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= GOLD_COST && resourcesManager.Wood >= WOOD_COST;
        }
    }
}
