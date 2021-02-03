using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Net;
using CT.Instance;

namespace CT.UI.Training
{
    public class TrainingOptionUI : MonoBehaviour
    {
        public Image iconImage;
        public Text costText;

        TrainingUI ui;
        UnitData unit;

        public void Init(TrainingUI ui, UnitData unit)
        {
            this.ui = ui;
            this.unit = unit;
            iconImage.sprite = unit.icon;
            costText.text = unit.trainCost.ToString();
        }

        public void Buy()
        {
            ui.TryToBuy(unit);
        }
    }
}
