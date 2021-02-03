using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Instance;
using CT.Helper;

namespace CT.UI
{
    public class ConstructionUIManager : MonoBehaviour
    {
        public GameObject constructionUI;

        public Text nameText, timeLeftText, rushText;
        public Image fillImage, iconImage;

        public static ConstructionUIManager instance;

        public Construction Chosen { get; private set; }

        void Awake()
        {
            instance = this;
        }

        public void TurnOff()
        {
            enabled = false;
            Chosen = null;
            if (constructionUI != null) constructionUI.SetActive(false);
        }

        public void Init(Construction construction)
        {
            enabled = true;
            Chosen = construction;
            constructionUI.SetActive(true);
            nameText.text = Chosen.Data.toConstruct.name;
            iconImage.sprite = Chosen.Data.toConstruct.Original.icon;
        }

        void FixedUpdate()
        {
            if (Chosen == null)
            {
                TurnOff();
                return;
            }
            timeLeftText.text = Chosen.Countdown.ToDisplayText();
            fillImage.fillAmount = Chosen.FillRatio;
            rushText.text = $"Rush for {Chosen.RushGemCost}";
        }

        public void Cancel()
        {
            Chosen.Cancel();
            TurnOff();
        }

        public void Rush()
        {
            Chosen.Rush();
            TurnOff();
        }
    }
}
