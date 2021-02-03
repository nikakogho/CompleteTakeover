using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Mine", menuName = "Faction/Building/Mine")]
    public class MineData : BuildingData
    {
        public override BuildingType Type => BuildingType.Mine;
        public ResourceType resourceType;

        public VersionData original;
        public VersionData[] upgrades;

        public override BaseVersionData Original => original;

        public override BaseVersionData[] Upgrades => upgrades;

        [System.Serializable]
        public class VersionData : BaseVersionData
        {
            public GameTime yieldTime;
            public int capacity;
            public GameTime fillTime;

            public int YieldEachTime => (int)(capacity * (yieldTime.TotalSeconds / fillTime.TotalSeconds));
            public int HourlyYield => YieldEachTime * (GameTime.Hour / yieldTime);
        }
    }
}