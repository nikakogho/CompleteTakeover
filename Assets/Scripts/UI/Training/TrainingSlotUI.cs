using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Data.Instance;
using CT.Instance;
using CT.Net;

namespace CT.UI.Training
{
    public class TrainingSlotUI : MonoBehaviour
    {
        public Image iconImage;
        public GameObject emptyUI, busyUI;
        public Text maxAmountText;
        public Text amountText;

        public TrainingZoneInstanceData.TrainSlot slot;

        public int Capacity { get; private set; }

        TrainingUI ui;

        public bool IsEmpty { get; private set; }

        public void InitEmpty(int capacity)
        {
            IsEmpty = true;
            Capacity = capacity;
            emptyUI.SetActive(true);
            busyUI.SetActive(false);
            maxAmountText.text = $"{capacity}x";
        }

        public void Init(TrainingUI ui, TrainingZoneInstanceData.TrainSlot slot)
        {
            this.slot = slot;
            this.ui = ui;
            IsEmpty = false;
            busyUI.SetActive(true);
            emptyUI.SetActive(false);
            iconImage.sprite = slot.unit.icon;
            amountText.text = slot.amount.ToString();
        }

        public void RemoveOne()
        {
            slot.amount--;
            amountText.text = slot.amount.ToString();
            ClientSend.AddResources(Base.active.Data.ID, slot.unit.trainCost, 0);

            if (slot.amount == 0) ui.RemoveSlot(this);
        }
    }
}
