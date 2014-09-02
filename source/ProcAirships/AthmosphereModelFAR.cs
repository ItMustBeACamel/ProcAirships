using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcAirships
{
    class AthmosphereModelFAR : IAthmosphereModel
    {
        public double getAirDensity(double altitude, CelestialBody body)
        {
            return ferram4.FARAeroUtil.GetCurrentDensity(body, altitude);
        }

        public double getAirDensity(UnityEngine.Vector3 worldPosition, CelestialBody body)
        {

            //return ferram4.FARAeroUtil.GetCurrentDensity(body, worldPosition);
            // FAR uses a bugged function so this is the workaround until its fixed
            return getAirDensity(FlightGlobals.getAltitudeAtPos(worldPosition, body), body);
        }

        // FAR does not seem to have a function for this so I copied this out of it's AeroUtil class
        public double getAirPressure(double altitude, CelestialBody body)
        {
            ferram4.FARAeroUtil.UpdateCurrentActiveBody(body);

            if (altitude > body.maxAtmosphereAltitude)
                return 0;

            //double temp = Math.Max(0.1, ferram4.FARAeroUtil.currentBodyTemp + FlightGlobals.getExternalTemperature((float)altitude, body));

            double pressure = FlightGlobals.getStaticPressure(altitude, body);
            if (pressure > 0)
                pressure = (pressure - ferram4.FARAeroUtil.currentBodyAtmPressureOffset); 

            return pressure;
        }

        // FAR does not seem to have a function for this so I copied this out of it's AeroUtil class
        public double getAirPressure(UnityEngine.Vector3 worldPosition, CelestialBody body)
        {
            ferram4.FARAeroUtil.UpdateCurrentActiveBody(body);

            //double temp = Math.Max(0.1, ferram4.FARAeroUtil.currentBodyTemp + FlightGlobals.getExternalTemperature(worldPosition));

            double pressure = FlightGlobals.getStaticPressure(worldPosition, body);
            if (pressure > 0)
                pressure = (pressure - ferram4.FARAeroUtil.currentBodyAtmPressureOffset);

            return pressure;
        }
    }
}
