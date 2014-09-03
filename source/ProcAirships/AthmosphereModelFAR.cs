using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace ProcAirships
{
    class AthmosphereModelFAR : IAthmosphereModel
    {
        private delegate double getAirDensityAltFunc(CelestialBody body, double altitude);
        private delegate double getAirDensityPosFunc(CelestialBody body, Vector3 worldPosition);
        private delegate void updateCurrentActivelBodyFunc(CelestialBody body);

        private getAirDensityAltFunc getAirDensityAlt;
        private getAirDensityPosFunc getAirDensityPos;
        //private updateCurrentActivelBodyFunc updateCurrentActiveBody;

  

        private Type aeroUtilType;

        public bool init()
        {
            if (!AssemblyLoader.loadedAssemblies.Any(a => a.assembly.GetName().Name == "FerramAerospaceResearch"))
                return false;

            AssemblyLoader.LoadedAssembly far = AssemblyLoader.loadedAssemblies.First(a => a.assembly.GetName().Name == "FerramAerospaceResearch");

            /*
            foreach(Type t in far.assembly.GetExportedTypes())
            {
                Log.post("--exported Type from FAR--");
                Log.post("name: " + t.Name);
                Log.post("full name: " + t.FullName);
                Log.post("is Class: " + t.IsClass);
                Log.post("---------------------------");
            }
             */

            aeroUtilType = far.assembly.GetExportedTypes().First(x => x.Name == "FARAeroUtil");

            if (aeroUtilType == null)
            {
                Log.post("could not find class FARAeroUtil");
                return false;
            }

            // init delegates
            try
            {

                getAirDensityAlt = (getAirDensityAltFunc)Delegate.CreateDelegate(typeof(getAirDensityAltFunc), null,
                    aeroUtilType.GetMethod("GetCurrentDensity", new Type[] { typeof(CelestialBody), typeof(double) }));

                getAirDensityPos = (getAirDensityPosFunc)Delegate.CreateDelegate(typeof(getAirDensityPosFunc), null,
                    aeroUtilType.GetMethod("GetCurrentDensity", new Type[] { typeof(CelestialBody), typeof(Vector3) }));

                //updateCurrentActiveBody = (updateCurrentActivelBodyFunc)Delegate.CreateDelegate(typeof(updateCurrentActivelBodyFunc), null,
                //    aeroUtilType.GetMethod("UpdateCurrentActiveBody", new Type[] { typeof(CelestialBody)}));

                
               

            }
            catch (Exception e)
            {
                Log.postException("error initializing FAR athmosphere model", e);
                return false;
            }

            return true;
        }


        public double getAirDensity(double altitude, CelestialBody body)
        {
            //return ferram4.FARAeroUtil.GetCurrentDensity(body, altitude);

            return getAirDensityAlt(body, altitude);
        }

        public double getAirDensity(UnityEngine.Vector3 worldPosition, CelestialBody body)
        {
  
            //return getAirDensityPos(body, worldPosition);
            // FAR uses a bugged function so this is the workaround until its fixed
            return getAirDensity(FlightGlobals.getAltitudeAtPos(worldPosition, body), body);
        }

        // FAR does not seem to have a function for this so I copied this out of it's AeroUtil class
        public double getAirPressure(double altitude, CelestialBody body)
        {
            /*
            //ferram4.FARAeroUtil.UpdateCurrentActiveBody(body);
            updateCurrentActiveBody(body);

            if (altitude > body.maxAtmosphereAltitude)
                return 0;

            //double temp = Math.Max(0.1, ferram4.FARAeroUtil.currentBodyTemp + FlightGlobals.getExternalTemperature((float)altitude, body));
            double currentBodyAtmPressureOffset = (double)aeroUtilType.GetField("currentBodyAtmPressureOffset").GetValue(null);
            

            double pressure = FlightGlobals.getStaticPressure(altitude, body);
            if (pressure > 0)
                //pressure = (pressure - ferram4.FARAeroUtil.currentBodyAtmPressureOffset);
                pressure = (pressure - currentBodyAtmPressureOffset);

            return pressure;
             * 
             */

            return FlightGlobals.getStaticPressure(altitude, body);
        }

        // FAR does not seem to have a function for this so I copied this out of it's AeroUtil class
        public double getAirPressure(UnityEngine.Vector3 worldPosition, CelestialBody body)
        {
            /*
            //ferram4.FARAeroUtil.UpdateCurrentActiveBody(body);
            updateCurrentActiveBody(body);

            //double temp = Math.Max(0.1, ferram4.FARAeroUtil.currentBodyTemp + FlightGlobals.getExternalTemperature(worldPosition));

            double currentBodyAtmPressureOffset = (double)aeroUtilType.GetField("currentBodyAtmPressureOffset").GetValue(null);
            

            double pressure = FlightGlobals.getStaticPressure(worldPosition, body);
            if (pressure > 0)
                //pressure = (pressure - ferram4.FARAeroUtil.currentBodyAtmPressureOffset);
                pressure = (pressure - currentBodyAtmPressureOffset);

            return pressure;
             */
            return FlightGlobals.getStaticPressure(worldPosition, body);
        }

        
    }
}
