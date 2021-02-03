using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    public class DefenseReport
    {
        public readonly int ID;
        public BaseData attacked;
        public string invasionName;
        public int lostGold;
        public int lostElixir;

        public List<ArmySquad> deployedArmy;

        public DefenseReport(int id, BaseData attacked, string invasionName, int lostGold, int lostElixir, List<ArmySquad> deployedArmy)
        {
            ID = id;
            this.attacked = attacked;
            this.invasionName = invasionName;
            this.lostGold = lostGold;
            this.lostElixir = lostElixir;
            this.deployedArmy = deployedArmy;
        }
    }
}