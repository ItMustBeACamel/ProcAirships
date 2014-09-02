using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcAirships
{
    class AthmosphereModelStock : IAthmosphereModel
    {

        public double getAirDensity(double altitude, CelestialBody body)
        {
            return FlightGlobals.getAtmDensity(getAirPressure(altitude, body));
        }

        public double getAirDensity(UnityEngine.Vector3 worldPosition, CelestialBody body)
        {
            return FlightGlobals.getAtmDensity(getAirPressure(worldPosition, body));
        }

        public double getAirPressure(double altitude, CelestialBody body)
        {
            return FlightGlobals.getStaticPressure(altitude, body);  
        }

        public double getAirPressure(UnityEngine.Vector3 worldPosition, CelestialBody body)
        {
            return FlightGlobals.getStaticPressure(worldPosition, body);
        }
    }
}
