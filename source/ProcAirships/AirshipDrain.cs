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
            base.OnAwake();
            PartMessageService.Register(this);   
        }

        public override void OnStart(StartState state)
        {
            print("Drain Start");
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                part.force_activate();
                List<string> drainResourceNames = new List<string>();
                foreach (AirshipDrainResource dr in GetComponents<AirshipDrainResource>())
                {
                    if (dr.Resource != null)
                    {
                        drainResources.Add(dr);
                        drainResourceNames.Add(dr.displayName);
                        Debug.Log("added drain resource: " + dr.displayName);
                    }
                }
                Debug.Log("finished adding");

                BaseField field = Fields["drainResource"];
                if (field != null)
                {
                    Debug.Log("field != null");
                    UI_ChooseOption range = (UI_ChooseOption)field.uiControlFlight;
                    
                    if (range != null)
                    {
                        if (drainResourceNames.Count > 0)
                        {
                            range.options = drainResourceNames.ToArray();
                            drainResource = drainResourceNames[0];
                        }
                        else
                            range.controlEnabled = false;
                        
                    }
                    else
                        Debug.LogError("range == null");

                }
                else
                    Debug.LogError("field == null");
            }
            

            
        }

        public override void OnUpdate()
        {
            if(drainResource != drainResourceOld)
            {
                drainChanged(drainResource);
                drainResourceOld = drainResource;
            }
        }



    }
}
