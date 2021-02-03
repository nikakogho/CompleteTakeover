using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.Instance;

namespace CT.UI.Upgrade
{
    public class WallUpgradeUI : UpgradeUI<WallInstanceData>
    {
        protected override void OnInit(WallInstanceData instance)
        {
            //nothing I guess
        }

        [ContextMenu("Apply Basics")]
        protected override void ApplyBasics()
        {
            base.ApplyBasics();
        }
    }
}