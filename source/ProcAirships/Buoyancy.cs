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

        private float buoyancyMultiplicator = 1.0f;

        public override void OnActive()
        {
            Log.post(this.ClassName + " OnActive-callback: ");
        }

        public override void OnAwake()
        {
            Log.post(this.ClassName + " OnAwake-callback: ");

            base.OnAwake();
            PartMessageService.Register(this);
        }

        public override void OnStart(StartState state)
        {
            // Add stuff to the log
            Log.post(this.ClassName + " OnStart-callback: " + state.ToString());

            
            athmosphere = Factory.getAthmosphere();

            if (state != StartState.Editor)
            {
                Log.post("force activate envelope");
                part.force_activate();
            }

            Log.post("get configuration. memo for me: PUT CONFIG STUFF IN A DEDICATED CLASS LATER.");
            ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes("PROC_AIRSHIPS_CONFIG");

            if (nodes.GetLength(0) == 0)
            {
                Debug.LogWarning("'PROC_AIRSHIPS_CONFIG' not detected. Using standard values.");
                return;
            }

            Config config = new Config();
            config.Load(nodes[0]);


            buoyancyMultiplicator = config.buoyancyMultiplicator;
            Log.post("Buoyancy Multiplicator: " + buoyancyMultiplicator, LogLevel.LOG_INFORMATION);
        }


        [PartMessageListener(typeof(PartVolumeChanged), scenes: ~GameSceneFilter.Flight)]
        public void ChangeVolume(string volumeName, float volume)
        {
            Log.post("received ChangeVolume message for " + volumeName + " Volume: " + volume);
            
            if (volumeName != PartVolumes.Tankage.ToString())
                return;

            if (volume <= 0f)
                throw new ArgumentOutOfRangeException("volume");
            Log.post("Buoyancy changed volume to" + volume, LogLevel.LOG_INFORMATION);
            tankVolume = volume;
        }

        public override void OnFixedUpdate()
        {
            
          
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

    }
}
