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
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static bool Toggle(this bool val)
        {
            return val == true ? false : true;
        }

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

        public static double pascalToBar(double p)
        {
            return p / 101325.0d;
        }

        public static double barToPascal(double p)
        {
            return p * 101325.0d;
        }

        public static double getPressure(double m, double M, double T, double V) // gas a(m)ount in kg, (M)olar mass in g/mol., (T)emperature in Kelvin, (V)olume in m³
        {
            //LiftingGas gas = getCurrentLiftingGas();

            double n = m / (M / 1000.0d);
            double R = util.GasConstant;
            //double T = util.celsiusToKelvin(getTemperature());
            //double V = envelopeVolumeNet;

            double p = (n * R * T) / V;

            return pascalToBar(p); // pressure in bar
        }

        public static double getGasAmount(double p, double M, double T, double V) // (p)ressure in bar, (M)olar mass in g/mol., (T)emperature in Kelvin, (V)olume in m³
        {

            //LiftingGas gas = getCurrentLiftingGas();
            double m;
            p = barToPascal(p); // bar to pascal
            M /= 1000.0d;
            //double V = envelopeVolumeNet;
            //double T = util.celsiusToKelvin(getTemperature());
            double R = util.GasConstant;

            m = M * ((p * V) / (R * T));

            return m;
        }


        public const double GasConstant = 8.3144621d;
    }
}
