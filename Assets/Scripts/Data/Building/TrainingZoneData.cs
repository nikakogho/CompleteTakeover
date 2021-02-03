using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Training Zone", menuName = "Faction/Building/Training Zone")]
    public class TrainingZoneData : BuildingData
    {
        public override BuildingType Type => BuildingType.TrainingZone;
        public VersionData original;
        public VersionData[] upgrades;

        public override BaseVersionData Original => original;

        public override BaseVersionData[] Upgrades => upgrades;

        [System.Serializable]
        public class VersionData : BaseVersionData
        {
            public UnitData[] canTrain;
            public List<int> slotSizes;
        }
    }
}