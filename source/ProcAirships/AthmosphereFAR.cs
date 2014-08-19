using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace ProcAirships
{
    public class AthmosphereFAR : Athmosphere
    {

        public override double getAirDensity()
        {
            return ferram4.FARAPI.GetActiveControlSys_AirDensity();
        }
    }
}
