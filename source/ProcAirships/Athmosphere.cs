using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP;


namespace ProcAirships
{
    public class  Athmosphere
    {

        public double getAirDensity(double altitude)
        {
            updateBody();
            try
            {
                return currentModel.getAirDensity(altitude, currentBody);
            }
            catch (Exception e)
            {
                Log.postException(e);
                return 0;
            }
        }

        public double getAirDensity(Vector3 worldPosition)
        {
            updateBody();
            double density = 0;
            try
            {

                if (util.editorActive())
                {
                    //density = currentModel.getAirDensity(worldPosition.y + EditorController.altitude, currentBody);
                    throw new InvalidOperationException("Do not use in editor!");
                }
                else
                    density = currentModel.getAirDensity(worldPosition, currentBody);
                
            }
            catch(Exception e)
            {
                Log.postException(e);
            }
            //Log.post(density);
            return density;

        }
        
        public double getAirPressure(double altitude)
        {
            updateBody();
            try
            {
                return currentModel.getAirPressure(altitude, currentBody);
            }
            catch (Exception e)
            {
                Log.postException(e);
                return 0;
            }
        }

        public double getAirPressure(Vector3 worldPosition)
        {
            updateBody();

            try
            {

                if (util.editorActive())
                {
                    //if (util.vabActive())
                    //    return currentModel.getAirPressure(worldPosition.y + vabAltitude, currentBody);
                    //else
                    //    return currentModel.getAirPressure(worldPosition.y + sphAltitude, currentBody);
                    //return currentModel.getAirPressure(worldPosition.y + EditorController.altitude, currentBody);
                    throw new InvalidOperationException("Do not use in editor!");
                }
                else
                    return currentModel.getAirPressure(worldPosition, currentBody);

            }
            catch (Exception e)
            {
                Log.postException(e);
                return 0;
            }

        }

        private Athmosphere()
        {
            if (currentModel == null)
            {

                bool isFARLoaded = AssemblyLoader.loadedAssemblies.Any(a => a.assembly.GetName().Name == "FerramAerospaceResearch");
                bool isNEARLoaded = AssemblyLoader.loadedAssemblies.Any(a => a.assembly.GetName().Name == "NEAR");

                //isFARLoaded = false;

                if (isFARLoaded)
                {

                    Log.post("FAR detected", LogLevel.LOG_INFORMATION);

                    AssemblyLoader.LoadedAssembly FAR = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.assembly.GetName().Name == "FerramAerospaceResearch");
                    Log.post("FAR Version: " + FAR.assembly.GetName().Version.ToString(), LogLevel.LOG_INFORMATION);

                    currentModel = new AthmosphereModelFAR();
                    if (!currentModel.init())
                        fallBackToStock();
                }
                else if (isNEARLoaded)
                {
                    Log.post("NEAR detected", LogLevel.LOG_INFORMATION);

                    AssemblyLoader.LoadedAssembly NEAR = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.assembly.GetName().Name == "NEAR");
                    Log.post("NEAR Version: " + NEAR.assembly.GetName().Version.ToString(), LogLevel.LOG_INFORMATION);

                    currentModel = new AthmosphereModelNEAR();
                    if (!currentModel.init())
                        fallBackToStock();

                }
                else
                {
                    Log.post("No FAR or NEAR detected...", LogLevel.LOG_INFORMATION);

                    fallBackToStock();
                }
            }

        }

        private void fallBackToStock()
        {
            Log.post("Falling back to stock athmosphere model.", LogLevel.LOG_INFORMATION);
            currentModel = new AthmosphereModelStock();
            if (!currentModel.init())
                Log.post("Could not init stock athmosphere model", LogLevel.LOG_ERROR);
        }

        private void updateBody()
        {

            if(util.editorActive())
            {
                if (forceBody != null)
                    currentBody = forceBody;
                else
                    currentBody = FlightGlobals.Bodies[1];

                
            }
            else
            {
                currentBody = FlightGlobals.currentMainBody;
            }

            if(currentBody != lastBody)
            {
                lastBody = currentBody;
                Log.post("current Body changed to: " + currentBody.bodyName);
            }
            

        }

        public static Athmosphere fetch()
        {
            if(instance == null)
            {
                instance = new Athmosphere();
                instance.updateBody();
            }

            return instance;
        }

        public CelestialBody ForceBody
        {
            get { return forceBody; }
            set 
            {
                forceBody = value;
                updateBody();
            }
        }

        public CelestialBody CurrentBody
        {
            get { return currentBody; }
        }

        //public double EditorAltitude
        //{
        //    get { return editorAltitude; }
        //    set { editorAltitude = value; }
        //}

        private static Athmosphere instance = null;
        
        public const double vabAltitude = 72.0;
        public const double sphAltitude = 69.0;

        private IAthmosphereModel currentModel = null;
        private CelestialBody currentBody = null;
        private CelestialBody lastBody = null;
        private CelestialBody forceBody = null;

        //private double editorAltitude = 0.0;
        
    }
}
