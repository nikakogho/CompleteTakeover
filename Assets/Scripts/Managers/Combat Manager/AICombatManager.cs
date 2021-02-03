using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CT.Data;
using CT.Army;
using CT.Net;

namespace CT.Manager.Combat
{
    public class AICombatManager : CombatManager<AICombatManager, AIAttackArmy>
    {
        public Transform deployParentsParent;

        static string invasionName;

        List<Transform> deploySpots;

        protected override void OnAttackOver(bool destroyedWholeBase = false)
        {
            var destroyedBuildings = DefenderBase.DestroyedBuildings.Select(b => b.BaseInstanceData.ID);
            var army = deployedArmy.Select(squad => new ArmySquad(squad.Key.name, squad.Value));
            var robbedMines = DefenderBase.RobbedMines;
            var robbedStorages = DefenderBase.RobbedStorages;
            ClientSend.ProcessDefense(defender.ID, invasionName, AmassedGold, AmassedElixir, 
                army, destroyedBuildings, robbedMines, robbedStorages);
        }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            deploySpots = new List<Transform>();
            for (int i = 0; i < deployParentsParent.childCount; i++)
                for (int j = 0; j < deployParentsParent.GetChild(i).childCount; j++)
                    deploySpots.Add(deployParentsParent.GetChild(i).GetChild(j));
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(DeployRoutine());
        }

        void Deploy(ArmySquad squad, Transform spot)
        {
            Deploy(squad, spot.position, spot.rotation);
        }

        IEnumerator DeployRoutine()
        {
            var possibleSpots = new List<Transform>(deploySpots);
            yield return new WaitForSeconds(army.waitBeforeLaunching);
            foreach (var squad in army.squads)
            {
                while (squad.amount > 0)
                {
                    if (possibleSpots.Count == 0) possibleSpots.AddRange(deploySpots);
                    int index = Random.Range(0, possibleSpots.Count);
                    var spot = possibleSpots[index];
                    possibleSpots.RemoveAt(index);
                    Deploy(squad, spot);
                    yield return new WaitForSeconds(army.timeBetweenUnitSpawns);
                }
                yield return new WaitForSeconds(army.squadSpawnDelta);
            }
        }

        public static void Init(BaseData _defender, string _invasionName, AIAttackArmy _army)
        {
            defender = _defender;
            invasionName = _invasionName;
            army = _army;
            //Debug.Log("Init with defender null: " + (defender == null));
        }
    }
}