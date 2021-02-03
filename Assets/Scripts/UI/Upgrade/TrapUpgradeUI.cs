using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.Instance;
using UnityEngine.UI;

namespace CT.UI.Upgrade
{
    public class TrapUpgradeUI : UpgradeUI<TrapInstanceData>
    {
        public Text oldRadiusText, oldDamageText;
        public Text newRadiusText, newDamageText;

        protected override void OnInit(TrapInstanceData instance)
        {
            //var data = instance.data as TrapData;
            var current = instance.CurrentData;
            var next = instance.NextData;

            oldRadiusText.text = current.explosionRange.ToString();
            oldDamageText.text = current.explosionDamage.ToString();
            
            if (next == null) return;

            newRadiusText.text = next.explosionRange.ToString();
            newDamageText.text = next.explosionDamage.ToString();
        }

        [ContextMenu("Apply Basics")]
        protected override void ApplyBasics()
        {
            base.ApplyBasics();
        }
    }
}