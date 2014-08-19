using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using KSP;
using UnityEngine;
using KSPAPIExtensions;
using KSPAPIExtensions.PartMessage;

namespace ProcAirships
{

    [PartMessageDelegate(isAbstract: true)]
    public delegate void AirshipDrainChanged();

    [PartMessageDelegate(typeof(AirshipDrainChanged))]
    public delegate void AirshipDrainSelected([UseLatest] string displayName);

    
    public class AirshipDrain : PartModule
    {

        [PartMessageEvent]
        public event AirshipDrainSelected drainChanged;

        private List<AirshipDrainResource> drainResources = new List<AirshipDrainResource>();

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "dump", category = "dump resource"),
        UI_ChooseOption(scene = UI_Scene.Flight, controlEnabled=true)]
        public string drainResource;

        private string drainResourceOld;

        public override void OnAwake()
        {
            Log.post(this.ClassName + " awake-callback");
            base.OnAwake();
            PartMessageService.Register(this);   
        }

        public override void OnStart(StartState state)
        {
            Log.post(this.ClassName + " start-callback: " + state.ToString());

            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                part.force_activate();

                Log.post("Searching for drainable resources");
                List<string> drainResourceNames = new List<string>();
                foreach (AirshipDrainResource dr in GetComponents<AirshipDrainResource>())
                {
                    if (dr.Resource != null)
                    {
                        drainResources.Add(dr);
                        drainResourceNames.Add(dr.displayName);
                        Log.post("Found drainable resource: " + dr.displayName, LogLevel.LOG_INFORMATION);
                    }
                    else
                        Log.post("Found drainable resource: " + dr.displayName + " but resource is not available on part", LogLevel.LOG_INFORMATION);

                }
                Log.post("Finished searching for drainable resources");

                Log.post("adding drainable resources to tweakable choose field");
                BaseField field = Fields["drainResource"];
                if (field != null)
                {
                    UI_ChooseOption options = (UI_ChooseOption)field.uiControlFlight;

                    if (options != null)
                    {
                        if (drainResourceNames.Count > 0)
                        {
                            options.options = drainResourceNames.ToArray();
                            drainResource = drainResourceNames[0];
                        }
                        else
                        {
                            Log.post("No drainable resources. tweakable disabled.", LogLevel.LOG_INFORMATION);
                            options.controlEnabled = false;
                        }
                        
                    }
                    else
                        Log.post("Field 'drainResource' does not have a UI_ChooseOption attribute", LogLevel.LOG_ERROR);

                }
                else
                    Log.post("Field 'drainResource' does not exist", LogLevel.LOG_ERROR);
            }
            

            
        }

        public override void OnUpdate()
        {
            if(drainResource != drainResourceOld)
            {
                Log.post("Drain resource selection changed");
                drainChanged(drainResource);
                drainResourceOld = drainResource;
            }
        }



    }
}
