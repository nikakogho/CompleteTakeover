using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CT.UI.Cheating
{
    public class CheatAddUnitUI : MonoBehaviour
    {
        public Image icon;
        UnitData unit;

        public void Init(UnitData unit)
        {
            this.unit = unit;
            icon.sprite = unit.icon;
        }

        public void Add()
        {
            Base.active.AddUnit(unit);
        }
    }
}