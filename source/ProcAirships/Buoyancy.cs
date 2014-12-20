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

        //private float buoyancyMultiplicator = 1.0f;

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

            
            

            if (state != StartState.Editor)
            {
                Log.post("force activate envelope");
                part.force_activate();
            }

            

            setupUI();
        }

        

        void Update()
        {
            if(util.editorActive())
            //if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.SPH)
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

            Vector3 GravForce = -FlightGlobals.getGeeForceAtPosition(part.rigidbody.worldCenterOfMass) * (part.mass + part.GetResourceMass());

            guiGravPull = GravForce.magnitude;

            guiBuoyancy = buoyantForce.magnitude - GravForce.magnitude;
        }

        public void updateBuoyancyEditor()
        {
            if (tankVolume > 0.0f)
                buoyantForce = getBuoyancyForce();
            else
                buoyantForce = Vector3.zero;


            double r = Athmosphere.fetch().CurrentBody.Radius;
            double h = EditorController.altitude;
            double mu = Athmosphere.fetch().CurrentBody.gravParameter;

            double geeForce = util.GetGeeAcc(r, h, mu);

            //double geeForce = Athmosphere.fetch().CurrentBody.GeeASL;

            Vector3 GravForce = Vector3.down * (float)geeForce * (part.mass + part.GetResourceMass());//this.vessel.GetTotalMass();

            guiGravPull = GravForce.magnitude;

            guiBuoyancy = buoyantForce.magnitude - GravForce.magnitude;
            

        }

        private void applyBuoyancyForce()
        {
            part.Rigidbody.AddForceAtPosition(buoyantForce, part.rigidbody.worldCenterOfMass, ForceMode.Force);
        }

        public Vector3 getBuoyancyForce()
        {
            float buoyancyMultiplicator = ProcAirships.Instance.buoyancyMultiplicator;
            
            if (util.editorActive())
            {
                //double geeForce = Athmosphere.fetch().CurrentBody.GeeASL;
                double alt = EditorController.altitude;
                double geeAcc = util.GetGeeAcc(
                    Athmosphere.fetch().CurrentBody.Radius,
                    alt,
                    Athmosphere.fetch().CurrentBody.gravParameter);
                Log.post(geeAcc + "m/s²");

                //float airDensity = (float)athmosphere.getAirDensity();
                float airDensity = (float)Athmosphere.fetch().getAirDensity(alt);
                return (-Vector3.down * (float)geeAcc * airDensity * tankVolume) * buoyancyMultiplicator / 1000.0f;
            }
            else
            {
                //float airDensity = (float)athmosphere.getAirDensity();
                float airDensity = (float)Athmosphere.fetch().getAirDensity(part.rigidbody.worldCenterOfMass);
                return (-FlightGlobals.getGeeForceAtPosition(part.rigidbody.worldCenterOfMass) * airDensity * tankVolume) * buoyancyMultiplicator / 1000.0f;
            }
        }

        private void setupUI()
        {
            BaseField field = Fields["guiBuoyancy"];

            if (field != null)
            {
                field.guiActiveEditor = Preferences.showBuoyancyInEditor;
                field.guiActive = Preferences.showBuoyancyInFlight;
            }

            field = Fields["guiGravPull"];
            if (field != null)
            {
                field.guiActiveEditor = Preferences.showGravPullInEditor;
                field.guiActive = Preferences.showGravPullInFlight;
            }


        }

    } // class
} // namespace
