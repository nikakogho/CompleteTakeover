using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Net;
using CT.UI;

namespace CT.Manager.UI
{
    public class BuildManager : UIManager
    {
        public static BuildManager instance;
        public GameObject gridUI;

        protected override KeyCode ActivationKey => KeyCode.B;

        Base _base;
        BaseData baseData;
        PlayerData player;
        ConstructionManager constructionManager;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
            player = GameManager.Player;
        }

        protected override void Start()
        {
            base.Start();
            _base = Base.active;
            baseData = _base.Data;
            constructionManager = ConstructionManager.instance;
        }

        protected override void OnUIToggled()
        {

        }

        protected override void Disable()
        {
            base.Disable();
            if(gridUI != null && gridUI.activeSelf) gridUI.SetActive(false);
        }

        bool CanBuild()
        {
            if (!BuildGridManager.placeWorks) return false;
            if (!constructionManager.IsAvailable) return false;
            var manager = BuildGridManager.instance;
            var data = manager.Chosen;
            if(manager.BuyWithResources)
                return _base.Gold >= data.Original.buildCostGold && _base.Elixir >= data.Original.buildCostElixir;
            return player.gems >= data.Original.buildCostGems;
        }

        void Build()
        {
            var manager = BuildGridManager.instance;
            var building = manager.Chosen;
            var original = building.Original;
            int x = manager.BuildX;
            int y = manager.BuildY;
            Vector3 pos = manager.BuildPos;
            Quaternion rot = manager.BuildRot;
            int gold = 0, elixir = 0, gems = 0;
            if (manager.BuyWithResources)
            {
                gold = original.buildCostGold;
                elixir = original.buildCostElixir;
                ClientSend.SubtractResources(baseData.ID, gold, elixir);
            }
            else
            {
                gems = original.buildCostGems;
                ClientSend.SubtractGems(player.username, gems);
                player.gems -= gems;
            }
            int ID = 0; //may do some kind of a change
            var data = new ConstructionData(ID, building, original.buildTime, x, y);
            var construction = constructionManager.Init(data);
            manager.OnBuild(construction);
        }

        void CantBuild()
        {
            Debug.Log("Can not build this building"); //to do some kind of display
        }

        public void ConfirmPlacement()
        {
            if (CanBuild()) Build();
            else CantBuild();
            Disable();
        }

        public void CancelPlacement()
        {
            Disable();
        }
    }
}