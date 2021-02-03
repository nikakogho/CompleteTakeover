using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    public abstract class BuildingData : ScriptableObject
    {
        new public string name;
        //public Sprite icon;
        public Faction.Type faction;
        public Faction Faction => Faction.GetFaction(faction);
        //public int health;
        public enum BuildingType
        {
            ArmyHolder, Bunker, Lab, MainHall, Mine, Storage,
            TrainingZone, Turret, Wall, Trap, Decoration
        }
        public enum ResourceType { Gold, Elixir, Gems }
        public abstract BuildingType Type { get; }
        [Range(1, 20)] public int tileWidth, tileHeight;

        public abstract BaseVersionData Original { get; }
        public abstract BaseVersionData[] Upgrades { get; }

        public BaseVersionData FinalVersion => Upgrades.Length == 0 ? Original : Upgrades[Upgrades.Length - 1];

        public BaseVersionData GetLevelData(int level)
        {
            if (level == 1) return Original;
            return Upgrades[level - 2];
        }

        /*
        public BuildingData upgradesInto;
        public int UpgradeCostGold => upgradesInto.buildCostGold;
        public int UpgradeCostElixir => upgradesInto.buildCostElixir;
        public int UpgradeCostGems => upgradesInto.buildCostGems;
        public GameTime UpgradeTime => upgradesInto.buildTime;
        public Sprite UpgradeIntoIcon => upgradesInto.icon;
        */

        public abstract class BaseVersionData
        {
            public int hallLevelNeeded = 1;
            public int health;
            public Sprite icon;
            public int buildCostGold;
            public int buildCostElixir;
            public int buildCostGems;
            public GameTime buildTime;
            public GameTime repairTime;
            public GameObject ghostPrefab;
            public GameObject prefab;
            public GameObject constructionPrefab;
            public AudioClip destroySound;
            public AudioClip repairSound;
        }
    }
}