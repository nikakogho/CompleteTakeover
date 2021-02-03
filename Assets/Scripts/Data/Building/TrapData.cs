using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Trap", menuName = "Faction/Building/Trap")]
    public class TrapData : BuildingData
    {
        public override BuildingType Type => BuildingType.Trap;
        public VersionData original;
        public VersionData[] upgrades;

        public override BaseVersionData Original => original;

        public override BaseVersionData[] Upgrades => upgrades;

        [System.Serializable]
        public class VersionData : BaseVersionData
        {
            public float damage;
            public float explosionRange;
            public float explosionDamage;
        }
    }
}
