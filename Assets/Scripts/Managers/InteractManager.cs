using CT.UI;
using CT.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.UI.Training;
using CT.UI.Army;

namespace CT.Manager
{
    public class InteractManager : UIManager
    {
        public enum InteractMode { Interact, Upgrade }
        public InteractMode interactMode;
        public Sprite interactIcon, upgradeIcon;

        public Image interactModeImage;
        public GameObject gridUI;

        public static InteractManager instance;

        protected override KeyCode ActivationKey => KeyCode.I;

        InteractGridManager gridManager;
        ConstructionManager constructionManager;
        Base _base;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        protected override void Start()
        {
            base.Start();
            gridManager = InteractGridManager.instance;
            constructionManager = ConstructionManager.instance;
            _base = Base.active;
        }

        bool Upgrade(bool withResources)
        {
            if (!constructionManager.IsAvailable) return false;
            var building = gridManager.SelectedBuilding;
            var instanceData = building.BaseInstanceData;
            if (instanceData.BaseNextData.hallLevelNeeded > _base.MainHall.InstanceData.level) return false;
            //var data = building.data;
            //int x = instanceData.tileX;
            //int y = instanceData.tileY;
            Vector3 pos = building.transform.position;
            Quaternion rot = building.transform.rotation;
            GameTime time;
            int gold = 0, elixir = 0, gems = 0;
            var player = GameManager.Player;
            if (withResources)
            {
                gold = instanceData.UpgradeCostGold;
                elixir = instanceData.UpgradeCostElixir;
                time = instanceData.UpgradeTime;
                if (_base.Gold < gold || _base.Elixir < elixir) return false;
                ClientSend.SubtractResources(_base.Data.ID, gold, elixir);
            }
            else
            {
                gems = instanceData.UpgradeCostGems;
                time = GameTime.Second;
                if (player.gems < gems) return false;
                ClientSend.SubtractGems(player.username, gems);
                player.gems -= gems;
            }
            gridManager.OnUpgrade();
            var conData = new ConstructionData(instanceData, time);
            constructionManager.Init(conData);
            return true;
        }

        void TryToUpgrade(bool normally)
        {
            bool upgraded = Upgrade(normally);

            if (upgraded) Disable();
            else Debug.Log("Failed to Upgrade!"); //to do some kind of ui
        }

        public void UpgradeWithResources()
        {
            TryToUpgrade(true);
        }

        public void UpgradeWithGems()
        {
            TryToUpgrade(false);
        }

        public void CancelConstruction(bool fromUpgrade)
        {
            var construction = gridManager.SelectedConstruction;
            construction.Cancel();
            Disable();
        }

        public void RushConstruction(bool fromUpgrade)
        {
            var construction = gridManager.SelectedConstruction;
            construction.Rush();
            Disable();
        }

        protected override void OnUIToggled()
        {
            if (on)
            {
                gridUI.SetActive(true);
                interactMode = InteractMode.Interact;
                interactModeImage.sprite = interactIcon;
            }
            else Disable();
        }

        public void SwitchInteractMode()
        {
            switch (interactMode)
            {
                case InteractMode.Interact:
                    interactMode = InteractMode.Upgrade;
                    interactModeImage.sprite = upgradeIcon;
                    break;

                case InteractMode.Upgrade:
                    interactMode = InteractMode.Interact;
                    interactModeImage.sprite = interactIcon;
                    break;
            }
        }

        public void Cancel()
        {
            Disable();
        }

        void OnDisable()
        {
            Disable();
        }

        protected override void Disable()
        {
            base.Disable();
            if(ui != null) ui.SetActive(false);
            if(gridManager != null) gridManager.Deactivate();
            if(TrainingUI.instance != null) TrainingUI.instance.Deactivate();
            if(ArmyUI.instance != null) ArmyUI.instance.Deactivate();
        }
    }
}