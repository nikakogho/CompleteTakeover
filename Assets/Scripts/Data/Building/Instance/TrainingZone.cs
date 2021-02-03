using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.UI;
using CT.UI.Training;
using CT.Manager;

namespace CT.Instance
{
    public class TrainingZone : Building//<TrainingZoneData, TrainingZoneInstanceData>
    {
        public TrainingZoneData Data => BaseData as TrainingZoneData;
        public TrainingZoneInstanceData InstanceData => BaseInstanceData as TrainingZoneInstanceData;
        public TrainingZoneData.VersionData VersionData => InstanceData.CurrentData;

        public UnitData Training { get; private set; }
        public GameTime TimeLeft => InstanceData.timeLeft;

        public Transform spawnPoint;

        public GameTime TotalTimeLeft { get {
                var time = TimeLeft;
                foreach (var slot in InstanceData.slots) time += slot.SlotTime;
                return time;
        } }

        //Queue<TrainingZoneInstanceData.TrainSlot> queue;

        TrainingUI ui;
        
        void Start()
        {
            //queue = new Queue<TrainingZoneInstanceData.TrainSlot>();
            ui = TrainingUI.instance;
        }

        IEnumerator TrainRoutine()
        {
            yield return new WaitForSeconds(1);
            while (!TimeLeft.Done)
            {
                //TimeLeft -= GameTime.Second;
                InstanceData.timeLeft -= GameTime.Second;
                yield return new WaitForSeconds(1);
            }

            DoneTraining();
        }

        void AddUnit()
        {
            var unitTransform = Base.AddUnit(Training);
            if (unitTransform != null) unitTransform.position = spawnPoint.position;
        }

        public override void ApplyInstanceData(Base _base, BuildingInstanceData instanceData)
        {
            base.ApplyInstanceData(_base, instanceData);

            string unitName = InstanceData.trainingUnitName;

            if (unitName == null) return;

            Training = LoadManager.unitsDictionary[unitName];
            //TimeLeft = InstanceData.timeLeft;

            StartCoroutine(TrainRoutine());
        }

        public void StartTraining(UnitData unit)
        {
            Training = unit;
            InstanceData.timeLeft = unit.trainTime;
            InstanceData.trainingUnitName = Training.name;
            InstanceData.timeLeft = TimeLeft;

            StartCoroutine(TrainRoutine());
        }

        public void GetNext(bool changeRoutines = true)
        {
            if(changeRoutines) StopAllCoroutines();

            InstanceData.trainingUnitName = null;
            Training = null;
            InstanceData.timeLeft = GameTime.Zero;

            if (InstanceData.slots.Count == 0) return;

            var slot = InstanceData.slots[0];//queue.Peek();
            if (slot == null) return;
            Training = slot.unit;
            InstanceData.trainingUnitName = Training.name;
            InstanceData.timeLeft = Training.trainTime;
            slot.amount--;
            if (slot.amount == 0) InstanceData.slots.RemoveAt(0);

            if(changeRoutines) StartCoroutine(TrainRoutine());
        }

        void DoneTraining(bool setRoutines = true)
        {
            AddUnit();
            GetNext(setRoutines);
        }

        public int FinishAll()
        {
            int room = Base.Data.ArmyCapacity - Base.Data.StoredArmySize;
            var skippedTime = GameTime.Zero;

            while (Training != null && room >= Training.capacity)
            {
                skippedTime += TimeLeft;
                DoneTraining(false);
            }

            int gemCost = skippedTime.GemCost;

            return gemCost;
        }

        public override void OnInteract()
        {
            ui.Init(this);
        }

        public override void OnUpgradeUISelected()
        {
            UpgradeUIManager.instance.Select(this);
        }
    }
}