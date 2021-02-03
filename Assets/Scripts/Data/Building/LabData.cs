using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Lab", menuName = "Faction/Building/Lab")]
    public class LabData : BuildingData
    {
        public override BuildingType Type => BuildingType.Lab;

        public VersionData original;
        public VersionData[] upgrades;

        public override BaseVersionData Original => original;

        public override BaseVersionData[] Upgrades => upgrades;

        [System.Serializable]
        public class VersionData : BaseVersionData
        {
            public string unlocks;
        }
    }
}