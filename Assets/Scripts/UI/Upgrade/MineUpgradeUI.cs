using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Data.Instance;
using CT.Instance;
using CT.Helper;

namespace CT.UI.Upgrade
{
    public class MineUpgradeUI : UpgradeUI<MineInstanceData>
    {
        public Text oldCapacityText, newCapacityText;
        public Text oldPerHourText, newPerHourText;
        public Text oldFillTimeText, newFillTimeText;

        protected override void OnInit(MineInstanceData instance)
        {
            //var data = instance.data as MineData;
            var current = instance.CurrentData;

            oldCapacityText.text = current.capacity.ToString();
            oldPerHourText.text = $"{current.HourlyYield} / hour";
            oldFillTimeText.text = current.fillTime.ToDisplayText();

            if (!instance.HasNextVersion) return;
            var next = instance.NextData;

            newCapacityText.text = next.capacity.ToString();
            newPerHourText.text = $"{next.HourlyYield} / hour";
            newFillTimeText.text = next.fillTime.ToDisplayText();
        }

        [ContextMenu("Apply Basics")]
        protected override void ApplyBasics()
        {
            base.ApplyBasics();
        }
    }
}