using Code.Characters;
using Code.Interactable;
using TMPro;
using UnityEngine;

namespace Code.ResourcesManager {
    public class Cost : MonoBehaviour {
        public void SetText(Player player) {
            this.SetText(player.GoldCost, player.WoodCost);
        }

        public void SetText(Building building) {
            this.SetText(building.GoldCost, building.WoodCost);
        }

        private void SetText(int gold, int wood) {
            this.transform.Find("Gold cost").GetComponent<TMP_Text>().text = gold.ToString();
            this.transform.Find("Wood cost").GetComponent<TMP_Text>().text = wood.ToString();
        }
    }
}
