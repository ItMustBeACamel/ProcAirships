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
            double pressure = 0;
            if(HighLogic.LoadedScene == GameScenes.EDITOR)
                pressure = FlightGlobals.getStaticPressure(72.5, FlightGlobals.Bodies[1]);
            else
                pressure = FlightGlobals.getStaticPressure(69.0, FlightGlobals.Bodies[1]);

            return FlightGlobals.getAtmDensity(pressure);
        }

       
    }
}
