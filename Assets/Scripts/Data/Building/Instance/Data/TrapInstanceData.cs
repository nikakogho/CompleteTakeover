using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Data.Instance
{
    public class TrapInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.Trap;

        public TrapData.VersionData CurrentData => BaseCurrentData as TrapData.VersionData;
        public TrapData.VersionData NextData => BaseNextData as TrapData.VersionData;
        public TrapData.VersionData LastData => BaseLastData as TrapData.VersionData;

        public TrapInstanceData(int id, TrapData data, int level, int tileX, int tileY, bool destroyed)
        : base(id, data, level, tileX, tileY, destroyed)
        {
            
        }

        public TrapInstanceData(TrapData data, int level, int tileX, int tileY)
        : this(0, data, level, tileX, tileY, false)
        {

        }
    }
}
