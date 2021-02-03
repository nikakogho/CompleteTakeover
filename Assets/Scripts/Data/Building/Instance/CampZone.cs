using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.UI;
using CT.UI.Army;

namespace CT.Instance
{
    public class CampZone : Building//<ArmyHoldData, ArmyHolderInstanceData>
    {
        public ArmyHoldData Data => BaseData as ArmyHoldData;
        public ArmyHolderInstanceData InstanceData => BaseInstanceData as ArmyHolderInstanceData;
        public ArmyHoldData.VersionData VersionData => InstanceData.CurrentData;

        public int Capacity => BaseData == null ? 0 : VersionData.capacity;
        public Transform[] walkPoints;
        public Transform armyParent;
        public int Filled
        {
            get
            {
                int count = 0;
                foreach (var unit in units) count += unit.Key.capacity * unit.Value;
                return count;
            }
        }
        public int FreeRoom => Capacity - Filled;
        public Dictionary<UnitData, int> units;

        protected override void Awake()
        {
            base.Awake();
            if (units == null) units = new Dictionary<UnitData, int>();
        }

        public Transform AddUnit(UnitData unit)
        {
            if (units == null) units = new Dictionary<UnitData, int>();
            if (units.ContainsKey(unit)) units[unit]++;
            else units[unit] = 1;
            int index = Random.Range(0, walkPoints.Length);
            var obj = Instantiate(unit.campWalkerPrefab, walkPoints[index].position, walkPoints[index].rotation);
            var agent = obj.GetComponent<CampMoveAgent>();
            int next = (index + 1) % walkPoints.Length;
            agent.Init(unit.moveSpeed, walkPoints, next);

            return obj.transform;
        }

        public override void OnInteract()
        {
            ArmyUI.instance.Init();
        }

        public override void OnUpgradeUISelected()
        {
            UpgradeUIManager.instance.Select(this);
        }
    }
}