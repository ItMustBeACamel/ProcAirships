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
            bool isFarLoaded = AssemblyLoader.loadedAssemblies.Any(a => a.assembly.GetName().Name == "FerramAerospaceResearch");

            if (isFarLoaded)
            {
                Log.post("FAR detected", LogLevel.LOG_INFORMATION);
                return new AthmosphereFAR();
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
