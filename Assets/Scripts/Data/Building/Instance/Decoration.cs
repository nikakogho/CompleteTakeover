using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CT.Data;
using CT.Data.Instance;

namespace CT.Instance
{
    public class Decoration : Building//<DecorationData, DecorationInstanceData>
    {
        public DecorationData Data => BaseData as DecorationData;
        public DecorationInstanceData InstanceData => BaseInstanceData as DecorationInstanceData;
        public DecorationData.VersionData VersionData => InstanceData.CurrentData;

        public override void OnInteract()
        {
            //may do display of sorts
        }

        public override void OnUpgradeUISelected()
        {
            //nothing
        }
    }
}
