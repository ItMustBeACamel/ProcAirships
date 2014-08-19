using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;
using KSPAPIExtensions;
using KSPAPIExtensions.PartMessage;

namespace ProcAirships
{
    public class AirshipEnvelope : PartModule
    {

        //[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Volume", guiUnits = "m³", guiFormat = "F2")]
        public float tankVolume = 0;

        //[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "dry mass", guiUnits = "t", guiFormat = "F2")]
        //public float guiDryMass = 0;

        
       
        public override void OnAwake()
        {
            Log.post(this.ClassName + " OnAwake-callback: ");

            base.OnAwake();
            PartMessageService.Register(this);   
        }

        public override void OnStart(StartState state)
        {
            Log.post(this.ClassName + " OnStart-callback: " + state.ToString());
        }

        public override void OnFixedUpdate()
        {
         
            PartResource air = part.Resources.Get("Air".GetHashCode());
            if (air == null)
            {
                Log.post("No Air resource on part", LogLevel.LOG_ERROR);
                return;
            }
            else
            {
                PartResource hydrogen = part.Resources.Get("Hydrogen".GetHashCode());

                if(hydrogen == null)
                {
                    Log.post("No hydrogen resource on part", LogLevel.LOG_ERROR);
                    return;
                }

                air.maxAmount = hydrogen.maxAmount - hydrogen.amount;
                air.amount = air.maxAmount;
            }
        }



        [PartMessageListener(typeof(PartVolumeChanged), scenes: ~GameSceneFilter.Flight)]
        public void ChangeVolume(string volumeName, float volume)
        {
            Log.post("received ChangeVolume message for " + volumeName + " Volume: " + volume);
            if (volumeName != PartVolumes.Tankage.ToString())
                return;

            if (volume <= 0f)
                throw new ArgumentOutOfRangeException("volume");
            Log.post("tank Volume Changed to " + volume, LogLevel.LOG_INFORMATION);
            tankVolume = volume;
        }

        [PartMessageListener(typeof(PartResourceInitialAmountChanged), scenes: GameSceneFilter.Flight)]
        public void ChangeInitResource(PartResource resource, double amount)
        {
            Log.post("Envelope changed init resource " + resource.resourceName + " to " + amount);
        }
         
    }
}
