using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Data.Instance
{
    public class LabInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.Lab;

        public string workingOnUnit;
        public GameTime timeLeft;

        static string StartWorkingOn => "\0";
        static GameTime StartTime => GameTime.Zero;

        public LabData.VersionData CurrentData => BaseCurrentData as LabData.VersionData;
        public LabData.VersionData NextData => BaseNextData as LabData.VersionData;
        public LabData.VersionData LastData => BaseLastData as LabData.VersionData;

        public LabInstanceData(int id, LabData data, int level, string workingOnUnit, GameTime timeLeft, int tileX, int tileY, bool destroyed)
        : base(id, data, level, tileX, tileY, destroyed)
        {
            this.workingOnUnit = workingOnUnit;
            this.timeLeft = timeLeft;
        }
        
        public LabInstanceData(LabData data, int level, int tileX, int tileY)
        : this(0, data, level, StartWorkingOn, StartTime, tileX, tileY, false)
        {

        }
    }
}
