using System;
using Code.Characters;
using Code.Enums;
using Code.Interactable;
using TMPro;
using UnityEngine;

namespace Code.ResourcesManager {
    public class Cost : MonoBehaviour {
        [field: SerializeField] private Buyable Buyable;

        private void Start() {
            switch (this.Buyable) {
                case Buyable.Lumberjack:
                    this.SetText(Lumberjack.GOLD_COST, Lumberjack.WOOD_COST);
                    break;
                case Buyable.Miner:
                    this.SetText(Miner.GOLD_COST, Miner.WOOD_COST);
                    break;
                case Buyable.Builder:
                    this.SetText(Builder.GOLD_COST, Builder.WOOD_COST);
                    break;
                case Buyable.Barracks:
                    this.SetText(Barracks.GOLD_COST, Barracks.WOOD_COST);
                    break;
                default: throw new Exception($"[Cost:Start] Unexpected Buyable {this.Buyable}");
            }
        }

        private void SetText(int gold, int wood) {
            this.transform.Find("Gold cost").GetComponent<TMP_Text>().text = gold.ToString();
            this.transform.Find("Wood cost").GetComponent<TMP_Text>().text = wood.ToString();
        }
    }
}
