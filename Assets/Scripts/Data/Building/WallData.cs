using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Wall", menuName = "Faction/Building/Wall")]
    public class WallData : BuildingData
    {
        public override BuildingType Type => BuildingType.Wall;

        public VersionData original;
        public VersionData[] upgrades;

        public override BaseVersionData Original => original;

        public override BaseVersionData[] Upgrades => upgrades;

        [System.Serializable]
        public class VersionData : BaseVersionData
        {

        }
    }
}
