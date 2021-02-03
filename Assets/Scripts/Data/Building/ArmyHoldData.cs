using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Army Holder", menuName = "Faction/Building/Army Holder")]
    public class ArmyHoldData : BuildingData
    {
        public override BuildingType Type => BuildingType.ArmyHolder;

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