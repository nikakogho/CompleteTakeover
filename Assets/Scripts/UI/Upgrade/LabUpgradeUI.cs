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
    public class LabUpgradeUI : UpgradeUI<LabInstanceData>
    {
        public Text oldUnlocksText, newUnlocksText;

        protected override void OnInit(LabInstanceData instance)
        {
            //var data = instance.data as LabData;
            var current = instance.CurrentData;
            var next = instance.NextData;

            oldUnlocksText.text = current.unlocks;
            
            if (next == null) return;

            newUnlocksText.text = next.unlocks;
        }

        [ContextMenu("Apply Basics")]
        protected override void ApplyBasics()
        {
            base.ApplyBasics();
        }
    }
}