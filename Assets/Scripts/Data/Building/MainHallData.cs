using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Hall", menuName = "Faction/Building/Hall")]
    public class MainHallData : BuildingData
    {
        public override BuildingType Type => BuildingType.MainHall;

        public VersionData original;
        public VersionData[] upgrades;

        public override BaseVersionData Original => original;

        public override BaseVersionData[] Upgrades => upgrades;

        [System.Serializable]
        public class VersionData : BaseVersionData
        {
            public int goldCapacity = 9000;
            public int elixirCapacity = 9000;
            public string dutiesDescription;
            public BuildsAllowed[] buildings;
        }

        [System.Serializable]
        public class BuildsAllowed
        {
            public BuildingData building;
            public int amount;
        }
    }
}