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
        //private Athmosphere athmosphere = null;

        private Vector3 buoyantForce;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Volume", guiUnits = "m³", guiFormat = "F2")]
        public float tankVolume = 0;

        [KSPField(guiActive = true, guiActiveEditor=true, guiName = "Buoyancy", guiUnits = "kN", guiFormat = "F2")]
        public float guiBuoyancy = 0;

        [KSPField(guiActive = true, guiActiveEditor=true, guiName = "Grav Pull", guiUnits = "kN", guiFormat = "F2")]
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

            
            //athmosphere = Factory.getAthmosphere();

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
            //Log.post("received ChangeVolume message for " + volumeName + " Volume: " + volume);
            
            //if (volumeName != PartVolumes.Tankage.ToString())
            //    return;

            //if (volume <= 0f)
            //    throw new ArgumentOutOfRangeException("volume");
            //Log.post("Buoyancy changed volume to" + volume, LogLevel.LOG_INFORMATION);
            //tankVolume = volume;
        }

        void Update()
        {

            if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.SPH)
            {
                updateVolume();
                updateBuoyancyEditor();
            }
        }

        public override void OnFixedUpdate()
        {
            updateVolume();
            updateBuoyancy();

            applyBuoyancyForce();
            
  
        }

        public void updateVolume()
        {
            tankVolume = 0.0f;
            foreach (AirshipEnvelope envelope in part.Modules.OfType<AirshipEnvelope>())
            {
                tankVolume += (float)envelope.EnvelopeVolumeNet;
            }
        }

        public void updateBuoyancy()
        {
            if (tankVolume > 0.0f)
                buoyantForce = getBuoyancyForce();
            else
                buoyantForce = Vector3.zero;

            Vector3 GravForce = -FlightGlobals.getGeeForceAtPosition(part.rigidbody.worldCenterOfMass) * (part.mass + part.GetResourceMass());//this.vessel.GetTotalMass();

            guiGravPull = GravForce.magnitude;

            guiBuoyancy = buoyantForce.magnitude - GravForce.magnitude;
        }

        public void updateBuoyancyEditor()
        {
            if (tankVolume > 0.0f)
                buoyantForce = getBuoyancyForce();
            else
                buoyantForce = Vector3.zero;

            Vector3 GravForce = Vector3.down * 9.8f * (part.mass + part.GetResourceMass());//this.vessel.GetTotalMass();

            guiGravPull = GravForce.magnitude;

            guiBuoyancy = buoyantForce.magnitude - GravForce.magnitude;
            

        }

        private void applyBuoyancyForce()
        {
            part.Rigidbody.AddForceAtPosition(buoyantForce, part.rigidbody.worldCenterOfMass, ForceMode.Force);
        }

        public Vector3 getBuoyancyForce()
        {
            if (util.editorActive())
            {
                //float airDensity = (float)athmosphere.getAirDensity();
                float airDensity = (float)Athmosphere.fetch().getAirDensity(part.rigidbody.worldCenterOfMass);
                return (-Vector3.down * 9.8f * airDensity * tankVolume) * buoyancyMultiplicator / 1000.0f;
            }
            else
            {
                //float airDensity = (float)athmosphere.getAirDensity();
                float airDensity = (float)Athmosphere.fetch().getAirDensity(part.rigidbody.worldCenterOfMass);
                return (-FlightGlobals.getGeeForceAtPosition(part.rigidbody.worldCenterOfMass) * airDensity * tankVolume) * buoyancyMultiplicator / 1000.0f;
            }
        }

    }
}
