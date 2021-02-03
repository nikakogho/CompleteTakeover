using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;

namespace CT.UI.Army
{
    public class ArmySlotUI : MonoBehaviour
    {
        public Image iconImage;
        public Text amountText;

        UnitData unit;
        int amount;
        Base _base;
        ArmyUI ui;

        public void Init(ArmyUI ui, UnitData unit, int amount)
        {
            this.ui = ui;
            this.unit = unit;
            this.amount = amount;
            _base = Base.active;
            iconImage.sprite = unit.icon;
            amountText.text = amount.ToString();
        }

        public void RemoveOne()
        {
            _base.RemoveUnit(unit);
            amount--;
            amountText.text = amount.ToString();

            if (amount == 0) ui.RemoveSlot(this);

            ui.UpdateStored();
        }
    }
}