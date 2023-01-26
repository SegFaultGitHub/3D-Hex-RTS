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
                case Buyable.Castle:
                    this.SetText(Castle.GOLD_COST, Castle.WOOD_COST);
                    break;
                case Buyable.ArcheryRange:
                    this.SetText(ArcheryRange.GOLD_COST, ArcheryRange.WOOD_COST);
                    break;
                case Buyable.Warrior:
                    this.SetText(Warrior.GOLD_COST, Warrior.WOOD_COST);
                    break;
                case Buyable.Samurai:
                    this.SetText(Samurai.GOLD_COST, Samurai.WOOD_COST);
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
