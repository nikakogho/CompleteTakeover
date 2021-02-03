using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.Instance;
using CT.Manager.UI;
using CT.Net;
using UnityEngine.UI;

namespace CT.Manager
{
    public class ConstructionManager : MonoBehaviour
    {
        public GameObject prefab;

        public Text buildersText;

        public static ConstructionManager instance;
        Base _base;
        BaseData baseData;
        BuildOptionManager bom;

        List<Construction> constructions;

        public int TotalBuilders => baseData.builders;
        public int FreeBuilders => TotalBuilders - constructions.Count;
        public bool IsAvailable => FreeBuilders > 0;

        public Construction this[int x, int y] { get {
                foreach (var construction in constructions)
                {
                    var data = construction.Data;
                    if (x == Mathf.Clamp(x, data.x, data.endX) && y == Mathf.Clamp(y, data.y, data.endY))
                        return construction;
                }

                return null;
            } }

        void Awake()
        {
            instance = this;
            constructions = new List<Construction>();
        }

        void Start()
        {
            _base = Base.active;
            baseData = _base.Data;
            bom = BuildOptionManager.instance;
        }

        public int GetBeingBuiltCount(BuildingData data)
        {
            return constructions.Where(c => c.Data.toConstruct == data).Count();
        }

        public Construction GetUpgradeOf(BuildingInstanceData building)
        {
            return constructions.Where(c => c.Data.isUpgradeOf == building).FirstOrDefault();
        }

        public bool IsUpgrading(BuildingInstanceData building)
        {
            return GetUpgradeOf(building) != null;
        }

        public void InitCurrentConstructions()
        {
            foreach (var data in baseData.constructions) Init(data);
            UpdateBuildersText();
        }

        public Construction Init(ConstructionData data)
        {
            var obj = Instantiate(prefab, transform);
            var construction = obj.GetComponent<Construction>();
            construction.Init(data);
            constructions.Add(construction);
            UpdateBuildersText();

            return construction;
        }

        #region Over

        void OnConstructionOver(Construction construction)
        {
            constructions.Remove(construction);
            UpdateBuildersText();
        }

        public void OnConstructionCompleted(Construction construction)
        {
            var data = construction.Data;
            var upgrading = data.isUpgradeOf;
            if(upgrading != null)
            {
                if (upgrading.Type == BuildingInstanceData.BuildingType.MainHall) bom.ReInitOptions();
                //may do other checks
            }
            OnConstructionOver(construction);
        }
        
        public void OnConstructionCancelled(Construction construction)
        {
            var data = construction.Data;
            if(data.toConstruct != null)
            {
                var ui = bom.options[data.toConstruct];
                ui.UpdateAmounts(ui.CurrentAmount - 1, ui.AllowedAmount);
            }

            OnConstructionOver(construction);
        }

        #endregion

        void UpdateBuildersText()
        {
            buildersText.text = $"{FreeBuilders} / {TotalBuilders}";
        }

        public void AddBuilder()
        {
            _base.Data.builders++;
            ClientSend.AddBuilder(_base.Data.ID);
            UpdateBuildersText();
        }
    }
}
