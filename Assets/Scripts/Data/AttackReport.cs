using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    public class AttackReport
    {
        public readonly int ID;
        public BaseData attacked;
        public string attackerUsername;
        public int takenGold;
        public int takenElixir;

        public List<ArmySquad> deployedArmy;

        public AttackReport(int id, BaseData attacked, string attackerUsername, int takenGold, int takenElixir, List<ArmySquad> deployedArmy)
        {
            ID = id;
            this.attacked = attacked;
            this.attackerUsername = attackerUsername;
            this.takenGold = takenGold;
            this.takenElixir = takenElixir;
            this.deployedArmy = deployedArmy;
        }
    }
}