using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using CT.Data;
using CT.Army;
using CT.Net;
using CT.UI;

namespace CT.Manager.Combat
{
    public class PlayerCombatManager : CombatManager<PlayerCombatManager, PlayerAttackArmy>
    {
        public LayerMask spawnMask;

        public GameObject inGameUI;

        public Transform deploysParent;
        public GameObject deployUIPrefab;

        public GameObject[] spawnAreas;

        public Text retreatText;

        static bool isRevenge;

        static BaseData attackerBase;

        DeploySquadUI selected;
        ArmySquad squad;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            GenerateDeployUI();
        }

        void GenerateDeployUI()
        {
            foreach(var squad in army.squads)
            {
                //Debug.Log($"{squad.amount} {squad.name} here!");
                var obj = Instantiate(deployUIPrefab, deploysParent);
                var ui = obj.GetComponent<DeploySquadUI>();
                ui.Init(squad.unit, squad.amount);
            }
        }

        public void Select(DeploySquadUI chosen)
        {
            selected = chosen;
            if (selected == null)
            {
                UnSelect();
                return;
            }

            foreach (var squad in army.squads)
                if (squad.unit == chosen.Unit)
                {
                    this.squad = squad;
                    break;
                }

            foreach (var area in spawnAreas) area.SetActive(true);
        }

        public void UnSelect()
        {
            selected = null;
            squad = null;
            foreach (var area in spawnAreas) area.SetActive(false);
        }

        protected override void OnAttackOver(bool destroyedWholeBase)
        {
            //to do write this
            inGameUI.SetActive(false);
            var destroyedBuildings = DefenderBase.DestroyedBuildings.Select(b => b.BaseInstanceData.ID);
            var army = deployedArmy.Select(squad => new ArmySquad(squad.Key.name, squad.Value));
            string username = GameManager.Player.username;
            var robbedMines = DefenderBase.RobbedMines;
            var robbedStorages = DefenderBase.RobbedStorages;
            UpdateArmyData();
            ClientSend.ProcessAttack(defender.ID, attackerBase.ID, username, isRevenge, AmassedGold, AmassedElixir, army, 
                destroyedBuildings, robbedMines, robbedStorages);
        }

        void UpdateArmyData()
        {
            var squads = attackerBase.army.squads;
            foreach (var s in deployedArmy)
                squads.Where(sq => sq.unit == s.Key).FirstOrDefault().amount -= s.Value;
            for (int i = squads.Count - 1; i >= 0; i--) if (squads[i].amount == 0) squads.RemoveAt(i);
        }

        public static void Init(BaseData _defender, BaseData attacker, PlayerAttackArmy _army, bool revenge)
        {
            defender = _defender;
            attackerBase = attacker;
            army = _army;
            isRevenge = revenge;
            //Debug.Log("Init with defender null: " + (defender == null));
        }

        public void Retreat()
        {
            if (deployedUnitCount == 0) GoHome();
            else Over(false);
        }

        void Deploy(Vector3 pos, Quaternion rotation)
        {
            if (!started) retreatText.text = "RETREAT";
            Deploy(squad, pos, rotation);
            ClientSend.RemoveUnit(squad.name, attackerBase.ID);
            selected.Deploy();
        }

        void Update()
        {
            if (selected == null) return;
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000, spawnMask))
                {
                    Vector3 pos = hit.point;
                    Quaternion rotation = hit.transform.rotation;
                    Deploy(pos, rotation);
                }
            }
        }
    }
}