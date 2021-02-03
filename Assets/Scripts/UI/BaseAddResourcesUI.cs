using CT.Manager;
using CT.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CT.UI
{
    public class BaseAddResourcesUI : MonoBehaviour
    {
        public GameObject headUI;
        public GameObject ui;
        public OptionUI _10PercentUI, halfUI, fillUI;

        public bool isGold;

        [Header("Elixir")]
        public Image[] elixirIconImages;

        public static BaseAddResourcesUI goldInstance, elixirInstance;

        void Awake()
        {
            if (isGold) goldInstance = this;
            else elixirInstance = this;
        }

        public void Init(int stored, int capacity)
        {
            headUI.SetActive(true);
            ui.SetActive(true);
            float fill = (float)stored / capacity;
            float toFill = 1 - fill;
            _10PercentUI.chooseButton.interactable = fill <= 0.9f;
            halfUI.chooseButton.interactable = fill <= 0.5f;
            fillUI.chooseButton.interactable = fill < 1;

            _10PercentUI.amountText.text = (capacity / 10).ToString();
            halfUI.amountText.text = (capacity / 2).ToString();
            fillUI.amountText.text = (capacity - stored).ToString();

            _10PercentUI.gemCostText.text = "1";
            halfUI.gemCostText.text = "4";
            fillUI.gemCostText.text = ((int)(toFill * 7)).ToString();

            var elixirIcon = GameManager.Faction.elixirIcon;

            foreach (var image in elixirIconImages) image.sprite = elixirIcon;
        }

        public void TurnOff()
        {
            ui.SetActive(false);
            headUI.SetActive(false);
        }

        public void ChooseToAdd(int percent)
        {
            int amount = 0;
            int gems = 0;

            switch (percent)
            {
                case 10: amount = Convert.ToInt32(_10PercentUI.amountText.text); gems = 1; break;
                case 50: amount = Convert.ToInt32(halfUI.amountText.text); gems = 4; break;
                case 100:
                    amount = Convert.ToInt32(fillUI.amountText.text);
                    gems = Convert.ToInt32(fillUI.gemCostText.text);
                    break;
            }

            int gold = isGold ? amount : 0;
            int elixir = isGold ? 0 : amount;

            ClientSend.AddResources(Base.active.Data.ID, gold, elixir);
            ClientSend.SubtractGems(GameManager.Player.username, gems);

            TurnOff();
        }

        [Serializable]
        public class OptionUI
        {
            public Text amountText;
            public Text gemCostText;
            public Button chooseButton;
        }
    }
}
