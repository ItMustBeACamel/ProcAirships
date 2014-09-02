using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships
{
    public class AthmosphereFAREditor :  Athmosphere
    {
        public override double getAirDensity()
        {

            double density = 0;
            try
            {
                if (HighLogic.LoadedScene == GameScenes.EDITOR)
                    density = ferram4.FARAeroUtil.GetCurrentDensity(FlightGlobals.Bodies[1], 72.5);
                else
                    density = ferram4.FARAeroUtil.GetCurrentDensity(FlightGlobals.Bodies[1], 69.0);
            }
            catch (Exception e)
            {
                Log.postException(e);
                return 0;
            }

            return density;
        }// getAirDensity

        public override double getAirPressure()
        {
            double pressure = 0.0;
            float altitude;

            if (HighLogic.LoadedScene == GameScenes.EDITOR)
                altitude = 72.5f;
            else
                altitude =  69.0f;


            try
            {
                ferram4.FARAeroUtil.UpdateCurrentActiveBody(FlightGlobals.Bodies[1]);

                double temp = Math.Max(0.1, ferram4.FARAeroUtil.currentBodyTemp + FlightGlobals.getExternalTemperature(altitude, FlightGlobals.Bodies[1]));

                pressure = FlightGlobals.getStaticPressure(altitude, FlightGlobals.Bodies[1]);
                if (pressure > 0)
                    pressure = (pressure - ferram4.FARAeroUtil.currentBodyAtmPressureOffset);   
            }
            catch (Exception e)
            {
                Log.postException(e);
                return 0;
            }

            return pressure;
        }


        public override double getAirDensity(CelestialBody body, Vector3 worldLocation)
        {
            /*
            double density = 0.0;
            try
            {
                density = ferram4.FARAeroUtil.GetCurrentDensity(body, worldLocation);
            }
            catch (Exception e)
            {
                Log.postException(e);
                return 0;
            }

            return density;
             */
            return getAirDensity();

        }

        public override double getAirPressure(CelestialBody body, Vector3 worldLocation)
        {
            /*
            double pressure = 0.0;
            try
            {
                ferram4.FARAeroUtil.UpdateCurrentActiveBody(body);

                double temp = Math.Max(0.1, ferram4.FARAeroUtil.currentBodyTemp + FlightGlobals.getExternalTemperature(worldLocation));

                pressure = FlightGlobals.getStaticPressure(worldLocation, body);
                if (pressure > 0)
                    pressure = (pressure - ferram4.FARAeroUtil.currentBodyAtmPressureOffset);     //Need to convert atm to Pa
            }
            catch (Exception e)
            {
                Log.postException(e);
                return 0;
            }

            return pressure;
             */
            return getAirPressure();

        }

    }
}
