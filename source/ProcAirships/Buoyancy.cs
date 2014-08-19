using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;
using KSPAPIExtensions;
using KSPAPIExtensions.PartMessage;

namespace ProcAirships
{
    public class Buoyancy : PartModule
    {
        private Athmosphere athmosphere = null;

        private Vector3 buoyantForce;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = false, guiName = "Volume", guiUnits = "m³", guiFormat = "F2")]
        public float tankVolume = 0;

        [KSPField(guiActive = true, guiName = "Buoyancy", guiUnits = "kN", guiFormat = "F2")]
        public float guiBuoyancy = 0;

        [KSPField(guiActive = true, guiName = "Grav Pull", guiUnits = "kN", guiFormat = "F2")]
        public float guiGravPull = 0;

        [KSPField(guiActive = true, guiName = "temperature", guiUnits = "celsius", guiFormat = "F2")]
        public float guiTemperature = 0;

        private float buoyancyMultiplicator = 1.0f;

        public override void OnActive()
        {
            Debug.Log("Buoyancy Active");
        }

        public override void OnAwake()
        {
            Debug.Log("Buoyancy Awake");

            base.OnAwake();
            PartMessageService.Register(this);
        }

        public override void OnStart(StartState state)
        {
            // Add stuff to the log
            print("Buoyancy Start");

            
            athmosphere = Factory.getAthmosphere();

            if (state != StartState.Editor)
            {
                part.force_activate();
                print("Buoyancy force activate");

            }

            ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes("PROC_AIRSHIPS_CONFIG");

            if (nodes.GetLength(0) == 0)
            {
                Debug.LogWarning("'PROC_AIRSHIPS_CONFIG' not detected. Using standard values.");
                return;
            }

            Config config = new Config();
            config.Load(nodes[0]);


            buoyancyMultiplicator = config.buoyancyMultiplicator;
            Debug.Log("buoyancy multiplicator: " + buoyancyMultiplicator);
        }


        [PartMessageListener(typeof(PartVolumeChanged), scenes: ~GameSceneFilter.Flight)]
        public void ChangeVolume(string volumeName, float volume)
        {
            Debug.Log("Buoyancy received volume change message" + volume);
            if (volumeName != PartVolumes.Tankage.ToString())
                return;

            if (volume <= 0f)
                throw new ArgumentOutOfRangeException("volume");
            Debug.Log("Buoyancy changed volume to" + volume);
            tankVolume = volume;
        }

        public override void OnUpdate()
        {
          
           

        }



        public override void OnFixedUpdate()
        {
            //float airDensity = 0.0f;
            
            

            //airDensity = (float)athmosphere.getAirDensity();

            guiTemperature = part.temperature;

            buoyantForce = getBuoyancyForce();

            Vector3 GravForce = -FlightGlobals.getGeeForceAtPosition(part.rigidbody.worldCenterOfMass) * (part.mass + part.GetResourceMass());//this.vessel.GetTotalMass();


            guiBuoyancy = buoyantForce.magnitude - GravForce.magnitude;
            guiGravPull = GravForce.magnitude;

            part.Rigidbody.AddForceAtPosition(buoyantForce, part.rigidbody.worldCenterOfMass, ForceMode.Force);

            
        }

        public Vector3 getBuoyancyForce()
        {
            float airDensity = (float)athmosphere.getAirDensity();
            return (-FlightGlobals.getGeeForceAtPosition(part.rigidbody.worldCenterOfMass) * airDensity * tankVolume) * buoyancyMultiplicator / 1000.0f;
        }

        /*
        public float getBuoyancy()
        {
            if()
        }
         */

    }
}
