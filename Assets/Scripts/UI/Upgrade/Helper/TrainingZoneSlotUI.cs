using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CT.UI.Upgrade.Helper
{
    public class TrainingZoneSlotUI : MonoBehaviour
    {
        public Text amountText;

        public void Init(int amount)
        {
            amountText.text = $"x{amount}";
        }
    }
}
