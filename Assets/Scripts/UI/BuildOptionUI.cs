using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Manager.UI;

namespace CT.UI
{
    public class BuildOptionUI : MonoBehaviour
    {
        public Button button;
        public Image image;
        public Text nameText;
        public Text priceText;
        public Text amountText;

        BuildingData data;

        public int CurrentAmount { get; private set; }
        public int AllowedAmount { get; private set; }

        public void Init(BuildingData data, int currentAmount, int allowedAmount)
        {
            this.data = data;
            image.sprite = data.Original.icon;
            nameText.text = data.name;
            priceText.text = data.Original.buildCostGold.ToString();
            UpdateAmounts(currentAmount, allowedAmount);
        }

        public void UpdateAmounts(int currentAmount, int allowedAmount)
        {
            CurrentAmount = currentAmount;
            AllowedAmount = allowedAmount;
            bool free = currentAmount < allowedAmount;
            button.interactable = free;
            amountText.color = free ? Color.white : Color.red;
            amountText.text = $"{currentAmount} / {allowedAmount}";
        }

        public void OnNormalBuyClick()
        {
            BuildOptionManager.instance.SelectBuilding(data);
        }

        public void OnGemBuyClick()
        {
            BuildOptionManager.instance.SelectBuildingWithGems(data);
        }
    }
}