using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP.IO;

namespace ProcAirships
{
    
    public class Preferences
    {

        public static float buoyancyMultiplicator = 3.0f;

        public static bool showVolumeInfoInEditor       = false;
        public static bool showVolumeInfoInFlight       = false;
        public static bool showTemperatureInEditor      = false;
        public static bool showTemperatureInFlight      = true;
        public static bool showAbsPressureInEditor      = false;
        public static bool showAbsPressureInFlight      = false;
        public static bool showBuoyancyInEditor         = false;
        public static bool showBuoyancyInFlight         = false;
        public static bool showVesselBuoyancyInEditor   = false;
        public static bool showVesselBuoyancyInFlight   = false;
        public static bool showVesselMassInEditor       = false;
        public static bool showVesselMassInFlight       = false;
        public static bool showGravPullInEditor = false;
        public static bool showGravPullInFlight = false;
        public static uint debugLevel = (uint)LogLevel.LOG_ALL;
        public static bool pressureDestruction = true;
        public static bool alwaysControllable = false;

    }

    public static class PluginConfigurationExtensions
    {

        public static bool TryGetBool( this PluginConfiguration config, string key, ref bool value)
        {
            return bool.TryParse(config.GetValue(key, value.ToString()), out value);
        }

        public static void SetBool(this PluginConfiguration config, string key, bool value)
        {
            config.SetValue(key, value.ToString());
        }

        public static bool TryGetFloat(this PluginConfiguration config, string key, ref float value)
        {
            return float.TryParse(config.GetValue(key, value.ToString()), out value);
        }

        public static bool TryGetUInt(this PluginConfiguration config, string key, ref uint value)
        {
            return uint.TryParse(config.GetValue(key, value.ToString()), out value);
        }

        /*
        public static void SetFloat(this PluginConfiguration config, string key, float value)
        {
            config.SetValue(key, value.ToString());
        }
         */

        public static void SetVal<T>(this PluginConfiguration config, string key, T value) where T: IConvertible
        {
            config.SetValue(key, value.ToString());
        }

    }


    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class PrefLoader : MonoBehaviour
    {
        public void Awake()
        {

            PluginConfiguration config = PluginConfiguration.CreateForType<PrefLoader>();
 
            config.load();

            
            //float.TryParse(config.GetValue("buoyancyMultiplicator", Preferences.buoyancyMultiplicator.ToString()), out Preferences.buoyancyMultiplicator);

            config.TryGetFloat("buoyancyMultiplicator", ref Preferences.buoyancyMultiplicator);

            config.TryGetBool("showVolumeInfoInEditor", ref Preferences.showVolumeInfoInEditor);
            config.TryGetBool("showVolumeInfoInFlight", ref Preferences.showVolumeInfoInFlight);

            config.TryGetBool("showTemperatureInEditor", ref Preferences.showTemperatureInEditor);
            config.TryGetBool("showTemperatureInFlight", ref Preferences.showTemperatureInFlight);

            config.TryGetBool("showAbsPressureInEditor"   , ref Preferences.showAbsPressureInEditor);
            config.TryGetBool("showAbsPressureInFlight", ref Preferences.showAbsPressureInFlight);

            config.TryGetBool("showBuoyancyInEditor", ref Preferences. showBuoyancyInEditor);
            config.TryGetBool("showBuoyancyInFlight", ref Preferences.showBuoyancyInFlight);

            config.TryGetBool("showVesselBuoyancyInEditor", ref Preferences.showVesselBuoyancyInEditor);
            config.TryGetBool("showVesselBuoyancyInFlight", ref Preferences.showVesselBuoyancyInFlight);

            config.TryGetBool("showVesselMassInEditor", ref Preferences.showVesselMassInEditor);
            config.TryGetBool("showVesselMassInFlight", ref Preferences.showVesselMassInFlight);

            config.TryGetBool("showGravPullInEditor", ref Preferences.showGravPullInEditor);
            config.TryGetBool("showGravPullInFlight", ref Preferences.showGravPullInFlight);

            config.TryGetUInt("debugLevel", ref Preferences.debugLevel);

            config.TryGetBool("pressureDestruction", ref Preferences.pressureDestruction);
            config.TryGetBool("alwaysControllable", ref Preferences.alwaysControllable);

        }

        void OnDestroy()
        {
            PluginConfiguration config = PluginConfiguration.CreateForType<PrefLoader>();
          
            config.SetVal("buoyancyMultiplicator", Preferences.buoyancyMultiplicator);

            config.SetVal("showVolumeInfoInEditor", Preferences.showVolumeInfoInEditor);
            config.SetVal("showVolumeInfoInFlight", Preferences.showVolumeInfoInFlight);
            config.SetVal("showTemperatureInEditor", Preferences.showTemperatureInEditor);
            config.SetVal("showTemperatureInFlight", Preferences.showTemperatureInFlight);
            config.SetVal("showAbsPressureInEditor", Preferences.showAbsPressureInEditor);
            config.SetVal("showAbsPressureInFlight", Preferences.showAbsPressureInFlight);
            config.SetVal("showBuoyancyInEditor", Preferences.showBuoyancyInEditor);
            config.SetVal("showBuoyancyInFlight", Preferences.showBuoyancyInFlight);
            config.SetVal("showVesselBuoyancyInEditor", Preferences.showVesselBuoyancyInEditor);
            config.SetVal("showVesselBuoyancyInFlight", Preferences.showVesselBuoyancyInFlight);
            config.SetVal("showVesselMassInEditor", Preferences.showVesselMassInEditor);
            config.SetVal("showVesselMassInFlight", Preferences.showVesselMassInFlight);
            config.SetVal("showGravPullInEditor", Preferences.showGravPullInEditor);
            config.SetVal("showGravPullInFlight", Preferences.showGravPullInFlight);
            config.SetVal("debugLevel", Preferences.debugLevel);
            config.SetVal("pressureDestruction", Preferences.pressureDestruction);
            config.SetVal("alwaysControllable", Preferences.alwaysControllable);

            config.save();
      
        }

    }
}
