using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CT.Data;

namespace CT.Data.Instance
{
    public class TurretInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.Turret;

        public DefensiveData.VersionData CurrentData => BaseCurrentData as DefensiveData.VersionData;
        public DefensiveData.VersionData NextData => BaseNextData as DefensiveData.VersionData;
        public DefensiveData.VersionData LastData => BaseLastData as DefensiveData.VersionData;

        public TurretInstanceData(int id, DefensiveData data, int level, int tileX, int tileY, bool destroyed)
           : base(id, data, level, tileX, tileY, destroyed)
        {

        }

        public TurretInstanceData(DefensiveData data, int level, int tileX, int tileY)
        : this(0, data, level, tileX, tileY, false)
        {

        }
    }
}
