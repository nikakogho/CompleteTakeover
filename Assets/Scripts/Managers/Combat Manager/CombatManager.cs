using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CT.Data;
using CT.Army;
using CT.UI.Combat;

namespace CT.Manager.Combat
{
    public abstract class CombatManager<T, TArmy> : MonoBehaviour
        where T : CombatManager<T, TArmy>
        where TArmy : AttackArmy
    {
        public static T Instance { get; protected set; }
        public static TArmy army;

        protected List<Unit> deployedUnits;
        protected int deployedUnitCount;
        protected bool attackStarted;

        public string homeSceneName = "home";

        public GameObject attackerWonUI, attackerLostUI;

        public static int AmassedGold => DefendingBase.lostTotalGold;
        public static int AmassedElixir => DefendingBase.lostTotalElixir;

        protected static Dictionary<UnitData, int> deployedArmy;
        public Transform deployedArmyParentWon, deployedArmyParentLost;
        public GameObject deployedSquadPrefab;
        
        public Text amassedGoldTextAttackerWon, amassedGoldTextAttackerLost;
        public Text amassedElixirTextAttackerWon, amassedElixirTextAttackerLost;
        public Image amassedElixirImageAttackerWon, amassedElixirImageAttackerLost;

        const float checkDelta = 0.5f;

        protected static BaseData defender;
        protected static Faction defenderFaction;

        protected DefendingBase DefenderBase { get; private set; }

        protected bool started = false;
        bool over = false;

        protected virtual void Awake()
        {
            deployedUnits = new List<Unit>();
            deployedArmy = new Dictionary<UnitData, int>();
            defenderFaction = defender.Faction;
            SpawnEnvironment();
        }

        protected virtual void Start()
        {
            DefenderBase = DefendingBase.instance;
            DefenderBase.Apply(defender);
        }

        void SpawnEnvironment()
        {
            var env = Instantiate(defenderFaction.environmentPrefab, transform.position, transform.rotation);
            env.name = "Environment";
        }

        public void GoHome()
        {
            Time.timeScale = 1;
            LoadManager.RequestPlayerDataUpdate();
            //SceneManager.LoadScene(homeSceneName);
        }

        protected void Deploy(ArmySquad squad, Vector3 spot, Quaternion rotation)
        {
            var obj = Instantiate(squad.unit.attackingPrefab, spot, rotation, transform);
            var unit = obj.GetComponent<Unit>();
            deployedUnits.Add(unit);
            deployedUnitCount++;
            squad.amount--;
            //if (squad.amount == 0) army.squads.Remove(squad);

            var data = squad.unit;
            if (deployedArmy.ContainsKey(data)) deployedArmy[data]++;
            else deployedArmy.Add(data, 1);

            if (!started)
            {
                started = true;
                StartCoroutine(CheckOnUnitsRoutine());
            }
        }

        protected abstract void OnAttackOver(bool destroyedWholeBase);

        void GenerateDeployedUI(Transform parent)
        {
            foreach(var squad in deployedArmy)
            {
                var obj = Instantiate(deployedSquadPrefab, parent);
                var ui = obj.GetComponent<DeployedSquadUI>();
                ui.Init(squad.Key, squad.Value);
            }
        }

        protected void Over(bool destroyedWholeBase)
        {
            over = true;
            OnAttackOver(destroyedWholeBase);

            CombatSpeedManager.instance.OnBattleOver();

            //defender.storedGold -= AmassedGold;
            //defender.storedElixir -= AmassedElixir;

            amassedElixirImageAttackerLost.sprite = amassedElixirImageAttackerWon.sprite = defenderFaction.elixirIcon;
            amassedElixirTextAttackerLost.text = amassedElixirTextAttackerWon.text = AmassedElixir.ToString();
            amassedGoldTextAttackerLost.text = amassedGoldTextAttackerWon.text = AmassedGold.ToString();
            
            if (destroyedWholeBase)
            {
                attackerWonUI.SetActive(true);
                GenerateDeployedUI(deployedArmyParentWon);
            }
            else
            {
                attackerLostUI.SetActive(true);
                GenerateDeployedUI(deployedArmyParentLost);
            }
        }

        IEnumerator CheckOnUnitsRoutine()
        {
            while (!over)
            {
                yield return new WaitForSeconds(0.4f);
                CheckOnUnits();
            }
        }

        void CheckOnUnits()
        {
            for (int i = deployedUnitCount - 1; i > -1; i--)
            {
                if (deployedUnits[i] == null || deployedUnits[i].IsDead)
                {
                    deployedUnits.RemoveAt(i);
                    deployedUnitCount--;
                }
            }

            if (deployedUnitCount == 0 && army.IsEmpty) Over(false);
        }

        public void BaseDestroyed()
        {
            Over(true);
        }
    }
}

namespace CT.Army
{
    public abstract class AttackArmy
    {
        public List<ArmySquad> squads;

        public bool IsEmpty
        {
            get
            {
                foreach (var squad in squads) if (squad.amount > 0) return false;
                return true;
            }
        }
    }

    public class AIAttackArmy : AttackArmy
    {
        public float waitBeforeLaunching;
        public float timeBetweenUnitSpawns;
        public float squadSpawnDelta;
    }

    public class PlayerAttackArmy : AttackArmy
    {

    }
}