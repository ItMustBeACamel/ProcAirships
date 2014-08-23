using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace ProcAirships
{
    public static class util
    {
        public static bool editorActive()
        {
            if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.SPH)
                return true;
            else
                return false;   
        }

        public static double celsiusToKelvin(double T)
        {
            return T + 273.15d;
        }

        public static double kelvinToCelsius(double T)
        {
            return T - 273.15d;
        }

        public const double GasConstant = 8.3144621d;
    }
}
