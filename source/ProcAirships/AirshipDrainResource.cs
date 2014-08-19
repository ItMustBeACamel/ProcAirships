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

        [KSPField(isPersistant = true, guiName = "dump rate", guiActive = false, guiActiveEditor = false, category = "dump resource", guiFormat="S6+3", guiUnits="L"),
            UI_FloatEdit(scene = UI_Scene.Flight, incrementLarge = 1.0f, incrementSmall = 0.1f, incrementSlide = 0.1f )]
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
            dumping = (dumping == true) ? false : true;
        }

        [KSPAction(guiName:"start dumping")]
        public void startDumping(KSPActionParam ap)
        {
            dumping = true;
        }

        [KSPAction(guiName:"stop dumping")]
        public void stopDumping(KSPActionParam ap)
        {
            dumping = false;
        }

#endregion
//------------------------------------------------------------------------------------------------------------------------
#region propeties

        public  PartResource Resource
        {
            get { return part.Resources.Get(resourceName.GetHashCode()); }
        }

#endregion
//------------------------------------------------------------------------------------------------------------------------
#region Message receiving

        [PartMessageListener(typeof(AirshipDrainSelected), scenes:GameSceneFilter.Flight, relations:PartRelationship.Self)]
        public void AirshipDrainSelected(string displayName)
        {
            if(this.displayName == displayName)
            {
                activateGui();
            }
            else
            {
                deactivateGui();
            }

        }


#endregion
//------------------------------------------------------------------------------------------------------------------------
#region callbacks

        public override void OnAwake()
        {
            base.OnAwake();
            PartMessageService.Register(this);
        }

        public override void OnStart(StartState state)
        {
            Actions["toggleDump"].guiName = "toggle " + resourceName + " dumping";
            Actions["startDumping"].guiName = "dump " + resourceName;
            Actions["stopDumping"].guiName = "stop dumping " + resourceName;

            ((UI_FloatEdit)Fields["dumpRate"].uiControlFlight).minValue = minDumpRate;
            ((UI_FloatEdit)Fields["dumpRate"].uiControlFlight).maxValue = maxDumpRate;
            
           
        }

        public override void OnFixedUpdate()
        {
            if (dumping)
            {
                part.RequestResource(resourceName, dumpRate * TimeWarp.deltaTime);
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
