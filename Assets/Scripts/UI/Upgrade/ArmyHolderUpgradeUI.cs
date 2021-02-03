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
    public class ArmyHolderUpgradeUI : UpgradeUI<ArmyHolderInstanceData>
    {
        public Text oldCapacityText, newCapacityText;

        protected override void OnInit(ArmyHolderInstanceData instance)
        {
            //var data = instance.data as ArmyHoldData;
            var current = instance.CurrentData;
            var next = instance.NextData;

            oldCapacityText.text = current.capacity.ToString();
            
            if (next == null) return;

            newCapacityText.text = next.capacity.ToString();
        }

        [ContextMenu("Apply Basics")]
        protected override void ApplyBasics()
        {
            base.ApplyBasics();
        }
    }
}