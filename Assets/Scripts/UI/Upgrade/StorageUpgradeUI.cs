using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.Instance;
using UnityEngine.UI;

namespace CT.UI.Upgrade
{
    public class StorageUpgradeUI : UpgradeUI<StorageInstanceData>
    {
        public Text oldStorageText, newStorageText;

        protected override void OnInit(StorageInstanceData instance)
        {
            //var data = instance.data as StorageData;
            var current = instance.CurrentData;
            var next = instance.NextData;

            oldStorageText.text = current.capacity.ToString();

            if (next == null) return;
            
            newStorageText.text = next.capacity.ToString();
        }

        [ContextMenu("Apply Basics")]
        protected override void ApplyBasics()
        {
            base.ApplyBasics();
        }
    }
}