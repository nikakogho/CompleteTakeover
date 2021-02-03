using CT.Data;
using CT.Data.Instance;
using CT.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Instance
{
    public class MainHall : Building//<MainHallData, MainHallInstanceData>
    {
        public MainHallData Data => BaseData as MainHallData;
        public MainHallInstanceData InstanceData => BaseInstanceData as MainHallInstanceData;
        public MainHallData.VersionData VersionData => InstanceData.CurrentData;

        public int Gold => InstanceData.storedGold;
        public int Elixir => InstanceData.storedElixir;

        protected override int StartGold => Gold;
        protected override int StartElixir => Elixir;

        public Dictionary<BuildingData, int> GetAllowedBuildings()
        {
            var list = new Dictionary<BuildingData, int>();

            for(int level = 1; level <= InstanceData.level; level++)
            {
                foreach (var allowed in (InstanceData.BaseGetLevelData(level) as MainHallData.VersionData).buildings)
                {
                    if (list.ContainsKey(allowed.building)) list[allowed.building] += allowed.amount;
                    else list.Add(allowed.building, allowed.amount);
                }
            }

            return list;
        }

        public void SetResources(int gold, int elixir)
        {
            InstanceData.storedGold = gold;
            InstanceData.storedElixir = elixir;
        }

        public override void OnInteract()
        {
            OnUpgradeUISelected();
        }

        public override void OnUpgradeUISelected()
        {
            UpgradeUIManager.instance.Select(this);
        }
    }
}