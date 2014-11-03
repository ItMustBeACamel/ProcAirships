using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.SPH, GameScenes.SPACECENTER)]
    class ProcAirships : ScenarioModule
    {
        private static ProcAirships instance = null;

        [KSPField(isPersistant=true)]
        public float buoyancyMultiplicator = 5.0f;

        [KSPField(isPersistant = true)]
        public bool pressureDestruction = true;

        [KSPField(isPersistant = true)]
        public bool alwaysControllable = false;

        [KSPField(isPersistant = true)]
        public bool showTemperatureInFlight = true;

        public static ProcAirships Instance
        {
            get
            {
                return instance;
            }
        }

        ProcAirships()
            : base()
        {
            instance = this;            
        }


        public override void OnAwake()
        {
            Log.post("ProcAirships Scenario onAwake", LogLevel.LOG_DEBUG);
            base.OnAwake();
        }
        

        

        

    }
}
