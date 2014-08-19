using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;

namespace ProcAirships
{
    public class AthmosphereStock : Athmosphere
    {
        public override double getAirDensity()
        {
            //Log.post("Getting air density from stock athmosphere");
            double pressure = FlightGlobals.getStaticPressure();
            return FlightGlobals.getAtmDensity(pressure);
        }  
    }
}
