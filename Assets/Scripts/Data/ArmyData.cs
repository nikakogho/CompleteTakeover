using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Manager;

namespace CT.Data
{
    public class ArmyData
    {
        public List<ArmySquad> squads;

        public ArmyData()
        {
            squads = new List<ArmySquad>();
        }

        public ArmyData(List<ArmySquad> squads)
        {
            this.squads = squads;
        }
    }

    public class ArmySquad
    {
        public readonly int ID;
        public readonly string name;
        public int amount;

        public readonly UnitData unit;
        
        public ArmySquad(string name, int amount)
        {
            this.name = name;
            this.amount = amount;
            unit = LoadManager.unitsDictionary[name];
        }

        public ArmySquad(int id, string name, int amount)
        :this(name, amount)
        {
            ID = id;
        }

        public ArmySquad(UnitData unit)
        :this(unit.name, 1)
        {

        }

        public ArmySquad(ArmySquad copy)
        : this(copy.ID, copy.name, copy.amount)
        {

        }
    }
}