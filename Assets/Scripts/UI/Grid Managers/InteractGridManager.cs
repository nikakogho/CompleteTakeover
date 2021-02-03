using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Manager;

namespace CT.UI
{
    public class InteractGridManager : GridManager
    {
        public override Color NowBuildingHereColor => tileValidColor;
        public override Color StartBuildingHereColor => tileValidColor;
        public override Color ConstructionHereColor => tileValidColor;

        public UpgradeUIManager UpgradeUIManager { get; private set; }
        public ConstructionUIManager ConstructionUIManager { get; private set; }

        public Building SelectedBuilding { get; private set; }
        public Construction SelectedConstruction { get; private set; }

        public static InteractGridManager instance;

        InteractManager manager;

        public InteractGridManager()
        {
            instance = this;
        }

        void Start()
        {
            manager = InteractManager.instance;
            UpgradeUIManager = UpgradeUIManager.instance;
            ConstructionUIManager = ConstructionUIManager.instance;
        }

        void OnEnable()
        {
            if (grid != null) DestroyGrid();
            GenerateGrid();
        }

        void BuildingClicked(Building building)
        {
            switch (manager.interactMode)
            {
                case InteractManager.InteractMode.Interact: building.OnInteract(); break;
                case InteractManager.InteractMode.Upgrade: building.OnUpgradeUISelected(); break;
            }
        }

        void NewConstructionClicked(Construction construction)
        {
            //construction.OnInteract();
            ConstructionUIManager.Init(construction);
            SelectedConstruction = construction;
        }

        public override void OnTileClicked(int x, int y)
        {
            var tile = grid[x, y];
            var building = SelectedBuilding = tile.Building;

            var construction = (SelectedConstruction = tile.Construction);
            
            if (building == null)
            {
                if (construction == null) return;

                NewConstructionClicked(construction);

                return;
            }

            if (construction == null) BuildingClicked(building);
            else building.OnUpgradeUISelected();
        }

        public void Deactivate()
        {
            //if (gameObject == null) return;
            //if (!gameObject.activeSelf) return;
            SelectedBuilding = null;
            if(grid != null) DestroyGrid();
            if (UpgradeUIManager != null) UpgradeUIManager.TurnOff();
            if (ConstructionUIManager != null) ConstructionUIManager.TurnOff();
            if(gameObject != null) gameObject.SetActive(false);
        }

        public void OnUpgrade()
        {
            //Deactivate(); may do
        }
    }
}