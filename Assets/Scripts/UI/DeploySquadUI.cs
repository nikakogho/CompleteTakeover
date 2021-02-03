using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Manager.Combat;

namespace CT.UI
{
    public class DeploySquadUI : MonoBehaviour
    {
        public Text nameText;
        public Text amountText;
        public Image iconImage;
        public Button selectButton;

        public UnitData Unit { get; private set; }
        int amount;
        PlayerCombatManager manager;

        public void Init(UnitData unit, int amount)
        {
            Unit = unit;
            this.amount = amount;
            nameText.text = unit.name;
            amountText.text = amount.ToString();
            iconImage.sprite = unit.icon;
            manager = PlayerCombatManager.Instance;
        }

        public void Deploy()
        {
            amount--;
            amountText.text = amount.ToString();
            if (amount == 0)
            {
                amountText.enabled = false;
                selectButton.interactable = false;
                UnSelect();
            }
        }

        public void Select()
        {
            manager.Select(this);
        }

        void UnSelect()
        {
            manager.UnSelect();
        }
    }
}