using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.Instance;
using UnityEngine.UI;

namespace CT.UI.Upgrade
{
    public class TurretUpgradeUI : UpgradeUI<TurretInstanceData>
    {
        public Text oldRangeText, newRangeText;
        public Text oldFireRateText, newFireRateText;
        public Text oldDamageText, newDamageText;
        public Text oldUnitNameText, newUnitNameText;
        public Image oldUnitImage, newUnitImage;
        public Text oldUnitAmountText, newUnitAmountText;
        public Text oldSpawnDeltaText, newSpawnDeltaText;
        public GameObject unitLauncherOldUI, unitLauncherNewUI, turretOldUI, turretNewUI;

        void OnDisable()
        {
            unitLauncherOldUI.SetActive(false);
            unitLauncherNewUI.SetActive(false);
            turretOldUI.SetActive(false);
            turretNewUI.SetActive(false);
        }

        protected override void OnInit(TurretInstanceData instance)
        {
            //var data = instance.data as DefensiveData;
            var current = instance.CurrentData;
            
            oldRangeText.text = current.range.ToString();

            if(current.defenseType == DefensiveData.DefenseType.Turret)
            {
                turretOldUI.SetActive(true);
                oldFireRateText.text = current.fireRate.ToString();
                oldDamageText.text = current.Damage.ToString();
            }
            else
            {
                unitLauncherOldUI.SetActive(true);
                oldUnitNameText.text = current.unit.name;
                oldUnitImage.sprite = current.unit.icon;
                oldUnitAmountText.text = current.amount.ToString();
                oldSpawnDeltaText.text = current.spawnDelta.ToString();
            }

            var next = instance.NextData;
            if (next == null) return;

            newRangeText.text = next.range.ToString();

            if (next.defenseType == DefensiveData.DefenseType.Turret)
            {
                turretOldUI.SetActive(true);
                oldFireRateText.text = next.fireRate.ToString();
                oldDamageText.text = next.Damage.ToString();
            }
            else
            {
                unitLauncherOldUI.SetActive(true);
                oldUnitNameText.text = next.unit.name;
                newUnitImage.sprite = next.unit.icon;
                oldUnitAmountText.text = next.amount.ToString();
                oldSpawnDeltaText.text = next.spawnDelta.ToString();
            }
        }

        [ContextMenu("Apply Basics")]
        protected override void ApplyBasics()
        {
            base.ApplyBasics();
        }
    }
}