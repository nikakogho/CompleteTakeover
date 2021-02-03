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
    public class MainHallUpgradeUI : UpgradeUI<MainHallInstanceData>
    {
        public Text oldDutiesText, newDutiesText;

        protected override void OnInit(MainHallInstanceData instance)
        {
            //var data = instance.data as MainHallData;
            var current = instance.CurrentData;
            var next = instance.NextData;

            oldDutiesText.text = current.dutiesDescription;
            
            if (next == null) return;

            newDutiesText.text = next.dutiesDescription;
        }

        [ContextMenu("Apply Basics")]
        protected override void ApplyBasics()
        {
            base.ApplyBasics();
        }
    }
}