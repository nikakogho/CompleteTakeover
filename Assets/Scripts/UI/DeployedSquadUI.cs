using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;

namespace CT.UI.Combat
{
    public class DeployedSquadUI : MonoBehaviour
    {
        public Text nameText;
        public Text amountText;
        public Image iconImage;

        public void Init(UnitData unit, int amount)
        {
            nameText.text = unit.name;
            iconImage.sprite = unit.icon;
            amountText.text = amount.ToString();
        }
    }
}