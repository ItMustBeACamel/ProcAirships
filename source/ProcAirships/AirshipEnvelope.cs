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
        /*
        [KSPEvent(guiActive = true, guiName = "toggle gas dumping")]
        public void toggleGasDump()
        {
            if (dumpingGas == true) dumpingGas = false; else dumpingGas = true;
        }
         */

        //[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Volume", guiUnits = "m³", guiFormat = "F2")]
        public float tankVolume = 0;

        //[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "dry mass", guiUnits = "t", guiFormat = "F2")]
        //public float guiDryMass = 0;

        
       
        public override void OnAwake()
        {
            Debug.Log("Envelope Awake");

            base.OnAwake();
            PartMessageService.Register(this);   
        }

        public override void OnStart(StartState state)
        {
            // Add stuff to the log
            print("Envelope Start");
        }

        public override void OnFixedUpdate()
        {
         
            PartResource air = part.Resources.Get("Air".GetHashCode());
            if (air == null)
            {
                Debug.LogError("no air on part");
                return;
            }
            else
            {
                PartResource hydrogen = part.Resources.Get("Hydrogen".GetHashCode());

                if(hydrogen == null)
                {
                    Debug.LogError("no Hydrogen on part");
                    return;
                }

                air.maxAmount = hydrogen.maxAmount - hydrogen.amount;
                air.amount = air.maxAmount;
            }
        }



        [PartMessageListener(typeof(PartVolumeChanged), scenes: ~GameSceneFilter.Flight)]
        public void ChangeVolume(string volumeName, float volume)
        {
            
            if (volumeName != PartVolumes.Tankage.ToString())
                return;

            if (volume <= 0f)
                throw new ArgumentOutOfRangeException("volume");
            Debug.Log("Envelope changed volume to" + volume);
            tankVolume = volume;
        }

        [PartMessageListener(typeof(PartResourceInitialAmountChanged), scenes: GameSceneFilter.Flight)]
        public void ChangeInitResource(PartResource resource, double amount)
        {
            Debug.Log("Envelope changed init resource " + resource.resourceName + " to " + amount);
        }
         
    }
}
