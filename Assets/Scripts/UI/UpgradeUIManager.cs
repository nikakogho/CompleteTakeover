using CT.Data;
using CT.Instance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.UI.Upgrade;
using CT.Manager;

namespace CT.UI
{
    public class UpgradeUIManager : MonoBehaviour
    {
        public static UpgradeUIManager instance;

        public GameObject upgradeUI;
        public GameObject mineUI, storageUI, turretUI, wallUI, trapUI;
        public GameObject bunkerUI, campUI, trainingZoneUI, labUI, hallUI;

        #region Upgrade UI

        public ArmyHolderUpgradeUI armyHolderUpgradeUI;
        public BunkerUpgradeUI bunkerUpgradeUI;
        public LabUpgradeUI labUpgradeUI;
        public MainHallUpgradeUI mainHallUpgradeUI;
        public MineUpgradeUI mineUpgradeUI;
        public StorageUpgradeUI storageUpgradeUI;
        public TrainingZoneUpgradeUI trainingZoneUpgradeUI;
        public TrapUpgradeUI trapUpgradeUI;
        public TurretUpgradeUI turretUpgradeUI;
        public WallUpgradeUI wallUpgradeUI;

        #endregion

        GameObject selectedUI;
        //BuildingInstanceData selectedData;

        void Awake()
        {
            instance = this;
        }

        public void TurnOff()
        {
            if(upgradeUI != null) upgradeUI.SetActive(false);
            if(selectedUI != null)
            {
                selectedUI.SetActive(false);
                selectedUI = null;
            }
        }

        void InitUI<T>(GameObject ui, T data, UpgradeUI<T> up) where T : BuildingInstanceData
        {
            upgradeUI.SetActive(true);
            ui.SetActive(true);
            selectedUI = ui;
            //selectedData = data;
            up.Init(data, ConstructionManager.instance.GetUpgradeOf(data));
        }

        #region Select

        public void Select(Mine mine)
        {
            InitUI(mineUI, mine.InstanceData, mineUpgradeUI);
        }

        public void Select(Storage storage)
        {
            InitUI(storageUI, storage.InstanceData, storageUpgradeUI);
        }

        public void Select(Turret turret)
        {
            InitUI(turretUI, turret.InstanceData, turretUpgradeUI);
        }

        public void Select(Wall wall)
        {
            InitUI(wallUI, wall.InstanceData, wallUpgradeUI);
        }

        public void Select(Trap trap)
        {
            InitUI(trapUI, trap.InstanceData, trapUpgradeUI);
        }

        public void Select(MainHall hall)
        {
            InitUI(hallUI, hall.InstanceData, mainHallUpgradeUI);
        }

        public void Select(TrainingZone trainingZone)
        {
            InitUI(trainingZoneUI, trainingZone.InstanceData, trainingZoneUpgradeUI);
        }

        public void Select(CampZone camp)
        {
            InitUI(campUI, camp.InstanceData, armyHolderUpgradeUI);
        }
        
        public void Select(Lab lab)
        {
            InitUI(labUI, lab.InstanceData, labUpgradeUI);
        }

        public void Select(Bunker bunker)
        {
            InitUI(bunkerUI, bunker.InstanceData, bunkerUpgradeUI);
        }
        
        #endregion
    }
}