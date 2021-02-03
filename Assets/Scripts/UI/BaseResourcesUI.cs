using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Manager;

namespace CT.UI
{
    public class BaseResourcesUI : MonoBehaviour
    {
        public Base _base;
        public Text goldText, elixirText, gemsText;
        public Image elixirIcon;
        public Image goldBar, elixirBar;

        PlayerData player;
        Faction faction;
        BaseAddResourcesUI goldAdder, elixirAdder;

        public static BaseResourcesUI instance;
        
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            player = GameManager.Player;
            faction = GameManager.Faction;
            goldAdder = BaseAddResourcesUI.goldInstance;
            elixirAdder = BaseAddResourcesUI.elixirInstance;
            elixirIcon.sprite = faction.elixirIcon;
            InvokeRepeating("UpdateValues", 0, 1);
        }

        public void UpdateValues()
        {
            int gold = _base.Gold;
            int elixir = _base.Elixir;
            int goldCapacity = _base.GoldCapacity;
            int elixirCapacity = _base.ElixirCapacity;

            gemsText.text = player.gems.ToString();

            float goldFill = (float)gold / goldCapacity;
            float elixirFill = (float)elixir / elixirCapacity;

            goldBar.fillAmount = goldFill;
            elixirBar.fillAmount = elixirFill;

            goldText.text = $"{gold} / {goldCapacity}";
            elixirText.text = $"{elixir} / {elixirCapacity}";
        }

        public void AddGoldUI()
        {
            goldAdder.Init(_base.Gold, _base.GoldCapacity);
        }

        public void AddElixirUI()
        {
            elixirAdder.Init(_base.Elixir, _base.ElixirCapacity);
        }
    }
}