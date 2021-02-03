using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    public class ConstructionData
    {
        public readonly int ID;
        public GameTime time;
        public readonly GameTime startTime;
        public readonly BuildingInstanceData isUpgradeOf;
        public readonly BuildingData toConstruct;
        public readonly int x, y;
        public readonly int endX, endY;
        public readonly GameObject fx;
        public readonly Quaternion rotation;

        //New Building
        public ConstructionData(int id, BuildingData data, GameTime time, int x, int y)
        {
            ID = id;
            startTime = data.Original.buildTime;
            this.time = time;
            isUpgradeOf = null;
            toConstruct = data;
            this.x = x;
            this.y = y;
            endX = x + data.tileWidth - 1;
            endY = y + data.tileHeight - 1;
            fx = data.Original.constructionPrefab;
            rotation = Quaternion.identity;
        }

        //Upgrade Building
        public ConstructionData(BuildingInstanceData toUpgrade, GameTime time)
        {
            if (!toUpgrade.HasNextVersion)
                throw new System.ArgumentException($"Building with ID {toUpgrade.ID} doesn't have next version!");
            ID = 0; // may do change up
            var nextLevel = toUpgrade.BaseNextData;
            startTime = nextLevel.buildTime;
            this.time = time;
            toConstruct = null;
            isUpgradeOf = toUpgrade;
            x = toUpgrade.tileX;
            y = toUpgrade.tileY;
            fx = nextLevel.constructionPrefab;
            rotation = Quaternion.identity;
        }
    }
}