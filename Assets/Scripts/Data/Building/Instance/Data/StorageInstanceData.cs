using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Instance
{
    public class StorageInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.Storage;

        public int stored;

        static int StartStored => 0;

        public StorageData Data => data as StorageData;
        public StorageData.VersionData CurrentData => BaseCurrentData as StorageData.VersionData;
        public StorageData.VersionData NextData => BaseNextData as StorageData.VersionData;
        public StorageData.VersionData LastData => BaseLastData as StorageData.VersionData;

        public StorageInstanceData(int id, StorageData data, int level, int storedAmount, int tileX, int tileY, bool destroyed)
        : base(id, data, level, tileX, tileY, destroyed)
        {
            if (data.Type != BuildingData.BuildingType.Storage)
                throw new System.ArgumentException("Building Data must be Storage!");
            this.stored = storedAmount;
        }

        public StorageInstanceData(StorageData data, int level, int tileX, int tileY)
        : this(0, data, level, StartStored, tileX, tileY, false)
        {

        }
    }
}