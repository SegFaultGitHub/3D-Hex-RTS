namespace Code.Interactable {
    public class Castle : Shop {
        public const int GOLD_COST = 500;
        public const int WOOD_COST = 800;

        public override bool CanBuild(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= GOLD_COST && resourcesManager.Wood >= WOOD_COST;
        }
    }
}
