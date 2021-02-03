using CT.Data;
using CT.Data.Instance;
using CT.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Instance
{
    public class Storage : Building//<StorageData, StorageInstanceData>
    {
        public StorageData Data => BaseData as StorageData;
        public StorageInstanceData InstanceData => BaseInstanceData as StorageInstanceData;
        public StorageData.VersionData VersionData => InstanceData.CurrentData;

        public int Stored => InstanceData.stored;
        public bool IsGold => Data.storageType == StorageData.StoreType.Gold;

        protected override int StartGold => Data.storageType == StorageData.StoreType.Gold ? Stored : 0;
        protected override int StartElixir => Data.storageType == StorageData.StoreType.Elixir ? Stored : 0;

        public override void OnInteract()
        {
            var mines = IsGold ? Base.GoldMines : Base.ElixirMines;
            foreach (var mine in mines) mine.Collect();
        }

        public override void OnUpgradeUISelected()
        {
            UpgradeUIManager.instance.Select(this);
        }
    }
}