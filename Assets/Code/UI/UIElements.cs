using System;
using System.Collections.Generic;
using Code.Characters;
using Code.Interactable;
using UnityEngine;
using Building = Code.Enums.Building;
using Character = Code.Enums.Character;

namespace Code.UI {
    public class UIElements : MonoBehaviour {
        [field: SerializeField] private List<CharacterUIElement> CharacterUIElements;
        [field: SerializeField] private List<BuildingUIElement> BuildingUIElements;

        public CharacterUIElement GetUIElements(Character character) {
            return this.CharacterUIElements.Find(e => e.Character == character);
        }
        public BuildingUIElement GetUIElements(Building building) {
            return this.BuildingUIElements.Find(e => e.Building == building);
        }

        [Serializable]
        public struct CharacterUIElement {
            public Character Character;
            public Player Prefab;
            public UIElement UIElement;
        }
        [Serializable]
        public struct BuildingUIElement {
            public Building Building;
            public PhantomBuilding PhantomPrefab;
            public UIElement UIElement;
        }
    }
}
