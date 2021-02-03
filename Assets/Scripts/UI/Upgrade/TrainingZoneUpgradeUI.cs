using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Data.Instance;
using CT.Instance;
using CT.UI.Upgrade.Helper;

namespace CT.UI.Upgrade
{
    public class TrainingZoneUpgradeUI : UpgradeUI<TrainingZoneInstanceData>
    {
        public Transform oldSlotsParent, newSlotsParent;
        public GameObject slotPrefab;
        List<GameObject> oldList, newList;

        void Awake()
        {
            oldList = new List<GameObject>();
            newList = new List<GameObject>();
        }

        void OnDisable()
        {
            if (oldList != null) foreach (var obj in oldList) Destroy(obj);
            if (newList != null) foreach (var obj in newList) Destroy(obj);
            oldList.Clear();
            newList.Clear();
        }

        protected override void OnInit(TrainingZoneInstanceData instance)
        {
            //var data = instance.data as TrainingZoneData;
            var current = instance.CurrentData;
            var next = instance.NextData;

            InitSlots(oldSlotsParent, current, oldList);
            
            if (next == null) return;

            InitSlots(newSlotsParent, next, newList);
        }

        void InitSlots(Transform parent, TrainingZoneData.VersionData data, List<GameObject> l)
        {
            var list = data.slotSizes;
            foreach(int size in list)
            {
                var obj = Instantiate(slotPrefab, parent);
                var ui = obj.GetComponent<TrainingZoneSlotUI>();
                ui.Init(size);
                l.Add(obj);
            }
        }

        [ContextMenu("Apply Basics")]
        protected override void ApplyBasics()
        {
            base.ApplyBasics();
        }
    }
}