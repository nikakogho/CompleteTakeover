using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Net;
using CT.UI.Combat;

namespace CT.UI
{
    public class DefenseReportUI : MonoBehaviour
    {
        public Text invasionNameText;
        public Text lostGoldText;
        public Text lostElixirText;
        public Image elixirImage;
        public Transform deployedParent;
        public GameObject deployedSquadPrefab;
        int id;

        public void Init(DefenseReport report)
        {
            id = report.ID;
            invasionNameText.text = report.invasionName;
            lostGoldText.text = report.lostGold.ToString();
            lostElixirText.text = report.lostElixir.ToString();
            elixirImage.sprite = report.attacked.Faction.elixirIcon;
            GenerateDeployedUI(report.deployedArmy);
        }

        void GenerateDeployedUI(List<ArmySquad> deployedArmy)
        {
            foreach (var squad in deployedArmy)
            {
                var obj = Instantiate(deployedSquadPrefab, deployedParent);
                var ui = obj.GetComponent<DeployedSquadUI>();
                ui.Init(squad.unit, squad.amount);
            }
        }

        public void OK()
        {
            ClientSend.ConfirmDefenderProcessed(true, id);
            Destroy(gameObject);
        }
    }
}