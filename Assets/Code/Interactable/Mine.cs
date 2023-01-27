using Code.Extensions;

namespace Code.Interactable {
    public class Mine : Building {
        public override string Name => "Mine";
        public override int GoldCost => -1;
        public override int WoodCost => -1;

        protected override void Awake() {
            base.Awake();
            this.OnEndOfFrame(() => this.SetCompleted(false));
        }

        public override bool CanBuild(ResourcesManager.ResourcesManager resourcesManager) {
            return false;
        }
    }
}
