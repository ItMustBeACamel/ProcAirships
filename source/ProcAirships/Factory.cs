using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using ferram4;
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
                Debug.Log("FAR detected.");
                return new AthmosphereFAR();
            }
            else
            {
                Debug.Log("No FAR detected. Fallback to Stock behaviour.");
                if (HighLogic.LoadedScene == GameScenes.EDITOR)
                {
                    return new AthmosphereStockEditor();
                }
                else
                    return new AthmosphereStock();
            }
        }



    }
}
