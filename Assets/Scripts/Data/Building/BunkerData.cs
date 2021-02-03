using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Bunker", menuName = "Faction/Building/Bunker")]
    public class BunkerData : BuildingData
    {
        public override BuildingType Type => BuildingType.Bunker;

        public VersionData original;
        public VersionData[] upgrades;

        public override BaseVersionData Original => original;

        public override BaseVersionData[] Upgrades => upgrades;

        [System.Serializable]
        public class VersionData : BaseVersionData
        {
            public int capacity;
        }
    }
}