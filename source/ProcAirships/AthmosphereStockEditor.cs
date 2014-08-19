using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;


namespace ProcAirships
{
    class AthmosphereStockEditor : Athmosphere
    {

        public override double getAirDensity()
        {
            double pressure = FlightGlobals.getStaticPressure(73.0, FlightGlobals.Bodies[1]); 
            return FlightGlobals.getAtmDensity(pressure);
        }

       
    }
}
