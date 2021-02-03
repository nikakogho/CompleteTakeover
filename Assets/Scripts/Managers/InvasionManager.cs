using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CT.Data;
using CT.Army;
using CT.UI;
using CT.Manager.Combat;

namespace CT.Manager
{
    public class InvasionManager : MonoBehaviour
    {
        public static InvasionManager instance;

        public Invasion[] campaignInvasions;
        public Invasion[] specialInvasions;

        public GameObject invasionUI;

        public KeyCode invasionKeyCode = KeyCode.Q;

        public string defenseSceneName = "defense";

        public Transform optionsGrid;
        public GameObject optionPrefab;

        GameManager manager;

        List<Invasion> invasions;

        void Awake()
        {
            instance = this;
            invasions = new List<Invasion>(campaignInvasions);
            invasions.AddRange(specialInvasions);
        }

        void Start()
        {
            manager = GameManager.instance;
            GenerateInvasionButtons();
        }

        void GenerateInvasionButtons()
        {
            foreach (var invasion in invasions)
                Instantiate(optionPrefab, optionsGrid).GetComponent<InvasionButton>().Init(this, invasion);
        }

        void Update()
        {
            if (Input.GetKeyDown(invasionKeyCode)) invasionUI.SetActive(!invasionUI.activeSelf);
        }

        public void Launch(Invasion invasion)
        {
            var player = GameManager.Player;
            //var faction = GameManager.Faction;
            //Debug.Log("At launch point data is null: " + (manager._base.Data == null));
            AICombatManager.Init(manager._base.Data, invasion.name, invasion.ToArmy());
            SceneManager.LoadScene(defenseSceneName);
        }

        [System.Serializable]
        public class Invasion
        {
            public string name;
            public float spawnDelta;
            public float squadWait;
            public SquadWave[] waves;

            public Invasion next;

            public AIAttackArmy ToArmy()
            {
                var squads = new List<ArmySquad>();
                foreach (var wave in waves) squads.Add(wave.ToSquad());

                return new AIAttackArmy()
                {
                    squads = squads,
                    squadSpawnDelta = spawnDelta,
                    timeBetweenUnitSpawns = squadWait
                };
            }

            [System.Serializable]
            public class SquadWave
            {
                public UnitData unit;
                public int amount;

                public ArmySquad ToSquad()
                {
                    return new ArmySquad(0, unit.name, amount);
                }
            }
        }
    }
}