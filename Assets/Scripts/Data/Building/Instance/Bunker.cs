using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.UI;

namespace CT.Instance
{
    public class Bunker : Building//<BunkerData, BunkerInstanceData>
    {
        public BunkerData Data => BaseData as BunkerData;
        public BunkerInstanceData InstanceData => BaseInstanceData as BunkerInstanceData;
        public BunkerData.VersionData VersionData => InstanceData.CurrentData;

        public override void OnInteract()
        {
            //open ui
        }

        public override void OnUpgradeUISelected()
        {
            UpgradeUIManager.instance.Select(this);
        }
    }
}
