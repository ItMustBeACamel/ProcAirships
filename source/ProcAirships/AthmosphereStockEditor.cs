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
            //Debug.LogWarning("GetAirDensity " + FlightGlobals.Bodies[1].bodyName);
            //double pressure = FlightGlobals.Bodies[1].staticPressureASL;
            double pressure = FlightGlobals.getStaticPressure(0, FlightGlobals.Bodies[1]);
            //Debug.LogWarning("GetAirDensity ende " + pressure);
            return FlightGlobals.getAtmDensity(pressure);
           
        }

       
    }
}
