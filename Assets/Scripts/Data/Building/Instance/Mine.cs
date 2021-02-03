using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.Manager;
using CT.Net;
using CT.UI;

namespace CT.Instance
{
    public class Mine : Building
    {
        int yield;
        int capacity;
        float yieldSeconds;
        int ID => InstanceData.ID;

        public MineData Data => BaseData as MineData;
        public MineInstanceData InstanceData => BaseInstanceData as MineInstanceData;
        public MineData.VersionData VersionData => InstanceData.CurrentData;

        public bool IsGold => Data.resourceType == BuildingData.ResourceType.Gold;

        public int Stored => InstanceData.stored;
        public int Capacity => capacity;

        protected override int StartGold => Data.resourceType == BuildingData.ResourceType.Gold ? Stored : 0;
        protected override int StartElixir => Data.resourceType == BuildingData.ResourceType.Elixir ? Stored : 0;

        public GameTime FillTimeLeft => InstanceData.FillTimeLeft;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void ApplyInstanceData(Base _base, BuildingInstanceData instanceData)
        {
            base.ApplyInstanceData(_base, instanceData);
            yield = VersionData.YieldEachTime;
            capacity = VersionData.capacity;
            yieldSeconds = VersionData.yieldTime.TotalSeconds;
            StartCoroutine(YieldRoutine());
        }

        void AddToStore(int amount)
        {
            int stored = Mathf.Min(InstanceData.stored + yield, capacity);
            if (stored > Stored) UpdateStoredValue(stored);
        }

        public override void OnInteract()
        {
            Collect();
        }

        public override void OnUpgradeUISelected()
        {
            UpgradeUIManager.instance.Select(this);
        }

        public bool Collect()
        {
            if (Stored == 0) return false;

            int toAdd;

            if (IsGold)
            {
                int canAdd = Base.CanTakeGold;
                toAdd = Mathf.Min(Stored, canAdd);
            }
            else
            {
                int canAdd = Base.CanTakeElixir;
                toAdd = Mathf.Min(Stored, canAdd);
            }

            int gold = IsGold ? toAdd : 0;
            int elixir = IsGold ? 0 : toAdd;

            InstanceData.stored -= toAdd;

            SpawnCollectUI(toAdd);

            ClientSend.AddResources(Base.Data.ID, gold, elixir);
            ClientSend.UpdateMineStored(ID, Stored);

            return true;
        }

        void SpawnCollectUI(int amount)
        {
            var manager = GameManager.instance;
            var prefab = IsGold ? manager.goldCollectedPrefab : manager.elixirCollectedPrefab;
            Vector3 pos = transform.position + manager.collectedUIOffset;
            var obj = Instantiate(prefab, pos, Quaternion.identity, transform);
            var ui = obj.GetComponent<ResourcesTakenUI>();
            var icon = IsGold ? LoadManager.instance.goldIcon : Base.Data.Faction.elixirIcon;
            ui.Init(amount, icon);
            ui.MoveUp();
        }

        void UpdateStoredValue(int value)
        {
            InstanceData.stored = value;
            ClientSend.UpdateMineStored(ID, value);
        }

        IEnumerator YieldRoutine()
        {
            while (true)
            {
                if (InstanceData.destroyed) continue;
                yield return new WaitForSeconds(yieldSeconds);
                AddToStore(yield);
            }
        }
    }
}