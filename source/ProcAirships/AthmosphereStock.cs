using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using KSP;

namespace ProcAirships
{
    public class AthmosphereStock : Athmosphere
    {
        public override double getAirDensity()
        {
            double pressure = FlightGlobals.getStaticPressure();
            return FlightGlobals.getAtmDensity(pressure);
        }

        
    }
}
