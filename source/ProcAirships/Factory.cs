using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;


namespace ProcAirships
{
    public static class Factory
    {

        public static Athmosphere getAthmosphere()
        {
            bool isFARLoaded = AssemblyLoader.loadedAssemblies.Any(a => a.assembly.GetName().Name == "FerramAerospaceResearch");

            isFARLoaded = false; // force stock behaviour because FAR does not seem to work right yet.
            

            if (isFARLoaded)
            {
                Log.post("FAR detected", LogLevel.LOG_INFORMATION);

                AssemblyLoader.LoadedAssembly FAR = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.assembly.GetName().Name == "FerramAerospaceResearch");
                Log.post("FAR Version: " + FAR.assembly.GetName().Version.ToString(), LogLevel.LOG_INFORMATION);

                if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.SPH)
                {
                    Log.post("create Athmosphere for Editor.", LogLevel.LOG_INFORMATION);
                    return new AthmosphereStockEditor();
                }
                else
                {
                    Log.post("create Athmosphere for Flight.", LogLevel.LOG_INFORMATION);
                    return new AthmosphereFAR();
                }
            }
            else
            {
                Log.post("No FAR detected. Fallback to Stock behaviour.", LogLevel.LOG_INFORMATION);
                if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.SPH)
                {
                    
                    Log.post("create Athmosphere for Editor.", LogLevel.LOG_INFORMATION);
                    return new AthmosphereStockEditor();
                }
                else
                {
                    Log.post("create Athmosphere for Flight.", LogLevel.LOG_INFORMATION);
                    return new AthmosphereStock();
                }
            }
        }



    }
}
