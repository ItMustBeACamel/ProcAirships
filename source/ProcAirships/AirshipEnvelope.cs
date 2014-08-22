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
    [Serializable]
    public class AirshipEnvelope : PartModule
    {

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "envelope vol.", guiUnits = "m³", guiFormat = "F3")]
        private float envelopeVolume = 0;


        //[KSPField(guiActive = true, guiActiveEditor = true, guiName = "envelope net vol.", guiUnits = "m³", guiFormat = "F3")]
        public double envelopeVolumeNet = 0; // no save // ui as string
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "envelope net. vol.")]
        private string envelopeVolumeNetUI;


        [KSPField( isPersistant=true, guiActive=true, guiActiveEditor=true, guiName="lifting gas"),
            UI_ChooseOption(scene = UI_Scene.Editor, controlEnabled = true)]
        public string liftingGas; // type of lifting gas // KSPField peristent


        [KSPField]
        public float ballonetPercentageMax = 100.0f; // set by config // KSPField


        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "ballonet vol.", guiUnits="%", guiFormat="F3"),
            UI_FloatEdit(scene=UI_Scene.Editor, minValue=0.0f, maxValue=100.0f, incrementLarge=10.0f, incrementSmall= 1.0f, incrementSlide=0.01f)]
        public float ballonetPercentage = 30; // percentage of the overall volume of the envelope // set by user


        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "ballonet inflation", guiUnits = "%", guiFormat = "F6")]  
        public float ballonetInflation; //percent // calculated // ui // no save


        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "set inflation", guiUnits = "%", guiFormat = "F"),
            UI_FloatEdit(scene = UI_Scene.All, minValue = 0.0f, maxValue = 100.0f, incrementLarge = 10.0f, incrementSmall = 1.0f, incrementSlide = 0.01f)]
        public float ballonetTargetInflation = 50; // percent - set by user // save


        [KSPField]
        public float ballonetInflationRate = 1.0f; // m³ per second - KSPField // no persistence


        [KSPField]
        public float ballonetInflationRateEditor = 1.0f; // m³ per second - KSPField // no persistence


        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "ballonet max vol.", guiFormat = "F3")]
        public double ballonetVolumeMax; // m³ maximum possible volume of the ballonet // KSPField


        //[KSPField(isPersistant=true, guiActive = true, guiActiveEditor = true, guiName = "ballonet vol.", guiFormat="F3")]
        public double ballonetVolume = 0; // m³ // save // ui as string
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "ballonet vol.")]
        private string ballonetVolumeUI;


        [KSPField(guiActive=true, guiActiveEditor=true, guiName="ballonet status")]
        public string ballonetStatus; // ui


        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "lifting gas", guiFormat = "F3", guiUnits="kg"),
            UI_FloatEdit(scene = UI_Scene.Editor, minValue = 0.0f, maxValue = float.PositiveInfinity, incrementLarge = 1.0f, incrementSmall = 0.1f, incrementSlide = 0.001f)]
        public float liftingGasAmount; // kg


        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "temperature", guiFormat = "F4", guiUnits = "°C")]
        public float temperature; // gas temperature

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "abs. pressure", guiFormat = "F6", guiUnits="bar")]
        public float absolutePressure; // ui

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "rel. pressure", guiFormat = "F6", guiUnits = "bar")]
        public float relativePressure; // ui

        [KSPField]
        public float dryMassPerQubicMeter; // KSPField float



        [KSPField(guiName = "autofill", guiActive = false, guiActiveEditor = true),
            UI_Toggle(controlEnabled = true, enabledText = "", disabledText = "", scene = UI_Scene.Editor)]
        bool autofill = false; // ui

        
        List<LiftingGas> liftingGasOptions;
        
        Athmosphere athmosphere;

        //----------------------------------------------------------------------------------------------
        #region Actions 

        [KSPAction(guiName: "Ballonet - 1.0%")]
        public void ballonetMinusMinusMinus(KSPActionParam ap)
        {
            ballonetTargetInflation -= 1.0f;
        }

        [KSPAction(guiName: "Ballonet - 0.1%")]
        public void ballonetMinusMinus(KSPActionParam ap)
        {
            ballonetTargetInflation -= 0.1f;
        }

        [KSPAction(guiName: "Ballonet - 0.01%")]
        public void ballonetMinus(KSPActionParam ap)
        {
            ballonetTargetInflation -= 0.01f;
        }

        [KSPAction(guiName: "Ballonet + 0.01%")]
        public void ballonetPlus(KSPActionParam ap)
        {
            ballonetTargetInflation += 0.01f;   
        }

        [KSPAction(guiName: "Ballonet + 0.1%")]
        public void ballonetPlusPlus(KSPActionParam ap)
        {
            ballonetTargetInflation += 0.1f;
        }

        [KSPAction(guiName: "Ballonet + 1.0%")]
        public void ballonetPlusPlusPlus(KSPActionParam ap)
        {
            ballonetTargetInflation += 0.1f;
        }

        #endregion
        //----------------------------------------------------------------------------------------------



        public override void OnAwake()
        {
            Log.post(this.ClassName + " OnAwake-callback: ");
          
            base.OnAwake();
            PartMessageService.Register(this);   
        }

        public override void OnStart(StartState state)
        {
            Log.post(this.ClassName + " OnStart-callback: " + state.ToString());

            loadLiftingGasOptions();

            // check liftingGas validity. If invalid: set to default
            if (!liftingGasOptions.Any(a => a.displayName == liftingGas))
            {
                Log.post("no valid lifting gas selected. Set to default", LogLevel.LOG_WARNING);
                if (liftingGasOptions.Count > 0)
                {
                    liftingGas = liftingGasOptions.First <LiftingGas>().displayName;
                    Log.post("liftinggas set to: " + liftingGas, LogLevel.LOG_INFORMATION);
                }
                else
                    Log.post("no valid lifting gas option found.", LogLevel.LOG_ERROR);

            }
                
            setupUI();

            Log.post("AirshipEnvelope Module started", LogLevel.LOG_INFORMATION);
            
            foreach(BaseField f in Fields)
            {
                //Log.post(f.name + ": " + f.GetValue(this).ToString(), LogLevel.LOG_INFORMATION);
            }

            athmosphere = Factory.getAthmosphere();

            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                part.force_activate();
          
        }

        public override void OnLoad(ConfigNode node)
        {

            if (!node.TryGetValue("ballonetVolume", out ballonetVolume))
                Log.post("could not load ballonetVolume", LogLevel.LOG_ERROR);
        }

        public override void OnSave(ConfigNode node)
        {
            node.AddValue("ballonetVolume", ballonetVolume);
            //node.AddValue("testf", 0.003f);
            //node.AddValue("testd", 0.00000000003d);
            //node.AddValue("envelopeVolume", envelopeVolume);
        }

        double getTemperature()
        {
            //if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.SPH)
            //{
            //    return 20.0d;
            //}
            //else
            //    return part.temperature;
            return 20.0d;
        }

        double CelsiusToKelvin(double T)
        {
            return T + 273.15d;
        }

        double getAbsolutePressure()
        {
            LiftingGas gas = getCurrentLiftingGas();
            
            double n = liftingGasAmount / (gas.molarMass / 1000.0d);
            double R = 8.3144621;
            double T = CelsiusToKelvin(getTemperature());
            double V = envelopeVolumeNet;

            double p = (n * R * T) / V;

            return p / 1000.0d / 101.325d;
        }

        
        private LiftingGas getCurrentLiftingGas()
        {
            return liftingGasOptions.First<LiftingGas>(x => x.displayName == liftingGas);
        }

        public double requestBallonetAir(double amount)
        {
            float deltaTime = 0;
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                deltaTime = TimeWarp.deltaTime;
            else
                deltaTime = Time.deltaTime;

            if(Math.Abs(amount) <= ballonetInflationRate * deltaTime)
            {
                return amount;
            }
            else
            {
                return Math.Sign(amount) * ballonetInflationRate * deltaTime;
            }
        }

        private void updateEnvelope()
        {
            ballonetVolumeMax = (envelopeVolume / 100.0d) * (double)ballonetPercentage;

            ballonetInflation = (float)(ballonetVolume / (ballonetVolumeMax / 100.0f));

            double volumeDelta = (ballonetTargetInflation - ballonetInflation) * ballonetVolumeMax / 100.0d;

            if (volumeDelta > 0.01)
                ballonetStatus = "inflating";
            else if (volumeDelta < -0.01)
                ballonetStatus = "deflating";
            else
                ballonetStatus = "idle";

            ballonetVolume += requestBallonetAir(volumeDelta);
            ballonetVolumeUI = ballonetVolume.ToStringExt("F3") + "m³";

            envelopeVolumeNet = envelopeVolume - ballonetVolume;
            envelopeVolumeNetUI = envelopeVolumeNet.ToStringExt("F3") + "m³";

            temperature = (float)getTemperature();

            absolutePressure = (float)getAbsolutePressure();

            relativePressure = (float)(absolutePressure - athmosphere.getAirPressure());

            part.mass = dryMassPerQubicMeter * envelopeVolume + liftingGasAmount / 1000.0f;
            
            if(autofill)
            {
                if (relativePressure > 0.001f)
                    liftingGasAmount -= 0.0001f;
                else if (relativePressure < -0.001f)
                    liftingGasAmount += 0.0001f;
            }

        }

     
        void Update()
        {

            if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.SPH)
                updateEnvelope();
        }

        public override void OnFixedUpdate()
        {
            updateEnvelope();
         
        }

        // message receiving

        [PartMessageListener(typeof(PartVolumeChanged), scenes: ~GameSceneFilter.Flight)]
        public void ChangeVolume(string volumeName, float volume)
        {
            Log.post("received ChangeVolume message for " + volumeName + " Volume: " + volume);
            if (volumeName != PartVolumes.Tankage.ToString())
                return;

            if (volume <= 0f)
                throw new ArgumentOutOfRangeException("volume");
            Log.post("tank Volume Changed to " + volume, LogLevel.LOG_INFORMATION);
            
            envelopeVolume = volume;
        }

        [PartMessageListener(typeof(PartResourceInitialAmountChanged), scenes: GameSceneFilter.Flight)]
        public void ChangeInitResource(PartResource resource, double amount)
        {
            Log.post("Envelope changed init resource " + resource.resourceName + " to " + amount);
        }





        private void loadLiftingGasOptions()
        {
            liftingGasOptions = new List<LiftingGas>();
            
            ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes("LIFTING_GAS_OPTIONS");

            if (nodes.GetLength(0) == 0)
            {
                Log.post("ConfigNode 'LIFTING_GAS_OPTIONS' not found", LogLevel.LOG_ERROR);
                return;
            }
            else
            {
                foreach (ConfigNode optionsNode in nodes)
                {
                    foreach (ConfigNode liftingGasNode in optionsNode.GetNodes("LIFTING_GAS"))
                    {
                        LiftingGas liftingGas = new LiftingGas();

                        liftingGas.Load(liftingGasNode);
                        liftingGasOptions.Add(liftingGas);
                        Log.post("Added lifting gas option: " + liftingGas.displayName, LogLevel.LOG_INFORMATION);
                    }

                }

            }
        }

        private void setupUI()
        {
            
            BaseField field = Fields["liftingGas"];
            
            if (field != null)
            {
                UI_ChooseOption uiChooser = (UI_ChooseOption)field.uiControlEditor;

                if (uiChooser != null)
                {
                    Log.post("setting up UI for LiftingGasOptions");
                    uiChooser.options = liftingGasOptions.Select<LiftingGas, string>(a => a.displayName).ToArray();
                }
            }

            field = Fields["ballonetPercentage"];
            if (field != null)
            {
                UI_FloatEdit uiEdit = (UI_FloatEdit)field.uiControlEditor;

                if (uiEdit != null)
                {
                    Log.post("setting up UI for ballonetPercentage");
                    if (ballonetPercentageMax > 0.0f)
                        uiEdit.maxValue = ballonetPercentageMax;
                    else
                    {
                        field.guiActive = false;
                        field.guiActiveEditor = false;
                    }
                        
                }

            }
           
            
        }







        [Serializable]
        class LiftingGas : IConfigNode
        {
            [SerializeField]
            public string displayName;

            [SerializeField]
            public string resourceName;

            [SerializeField]
            public double molarMass;

            public void Load(ConfigNode node)
            {
                //ConfigNode.LoadObjectFromConfig(this, node);

                if (!node.TryGetValue("displayName", out displayName))
                    Log.post("Could not read displayName from ConfigNode", LogLevel.LOG_ERROR);

                if (!node.TryGetValue("resourceName", out resourceName))
                    Log.post("Could not read resourceName from ConfigNode", LogLevel.LOG_ERROR);

                if (!node.TryGetValue("molarMass", out molarMass))
                    Log.post("Could not read molarMass from ConfigNode", LogLevel.LOG_ERROR);


            }
            public void Save(ConfigNode node)
            {
                //ConfigNode.CreateConfigFromObject(this, node);

                node.SetValue("displayName", displayName);
                node.SetValue("resourceName", resourceName);
                node.SetValue("molarMass", molarMass.ToString());
            }

        }
         
    } // class
}
