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
    public class AirshipDrainResource : PartModule
    {

#region Fields

        [KSPField]
        public string displayName;

        [KSPField(isPersistant=true, guiName="resource", guiActive=false, guiActiveEditor=false, category="dump resource")]
        public string resourceName; // the name of the dumpable resource

        [KSPField]
        public float minDumpRate;

        [KSPField]
        public float maxDumpRate;

        [KSPField(isPersistant = true, guiName = "dump rate", guiActive = true, guiActiveEditor = false, category = "dump resource", guiFormat="F4", guiUnits="L/s"),
            UI_FloatEdit(scene = UI_Scene.Flight, incrementLarge = 100.0f, incrementSmall = 10.0f, incrementSlide = 1.0f )]
        public float dumpRate;

        [KSPField(guiName="dump", guiActive=false, guiActiveEditor=false, category="dump resource"),
            UI_Toggle(controlEnabled=true,enabledText="", disabledText="" ,scene= UI_Scene.Flight)]
        bool dumping = false;

#endregion
//------------------------------------------------------------------------------------------------------------------------
#region actions

        [KSPAction(guiName:"toggle dumping")]
        public void toggleDump(KSPActionParam ap)
        {
            Log.post("ACTION: toggle dumping '" + displayName + "'.", LogLevel.LOG_INFORMATION);
            dumping.Toggle();
        }

        [KSPAction(guiName:"start dumping")]
        public void startDumping(KSPActionParam ap)
        {
            Log.post("ACTION: start dumping '" + displayName + "'.", LogLevel.LOG_INFORMATION);
            dumping = true;
        }

        [KSPAction(guiName:"stop dumping")]
        public void stopDumping(KSPActionParam ap)
        {
            Log.post("ACTION: stop dumping '" + displayName + "'.", LogLevel.LOG_INFORMATION);
            dumping = false;
        }

        [KSPAction(guiName: "dumping rate - 100 L/s")]
        public void dumpingRateMinusMinusMinus(KSPActionParam ap)
        {
            DumpRate -= 100.0f;
        }

        [KSPAction(guiName: "dumping rate - 10 L/s")]
        public void dumpingRateMinusMinus(KSPActionParam ap)
        {
            DumpRate -= 10.0f;
        }

        [KSPAction(guiName: "dumping rate - 1 L/s")]
        public void dumpingRateMinus(KSPActionParam ap)
        {
            DumpRate -= 1.0f;
        }

        [KSPAction(guiName: "dumping rate + 1 L/s")]
        public void dumpingRatePlus(KSPActionParam ap)
        {
            DumpRate += 1.0f;
        }

        [KSPAction(guiName: "dumping rate + 10 L/s")]
        public void dumpingRatePlusPlus(KSPActionParam ap)
        {
            DumpRate += 10.0f;
        }

        [KSPAction(guiName: "dumping rate + 100 L/s")]
        public void dumpingRatePlusPlusPlus(KSPActionParam ap)
        {
            DumpRate += 100.0f;
        }

#endregion
//------------------------------------------------------------------------------------------------------------------------
#region propeties

        public  PartResource Resource
        {
            get { return part.Resources.Get(resourceName.GetHashCode()); }
        }

        public float DumpRate
        {
            get { return dumpRate; }
            set
            {
                value.Clamp(minDumpRate, maxDumpRate);
                dumpRate = value;
            }
        }

#endregion
//------------------------------------------------------------------------------------------------------------------------
#region Message receiving

        [PartMessageListener(typeof(AirshipDrainSelected), scenes:GameSceneFilter.Flight, relations:PartRelationship.Self)]
        public void AirshipDrainSelected(string displayName)
        {
            Log.post("Received Part message 'AirshipDrainSelected' for resource: " + displayName, LogLevel.LOG_INFORMATION);
            if(this.displayName == displayName)
            {
                Log.post("activate tweakable ui for resource: " + displayName, LogLevel.LOG_INFORMATION);
                activateGui();
                
            }
            else
            {
                Log.post("deactivate tweakable ui for resource: " + displayName, LogLevel.LOG_INFORMATION);
                deactivateGui();
            }

        }


#endregion
//------------------------------------------------------------------------------------------------------------------------
#region callbacks

        public override void OnAwake()
        {
            Log.post(this.ClassName + " OnAwake-callback: ");
            base.OnAwake();
            PartMessageService.Register(this);
        }

        public override void OnStart(StartState state)
        {
            Log.post(this.ClassName + " OnStart-callback: " + state.ToString());

            Actions["toggleDump"].guiName = "toggle " + resourceName + " dumping";
            Actions["startDumping"].guiName = "dump " + resourceName;
            Actions["stopDumping"].guiName = "stop dumping " + resourceName;

            ((UI_FloatEdit)Fields["dumpRate"].uiControlFlight).minValue = minDumpRate;
            ((UI_FloatEdit)Fields["dumpRate"].uiControlFlight).maxValue = maxDumpRate;
            //((UI_FloatEdit)Fields["dumpRate"].uiControlFlight).maxValue = maxDumpRate;
            
           
        }

        public override void OnFixedUpdate()
        {
            if (dumping)
            {
                float amountDumped;
                Log.post("dumping: " + resourceName + " at rate " + dumpRate);
                amountDumped = part.RequestResource(resourceName, dumpRate * TimeWarp.deltaTime);
                Log.post("dumped: " + amountDumped + " of " + resourceName);
            }
        }

#endregion
//------------------------------------------------------------------------------------------------------------------------
#region private methods

        private void activateGui()
        {
            Fields["dumpRate"].guiActive = true;
            Fields["dumping"].guiActive = true;
        }

        private void deactivateGui()
        {
            Fields["dumpRate"].guiActive = false;
            Fields["dumping"].guiActive = false;
        }


#endregion

    }
}
