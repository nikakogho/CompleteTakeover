using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data.Instance;
using CT.Data;
using CT.Helper;
using CT.Manager;

namespace CT.UI.Upgrade
{
    public abstract class UpgradeUI<T> : MonoBehaviour where T : BuildingInstanceData
    {
        public GameObject currentSide, upgradedSide, cantUpgradeUI;
        public Text goldCostText, elixirCostText, gemCostText;
        public Image elixirCostIcon;

        #region Costs

        public GameObject upgradeCostsAndNeedsUI;
        public GameObject goldCostUI, elixirCostUI;
        public Text upgradeTimeText;

        #endregion

        #region Needs

        public Image hallNeededIcon;
        public Text hallLevelNeededText;

        #endregion

        #region Construction

        public GameObject upgradeConstructionUI;
        public Text timeLeftText, rushCostText;
        public Image constructionFilledImage;

        #endregion

        public Text oldNameText, newNameText;
        public Text oldHealthText, newHealthText;
        public Image oldIcon, newIcon;
        
        public enum BuildMode { Normal, Instant }
        protected BuildMode buildMode;

        protected Construction construction;

        protected virtual void ApplyBasics()
        {
            var kids = new Dictionary<string, GameObject>();
            var stack = new Stack<Transform>();
            stack.Push(transform);
            do
            {
                var parent = stack.Pop();
                int count = parent.childCount;
                for (int i = 0; i < count; i++)
                {
                    var tr = parent.GetChild(i);
                    kids.Add(tr.gameObject.name, tr.gameObject);
                    stack.Push(tr);
                }
            } while (stack.Count > 0);
            currentSide = kids["Current Side Panel"];
            upgradedSide = kids["New Side Panel"];
            cantUpgradeUI = kids["Max Level Reached Side"];

            #region Cost

            upgradeCostsAndNeedsUI = kids["Costs And Needs Panel"];
            goldCostUI = kids["Gold Cost Panel"];
            elixirCostUI = kids["Elixir Cost Panel"];
            goldCostText = kids["Gold Cost Text"].GetComponent<Text>();
            elixirCostText = kids["Elixir Cost Text"].GetComponent<Text>();
            gemCostText = kids["Gems Cost Text"].GetComponent<Text>();
            elixirCostIcon = kids["Elixir Cost Icon"].GetComponent<Image>();

            #endregion

            #region Need

            hallNeededIcon = kids["Hall Level Needed Image"].GetComponent<Image>();
            hallLevelNeededText = kids["Hall Level Needed Text"].GetComponent<Text>();

            #endregion

            #region Construction

            upgradeConstructionUI = kids["Construction Panel"];
            timeLeftText = kids["Time Left Text"].GetComponent<Text>();
            rushCostText = kids["Rush Cost Text"].GetComponent<Text>();
            constructionFilledImage = kids["Construction Fill Image"].GetComponent<Image>();

            #endregion

            oldIcon = kids["Current Icon"].GetComponent<Image>();
            newIcon = kids["New Icon"].GetComponent<Image>();
            upgradeTimeText = kids["Build Time Text"].GetComponent<Text>();
            oldNameText = kids["Current Name Text"].GetComponent<Text>();
            newNameText = kids["New Name Text"].GetComponent<Text>();
            oldHealthText = kids["Current Health Text"].GetComponent<Text>();
            newHealthText = kids["New Health Text"].GetComponent<Text>();
        }
        
        public void Init(T instance, Construction construction)
        {
            var data = instance.data;
            var current = instance.BaseCurrentData;
            this.construction = construction;
            oldNameText.text = instance.FullName;
            oldHealthText.text = current.health.ToString();
            oldIcon.sprite = current.icon;
            if(!instance.HasNextVersion)
            {
                upgradedSide.SetActive(false);
                upgradeCostsAndNeedsUI.SetActive(false);
                upgradeConstructionUI.SetActive(false);
                cantUpgradeUI.SetActive(true);
            }
            else
            {
                var next = instance.BaseNextData;
                newHealthText.text = next.health.ToString();
                newNameText.text = instance.NextLevelName;
                elixirCostIcon.sprite = data.Faction.elixirIcon;
                newIcon.sprite = next.icon;

                if (construction == null)
                {
                    upgradeCostsAndNeedsUI.SetActive(true);
                    upgradeConstructionUI.SetActive(false);
                    upgradedSide.SetActive(true);
                    cantUpgradeUI.SetActive(false);
                    int hallLevelNeeded = next.hallLevelNeeded;
                    var hall = data.Faction.mainHallData.GetLevelData(hallLevelNeeded);
                    var hallIcon = hall.icon;
                    hallLevelNeededText.text = $"Hall Level {hallLevelNeeded} Needed";
                    hallNeededIcon.sprite = hallIcon;
                    int gold = next.buildCostGold;
                    int elixir = next.buildCostElixir;
                    int gems = next.buildCostGems;
                    goldCostUI.SetActive(gold > 0);
                    goldCostText.text = gold.ToString();
                    elixirCostUI.SetActive(elixir > 0);
                    elixirCostText.text = elixir.ToString();
                    gemCostText.text = gems.ToString();
                    upgradeTimeText.text = next.buildTime.ToDisplayText();
                }
                else
                {
                    upgradeCostsAndNeedsUI.SetActive(false);
                    upgradeConstructionUI.SetActive(true);
                    TimeLeftUpdate();
                }
            }
            OnInit(instance);
        }

        protected abstract void OnInit(T instance);

        public void SelectBuildMode(bool normal)
        {
            buildMode = normal ? BuildMode.Normal : BuildMode.Instant;
        }

        void FixedUpdate()
        {
            if (construction == null) return;
            TimeLeftUpdate();
        }

        void TimeLeftUpdate()
        {
            float fill = construction.FillRatio;
            timeLeftText.text = construction.Countdown.ToDisplayText();
            constructionFilledImage.fillAmount = fill;
            rushCostText.text = construction.RushGemCost.ToString();

            if (construction.Countdown == GameTime.Second) InteractManager.instance.OnManagerSelected();
        }

        void OnDisable()
        {
            construction = null;
        }
    }
}