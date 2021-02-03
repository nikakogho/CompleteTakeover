using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.UI;

namespace CT.Manager
{
    namespace UI
    {
        public class BuildOptionManager : MonoBehaviour
        {
            public GameObject gridUI;
            public BuildGridManager gridManager;
            public GameObject optionPrefab;
            public static BuildOptionManager instance;

            public Transform resourcesParent, armyParent, turretsParent, defensesParent, decorationsParent;

            bool init = false;

            public Dictionary<BuildingData, BuildOptionUI> options;

            List<GameObject> optionObjects;
            Base _base;

            public BuildOptionManager()
            {
                instance = this;
                optionObjects = new List<GameObject>();
                options = new Dictionary<BuildingData, BuildOptionUI>();
            }

            void Start()
            {
                _base = Base.active;
                InitOptions();
            }

            public void ReInitOptions()
            {
                init = false;
                foreach (var obj in optionObjects) Destroy(obj);
                optionObjects.Clear();
                InitOptions();
            }

            void InitOptions()
            {
                if (init) return;
                init = true;
                var options = _base.MainHall.GetAllowedBuildings();
                foreach (var option in options) InitOption(option.Key, option.Value);
            }

            Transform GetParent(BuildingData.BuildingType buildingType)
            {
                switch (buildingType)
                {
                    case BuildingData.BuildingType.Mine:
                    case BuildingData.BuildingType.Storage:
                        return resourcesParent;

                    case BuildingData.BuildingType.ArmyHolder:
                    case BuildingData.BuildingType.TrainingZone:
                    case BuildingData.BuildingType.Lab:
                        return armyParent;

                    case BuildingData.BuildingType.Turret: return turretsParent;

                    case BuildingData.BuildingType.Wall:
                    case BuildingData.BuildingType.Trap:
                    case BuildingData.BuildingType.Bunker:
                        return defensesParent;

                    case BuildingData.BuildingType.Decoration: return decorationsParent;

                    default: throw new System.ArgumentException("Invalid Building Type!");
                }
            }

            void InitOption(BuildingData option, int allowedAmount)
            {
                var parent = GetParent(option.Type);
                var obj = Instantiate(optionPrefab, parent);
                var ui = obj.GetComponent<BuildOptionUI>();
                int existingAmount = _base.GetBuildingCount(option);
                ui.Init(option, existingAmount, allowedAmount);
                optionObjects.Add(obj);
                options[option] = ui;
            }

            void SelectBuilding(BuildingData data, bool buyWithGems)
            {
                gridUI.SetActive(true);
                gridManager.ThinkToBuild(data, !buyWithGems);
            }

            public void SelectBuilding(BuildingData data)
            {
                SelectBuilding(data, false);
            }

            public void SelectBuildingWithGems(BuildingData data)
            {
                SelectBuilding(data, true);
            }

            void OnDisable()
            {
                if (gridUI != null) gridUI.SetActive(false);
            }
        }
    }
}