using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace ProcAirships
{
    class BuoyancyStats : PartModule
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "vessel buoyancy", guiUnits = "kN", guiFormat = "F3")]
        public float vesselBuoyancy;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "vessel net. buoyancy", guiUnits = "kN", guiFormat = "F3")]
        public float vesselNetBuoyancy;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "vessel mass", guiUnits = "t", guiFormat = "F3")]
        public float vesselMass;


        public override void OnStart(StartState state)
        {
            // Add stuff to the log
            print("BuoyancyStats Start");

            if (state != StartState.Editor)
            {
                part.force_activate();
            }
        }

        void Update()
        {
            if (HighLogic.LoadedScene != GameScenes.EDITOR)
                return;
            vesselBuoyancy = 0;
            vesselMass = 0;

            foreach (Part p in EditorLogic.fetch.ship.parts)
            {
                foreach(Buoyancy module in p.Modules.OfType<Buoyancy>())
                {
                    vesselBuoyancy += module.getBuoyancyForce().magnitude;
                }

                if (p.GetComponent<LaunchClamp>() == null)
                {
                    vesselMass += (p.mass + p.GetResourceMass());
                }
            }

            vesselNetBuoyancy = (float)(vesselBuoyancy - 9.8f * vesselMass); 
        }

        public override void OnFixedUpdate()
        {
            vesselBuoyancy = 0;
            vesselMass = 0;

            foreach (Part p in part.vessel.parts)
            {
                foreach (Buoyancy module in p.Modules.OfType<Buoyancy>())
                {
                    vesselBuoyancy += module.getBuoyancyForce().magnitude;
                }

                if (p.GetComponent<LaunchClamp>() == null)
                {
                    vesselMass += (p.mass + p.GetResourceMass());
                }
            }

            vesselNetBuoyancy = (float)(vesselBuoyancy - FlightGlobals.getGeeForceAtPosition(part.rigidbody.worldCenterOfMass).magnitude * vesselMass);
        }

    }
}
