using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Net;
using CT.UI.Combat;

namespace CT.UI
{
    public class AttackReportUI : MonoBehaviour
    {
        public Text attackerNameText;
        public Text stolenGoldText;
        public Text stolenElixirText;
        public Image elixirImage;
        public Transform deployedParent;
        public GameObject deployedSquadPrefab;
        int id;

        public void Init(AttackReport report)
        {
            id = report.ID;
            attackerNameText.text = report.attackerUsername;
            stolenGoldText.text = report.takenGold.ToString();
            stolenElixirText.text = report.takenElixir.ToString();
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
            ClientSend.ConfirmDefenderProcessed(false, id);
            Destroy(gameObject);
        }
    }
}