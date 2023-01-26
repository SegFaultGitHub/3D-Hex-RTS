namespace Code.Interactable {
    public class ArcheryRange : Building {
        public const int GOLD_COST = 150;
        public const int WOOD_COST = 230;

        public override void Interact(Selectable selected) { }

        public override bool CanBuild(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= GOLD_COST && resourcesManager.Wood >= WOOD_COST;
        }
    }
}
