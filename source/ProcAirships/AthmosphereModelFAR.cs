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
        private delegate double getAirDensityAltFunc(CelestialBody body, double altitude, bool oceanSmoothing);
        private delegate double getAirDensityPosFunc(CelestialBody body, Vector3 worldPosition, bool oceanSmoothing);
        //private delegate void updateCurrentActivelBodyFunc(CelestialBody body);

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
                    aeroUtilType.GetMethod("GetCurrentDensity", new Type[] { typeof(CelestialBody), typeof(double), typeof(bool) }));

                getAirDensityPos = (getAirDensityPosFunc)Delegate.CreateDelegate(typeof(getAirDensityPosFunc), null,
                    aeroUtilType.GetMethod("GetCurrentDensity", new Type[] { typeof(CelestialBody), typeof(Vector3), typeof(bool) }));

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
            return getAirDensityAlt(body, altitude, true);
        }

        public double getAirDensity(UnityEngine.Vector3 worldPosition, CelestialBody body)
        {
            // FAR uses a bugged function so this is the workaround until its fixed
            return getAirDensity(FlightGlobals.getAltitudeAtPos(worldPosition, body), body);
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
