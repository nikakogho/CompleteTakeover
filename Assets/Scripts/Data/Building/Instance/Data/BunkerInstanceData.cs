using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Instance
{
    public class BunkerInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.Bunker;

        public List<ArmySquad> squads;

        static List<ArmySquad> StartSquads => new List<ArmySquad>();

        public BunkerData.VersionData CurrentData => BaseCurrentData as BunkerData.VersionData;
        public BunkerData.VersionData NextData => BaseNextData as BunkerData.VersionData;
        public BunkerData.VersionData LastData => BaseLastData as BunkerData.VersionData;

        public BunkerInstanceData(int id, BunkerData data, int level, List<ArmySquad> squads, int tileX, int tileY, bool destroyed)
        :base(id, data, level, tileX, tileY, destroyed)
        {
            this.squads = squads;
        }

        public BunkerInstanceData(BunkerData data, int level, int tileX, int tileY)
        : this(0, data, level, StartSquads, tileX, tileY, false)
        {

        }
    }
}