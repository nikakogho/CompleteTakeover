using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    public abstract class BuildingInstanceData
    {
        public int ID;
        public readonly BuildingData data;

        public int level;

        public string FullName => $"{data.name} Level {level}";
        public string NextLevelName => $"{data.name} Level {level + 1}";

        [Range(0, BaseData.Width)]  public int tileX;
        [Range(0, BaseData.Height)] public int tileY;
        public bool destroyed;

        public BuildingData.BaseVersionData BaseCurrentData => level == 1 ? data.Original : data.Upgrades[level - 2];
        public BuildingData.BaseVersionData BaseNextData => HasNextVersion ? data.Upgrades[level - 1] : null;
        public BuildingData.BaseVersionData BaseLastData => data.FinalVersion;

        public Sprite UpgradeIntoIcon => BaseNextData.icon;
        public int UpgradeCostGold => BaseNextData.buildCostGold;
        public int UpgradeCostElixir => BaseNextData.buildCostElixir;
        public int UpgradeCostGems => BaseNextData.buildCostGems;
        public GameTime UpgradeTime => BaseNextData.buildTime;

        public bool HasNextVersion => level <= data.Upgrades.Length;

        public enum BuildingType
        {
            ArmyHolder, Bunker, Lab, MainHall, Mine, Storage,
            TrainingZone, Turret, Trap, Wall, Decoration
        }

        public abstract BuildingType Type { get; }

        public BuildingInstanceData(int id, BuildingData data, int level, int tileX, int tileY, bool destroyed)
        {
            ID = id;
            this.data = data;
            this.level = level;
            this.tileX = tileX;
            this.tileY = tileY;
            this.destroyed = destroyed;
        }

        public BuildingData.BaseVersionData BaseGetLevelData(int level)
        {
            if (level == 1) return data.Original;
            return data.Upgrades[level - 2];
        }
    }
}