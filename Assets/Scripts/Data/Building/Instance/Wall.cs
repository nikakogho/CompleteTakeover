using CT.Data;
using CT.Data.Instance;
using CT.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Instance
{
    public class Wall : Building//<WallData, WallInstanceData>
    {
        public WallData Data => BaseData as WallData;
        public WallInstanceData InstanceData => BaseInstanceData as WallInstanceData;

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