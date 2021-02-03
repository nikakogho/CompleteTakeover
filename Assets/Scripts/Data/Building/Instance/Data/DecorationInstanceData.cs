using UnityEngine;
using System.Collections.Generic;
using CT.Army;

namespace CT.Data.Instance
{
    public class DecorationInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.Decoration;

        public DecorationData Data => data as DecorationData;

        public DecorationData.VersionData CurrentData => BaseCurrentData as DecorationData.VersionData;
        public DecorationData.VersionData NextData => BaseNextData as DecorationData.VersionData;
        public DecorationData.VersionData LastData => BaseLastData as DecorationData.VersionData;

        public DecorationInstanceData(int id, DecorationData data, int level, int tileX, int tileY, bool destroyed)
        : base(id, data, level, tileX, tileY, destroyed)
        {

        }
    }
}