using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    public class DecorationData : BuildingData
    {
        public override BuildingType Type => BuildingType.Decoration;

        public VersionData original;
        public VersionData[] upgrades;

        public override BaseVersionData Original => original;

        public override BaseVersionData[] Upgrades => upgrades;

        [System.Serializable]
        public class VersionData : BaseVersionData
        {
            public string description;
        }
    }
}