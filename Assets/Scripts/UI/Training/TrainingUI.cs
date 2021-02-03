using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Data.Instance;
using CT.Instance;
using CT.Manager;
using CT.Helper;
using CT.Net;

namespace CT.UI.Training
{
    public class TrainingUI : MonoBehaviour
    {
        public Transform optionsParent, slotsParent;
        public GameObject optionPrefab, slotPrefab;

        public GameObject totalUI, trainingHereUI, trainingEmptyUI;

        public Image trainingIcon;
        public Image trainingFillImage, totalFillImage;

        public Text trainTimeText;
        public Text totalTimeText, totalRushCostText;

        List<TrainingOptionUI> options;
        List<TrainingSlotUI> slots;

        TrainingZone zone;
        PlayerData player;
        Base _base;

        //GameObject actionsUI;

        public static TrainingUI instance;

        IEnumerator uiUpdateRoutine;

        int totalRushCost;

        public TrainingUI()
        {
            instance = this;

            options = new List<TrainingOptionUI>();
            slots = new List<TrainingSlotUI>();
        }

        public void Init(TrainingZone zone)
        {
            if (gameObject.activeSelf) Deactivate();
            gameObject.SetActive(true);
            //actionsUI = GameManager.instance.actionsPanel;
            //actionsUI.SetActive(false);
            this.zone = zone;
            _base = Base.active;
            player = GameManager.Player;
            //InitCurrent();
            InitSlots();
            InitOptions();
            UpdateUI();
            uiUpdateRoutine = UIUpdateRoutine();
            StartCoroutine(uiUpdateRoutine);
        }

        #region Init

        /* may use at some point
        void InitCurrent()
        {

        }
        */

        void InitSlots()
        {
            #region Empty

            foreach (int size in zone.VersionData.slotSizes)
            {
                var obj = Instantiate(slotPrefab, slotsParent);
                var slot = obj.GetComponent<TrainingSlotUI>();
                slot.InitEmpty(size);
                slots.Add(slot);
            }

            #endregion

            #region Fill

            var fillSlots = zone.InstanceData.slots;

            for (int i = 0; i < fillSlots.Count; i++)
            {
                slots[i].Init(this, fillSlots[i]);
            }

            #endregion
        }

        void InitOptions()
        {
            foreach(var unit in zone.VersionData.canTrain)
            {
                var obj = Instantiate(optionPrefab, optionsParent);
                var option = obj.GetComponent<TrainingOptionUI>();
                option.Init(this, unit);
                options.Add(option);
            }
        }

        #endregion

        public void Deactivate()
        {
            if (!gameObject.activeSelf) return;

            foreach (var option in options) Destroy(option.gameObject);
            foreach (var slot in slots) Destroy(slot.gameObject);
            options.Clear();
            slots.Clear();
            zone = null;
            //actionsUI.SetActive(true);
            if(uiUpdateRoutine != null) StopCoroutine(uiUpdateRoutine);
            gameObject.SetActive(false);
        }

        public void TryToBuy(UnitData unit)
        {
            int cost = unit.trainCost;

            if (cost > _base.Gold)
            {
                Debug.Log("Insufficient gold!"); //to do some UI
                return;
            }

            bool didBuy = true;

            if (zone.Training == null) StartTraining(unit);
            else if (!AddToSlots(unit))
            {
                didBuy = false;
                Debug.Log("No room for this unit!"); //to do some UI
            }

            if (didBuy) ClientSend.SubtractResources(_base.Data.ID, cost, 0);
        }

        public void RemoveSlot(TrainingSlotUI slot)
        {
            zone.InstanceData.slots.Remove(slot.slot);
            UpdateSlots();
        }

        void StartTraining(UnitData unit)
        {
            zone.StartTraining(unit);
            UpdateUI();
        }

        bool AddToSlots(UnitData unit)
        {
            foreach(var slot in slots)
            {
                if(slot.IsEmpty)
                {
                    var newSlot = new TrainingZoneInstanceData.TrainSlot(unit, 1);
                    zone.InstanceData.slots.Add(newSlot);
                    slot.Init(this, newSlot);
                    return true;
                }
                if (slot.slot.unit != unit) continue;
                if (slot.slot.amount == slot.Capacity) continue;
                slot.slot.amount++;
                slot.amountText.text = slot.slot.amount.ToString();
                return true;
            }

            return false;
        }

        void UpdateSlots()
        {
            int size = zone.InstanceData.slots.Count;
            for (int i = 0; i < size; i++) slots[i].Init(this, zone.InstanceData.slots[i]);
            for (int i = size; i < slots.Count; i++) slots[i].InitEmpty(zone.VersionData.slotSizes[i]);
        }

        IEnumerator UIUpdateRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (enabled) UpdateUI();
            }
        }

        void UpdateTotalStats()
        {
            var totalTimeLeft = zone.TotalTimeLeft;
            totalRushCost = totalTimeLeft.GemCost;

            totalTimeText.text = totalTimeLeft.ToDisplayText();
            totalRushCostText.text = totalRushCost.ToString();
        }

        void UpdateUI()
        {
            bool isTraining = zone.Training != null;

            totalUI.SetActive(isTraining);
            trainingHereUI.SetActive(isTraining);
            trainingEmptyUI.SetActive(!isTraining);

            if (!isTraining)
            {
                if (!slots[0].IsEmpty)
                    for(int i = 0; i < slots.Count; i++) slots[i].InitEmpty(zone.VersionData.slotSizes[i]);
                return;
            }

            var trainTimeLeft = zone.TimeLeft;
            var totalTimeLeft = zone.TotalTimeLeft;

            UpdateTotalStats();

            trainTimeText.text = trainTimeLeft.ToDisplayText();

            var initialTrainTime = zone.Training.trainTime;
            var initialTotalTime = totalTimeLeft - trainTimeLeft + initialTrainTime;

            trainingFillImage.fillAmount = 1f - trainTimeLeft.TotalMinutes / initialTrainTime.TotalMinutes;
            totalFillImage.fillAmount = 1f - totalTimeLeft.TotalMinutes / initialTotalTime.TotalMinutes;

            trainingIcon.sprite = zone.Training.icon;

            UpdateSlots();
        }
        
        #region Buttons

        public void RushAll()
        {
            UpdateTotalStats();

            if(player.gems < totalRushCost)
            {
                Debug.Log("Not enough gems to rush all units!");
                return;
            }

            int actualRushCost = zone.FinishAll();

            UpdateUI();

            ClientSend.SubtractGems(player.username, actualRushCost);
        }

        public void CancelCurrent()
        {
            ClientSend.AddResources(_base.Data.ID, zone.Training.trainCost, 0);
            zone.GetNext();
            UpdateUI();
        }

        #endregion
    }
}