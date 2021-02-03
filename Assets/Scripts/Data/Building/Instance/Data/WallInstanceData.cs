using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Data.Instance
{
    public class WallInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.Wall;

        public WallData.VersionData CurrentData => BaseCurrentData as WallData.VersionData;
        public WallData.VersionData NextData => BaseNextData as WallData.VersionData;
        public WallData.VersionData LastData => BaseLastData as WallData.VersionData;

        public WallInstanceData(int id, WallData data, int level, int tileX, int tileY, bool destroyed)
        : base(id, data, level, tileX, tileY, destroyed)
        {

        }

        public WallInstanceData(WallData data, int level, int tileX, int tileY)
        : this(0, data, level, tileX, tileY, false)
        {
            
        }
    }
}
