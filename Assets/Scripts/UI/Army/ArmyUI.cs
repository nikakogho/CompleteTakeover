using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CT.UI.Army
{
    public class ArmyUI : MonoBehaviour
    {
        public Image fillImage;
        public Text fillText;

        public Transform slotsParent;
        public GameObject slotPrefab;

        public static ArmyUI instance;
        List<ArmySlotUI> slots;

        Base _base;

        public ArmyUI()
        {
            instance = this;
            slots = new List<ArmySlotUI>();
        }

        public void Init()
        {
            gameObject.SetActive(true);
            _base = Base.active;
            UpdateStored();
            UpdateSlots();
        }

        public void Deactivate()
        {
            if (!gameObject.activeSelf) return;

            ClearSlots();
            gameObject.SetActive(false);
        }

        #region Slots

        public void RemoveSlot(ArmySlotUI slot)
        {
            slots.Remove(slot);
            Destroy(slot.gameObject);
        }

        void InitSlot(UnitData unit, int amount)
        {
            var obj = Instantiate(slotPrefab, slotsParent);
            obj.name = $"{unit.name} slot";
            var slot = obj.GetComponent<ArmySlotUI>();
            slot.Init(this, unit, amount);
            slots.Add(slot);
        }

        void UpdateSlots()
        {
            ClearSlots();

            foreach (var squad in _base.Data.army.squads) InitSlot(squad.unit, squad.amount);
        }

        void ClearSlots()
        {
            if (slots.Count > 0)
            {
                foreach (var slot in slots) Destroy(slot.gameObject);
                slots.Clear();
            }
        }

        #endregion

        public void UpdateStored()
        {
            int stored = _base.Data.StoredArmySize;
            int capacity = _base.Data.ArmyCapacity;
            fillImage.fillAmount = (float)stored / capacity;
            fillText.text = $"{stored} / {capacity}";
        }
    }
}