using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Storage", menuName = "Faction/Building/Storage")]
    public class StorageData : BuildingData
    {
        public enum StoreType { Gold, Elixir }
        public StoreType storageType;
        public override BuildingType Type => BuildingType.Storage;

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