/*
 * Procedural Airships
 *   Copyright (C) 2014  Tobias Knappe <mindconductor@googlemail.com>
 * 
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
 *
 */
 

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
    public class AirshipEnvelope : PartModule, IPartMassModifier
    {

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "envelope vol.", guiUnits = "m³", guiFormat = "F3")]
        private float envelopeVolume = 0;


        //[KSPField(guiActive = true, guiActiveEditor = true, guiName = "envelope net vol.", guiUnits = "m³", guiFormat = "F3")]
        private double envelopeVolumeNet = 0; // no save // ui as string
        
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "envelope net. vol.")]
        private string envelopeVolumeNetUI;


        [KSPField( isPersistant=true, guiActive=true, guiActiveEditor=true, guiName="lifting gas"),
            UI_ChooseOption(scene = UI_Scene.Editor, controlEnabled = true)]
        public string liftingGas=""; // type of lifting gas // KSPField persistent


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
        public float ballonetInflationRate = 0.1f; // m³ per second per m³ volume - KSPField // no persistence


        [KSPField]
        public float ballonetInflationRateEditor = 1.0f; // m³ per second per m³ volume - KSPField // no persistence


        [KSPField(guiActive = false, guiActiveEditor = false, guiName = "ballonet max vol.", guiFormat = "F3")]
        public double ballonetVolumeMax; // m³ maximum possible volume of the ballonet // KSPField


        //[KSPField(isPersistant=true, guiActive = true, guiActiveEditor = true, guiName = "ballonet vol.", guiFormat="F3")]
        public double ballonetVolume = 0; // m³ // save // ui as string
        [KSPField(guiActive = false, guiActiveEditor = false, guiName = "ballonet vol.")]
        private string ballonetVolumeUI;


        [KSPField(guiActive=true, guiActiveEditor=true, guiName="ballonet status")]
        public string ballonetStatus; // ui


        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "lifting gas", guiFormat = "F3", guiUnits="kg"),
            UI_FloatEdit(scene = UI_Scene.Editor, minValue = 0.0f, maxValue = float.PositiveInfinity, incrementLarge = 1.0f, incrementSmall = 0.1f, incrementSlide = 0.001f)]
        private float liftingGasAmount = 0.0f; // kg


        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "temperature", guiFormat = "F4", guiUnits = "°C")]
        public float temperature = 0.0f; // gas temperature


        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "abs. pressure", guiFormat = "F6", guiUnits="bar")]
        private float absolutePressure; // ui // calsulated


        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "rel. pressure", guiFormat = "F6", guiUnits = "bar")]
        public float relativePressure; // ui // calculated

        [KSPField(guiActive=true, guiActiveEditor=true, guiName="press. Status", guiUnits="bar", guiFormat="F3"),
            UI_ProgressBar(controlEnabled=true, minValue= 0.0f, maxValue=1.0f, scene=UI_Scene.All)]
        private float pStatus;


        [KSPField]
        public float dryMassPerQubicMeter; // KSPField float

        [KSPField]
        private float idealRelPressure = 0.005f;

        [KSPField]
        private float pressureTolerance = 0.05f;


        [KSPField(isPersistant=true, guiName = "autofill", guiActive = false, guiActiveEditor = true),
            UI_Toggle(controlEnabled = true, enabledText = "", disabledText = "", scene = UI_Scene.Editor)]
        private bool autofill = true; // ui

        [KSPField(isPersistant=true, guiName = "pressureControl", guiActive = true, guiActiveEditor = true),
            UI_Toggle(controlEnabled = true, enabledText = "", disabledText = "", scene = UI_Scene.All)]
        private bool pressureControl = false; // ui

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "venting rate", guiUnits = "kg/s", guiFormat = "F4"),
            UI_FloatEdit(scene = UI_Scene.Flight, minValue = 0.01f, maxValue = float.PositiveInfinity, incrementLarge = 1.0f, incrementSmall = 0.1f, incrementSlide = 0.001f)]
        private float ventingRate = 0.01f;

        [KSPField(guiActive=true, guiActiveEditor=false, guiName="vent", isPersistant= true),
            UI_Toggle(controlEnabled=true, scene=UI_Scene.Flight)]
        private bool ventGas = false;

        private bool firstUpdate = false;

#region Properties

        public float EnvelopeVolume
        {
            get { return envelopeVolume; }
            protected set { envelopeVolume = value; }
        }

        public double EnvelopeVolumeNet
        {
            get { return envelopeVolumeNet; }
            protected set { envelopeVolumeNet = value; }
        }

        public float AbsolutePressure
        {
            get { return absolutePressure; }
            protected set { absolutePressure = value; }
        }

        public float LiftingGasAmount
        {
            get { return liftingGasAmount; }
            protected set 
            {
                liftingGasAmount = value;
                if (liftingGasAmount <= 0.0f)
                    liftingGasAmount = 0.0f;
            }
        }

        public float BallonetTargetInflation
        {
            get { return ballonetTargetInflation; }
            set
            {
                ballonetTargetInflation = value.Clamp(0.0f, 100.0f);
            }
        }

        public float VentingRate
        {
            get { return ventingRate; }
            protected set
            {
                ventingRate = value;
                if (ventingRate <= 0.0f)
                    ventingRate = 0.0f;
            }
        }

        public bool isControllable
        {
            get
            {
                if (null != ProcAirships.Instance)
                    return part.isControllable && !ProcAirships.Instance.alwaysControllable;
                else
                {
                    Log.post("Behavious ProcAirships not instantiated yet.", LogLevel.LOG_ERROR);
                    return true;
                }
            }

        }

        public float PressureTolerance
        {
            get { return pressureTolerance; }
        }

        public float RelPressure
        {
            get { return relativePressure; }
        }

        public float IdealRelPressure
        {
            get { return idealRelPressure; }
        }

        public bool AutoFill
        {
            get { return autofill; }
        }

        public float Overpressure
        {
            get
            {
                return Math.Abs((relativePressure - idealRelPressure)) - pressureTolerance;
            }
        }

        public bool PressureControl
        {
            get
            {
                return this.pressureControl;
            }
        }

        

#endregion
        
        List<LiftingGas> liftingGasOptions;
        
        //Athmosphere athmosphere;

        float damageTimer = 0.0f;

        float prevLiftingGasAmount = 0.0f;

        bool updateFlag = false;

//----------------------------------------------------------------------------------------------
#region Actions 

        [KSPAction(guiName: "Ballonet - 10.0%")]
        public void ballonetMinusMinusMinusMinus(KSPActionParam ap)
        {
            BallonetTargetInflation -= 10.0f;
        }

        [KSPAction(guiName: "Ballonet - 1.0%")]
        public void ballonetMinusMinusMinus(KSPActionParam ap)
        {
            BallonetTargetInflation -= 1.0f;
        }

        [KSPAction(guiName: "Ballonet - 0.1%")]
        public void ballonetMinusMinus(KSPActionParam ap)
        {
            BallonetTargetInflation -= 0.1f;
        }

        [KSPAction(guiName: "Ballonet - 0.01%")]
        public void ballonetMinus(KSPActionParam ap)
        {
            BallonetTargetInflation -= 0.01f;
        }

        [KSPAction(guiName: "Ballonet + 0.01%")]
        public void ballonetPlus(KSPActionParam ap)
        {
            BallonetTargetInflation += 0.01f;   
        }

        [KSPAction(guiName: "Ballonet + 0.1%")]
        public void ballonetPlusPlus(KSPActionParam ap)
        {
            BallonetTargetInflation += 0.1f;
        }

        [KSPAction(guiName: "Ballonet + 1.0%")]
        public void ballonetPlusPlusPlus(KSPActionParam ap)
        {
            BallonetTargetInflation += 1.0f;
        }

        [KSPAction(guiName: "Ballonet + 10.0%")]
        public void ballonetPlusPlusPlusPlus(KSPActionParam ap)
        {
            BallonetTargetInflation += 10.0f;
        }

        [KSPAction(guiName: "toggle venting")]
        public void toggleVenting(KSPActionParam ap)
        {
            ventGas = ventGas.Toggle();
        }

        [KSPAction(guiName: "vent gas")]
        public void startVenting(KSPActionParam ap)
        {
            ventGas = true;
        }

        [KSPAction(guiName: "stop gas venting")]
        public void stopVenting(KSPActionParam ap)
        {
            ventGas = false;
        }

        [KSPAction(guiName: "venting rate - 1.0")]
        public void ventingRateMinusMinusMinusMinus(KSPActionParam ap)
        {
            ventingRate -= 1.0f;
        }

        [KSPAction(guiName: "venting rate - 0.1")]
        public void ventingRateMinusMinusMinus(KSPActionParam ap)
        {
            ventingRate -= 0.1f;
        }

        [KSPAction(guiName: "venting rate - 0.01")]
        public void ventingRateMinusMinus(KSPActionParam ap)
        {
            ventingRate -= 0.01f;
        }

        [KSPAction(guiName: "venting rate - 0.001")]
        public void ventingRateMinus(KSPActionParam ap)
        {
            ventingRate -= 0.001f;
        }

        [KSPAction(guiName: "venting rate + 0.001")]
        public void ventingRatePlus(KSPActionParam ap)
        {
            ventingRate += 0.001f;
        }

        [KSPAction(guiName: "venting rate + 0.01")]
        public void ventingRatePlusPlus(KSPActionParam ap)
        {
            ventingRate += 0.01f;
        }

        [KSPAction(guiName: "venting rate + 0.1")]
        public void ventingRatePlusPlusPlus(KSPActionParam ap)
        {
            ventingRate += 0.1f;
        }

        [KSPAction(guiName: "venting rate + 1.0")]
        public void ventingRatePlusPlusPlusPlus(KSPActionParam ap)
        {
            ventingRate += 1.0f;
        }

        [KSPAction(guiName: "PControl On")]
        public void PControlOn(KSPActionParam ap)
        {
            pressureControl = true;
        }

        [KSPAction(guiName: "PControl Off")]
        public void PControlOff(KSPActionParam ap)
        {
            pressureControl = false;
        }

        [KSPAction(guiName: " toggle PControl")]
        public void PControlToggle(KSPActionParam ap)
        {
            pressureControl = pressureControl.Toggle();
        }

#endregion
//----------------------------------------------------------------------------------------------



        public override void OnAwake()
        {
            Log.post(this.ClassName + " OnAwake-callback: ");
          
            base.OnAwake();
            PartMessageService.Register(this);
   
            loadLiftingGasOptions();

            // check liftingGas validity. If invalid: set to default
            if (!liftingGasOptions.Any(a => a.displayName == liftingGas))
            {
                Log.post("no valid lifting gas selected. Set to default", LogLevel.LOG_WARNING);
                if (liftingGasOptions.Count > 0)
                {
                    liftingGas = liftingGasOptions.First<LiftingGas>(lgo => !lgo.deprecated).displayName;
                    Log.post("liftinggas set to: " + liftingGas, LogLevel.LOG_INFORMATION);
                }
                else
                    Log.post("no valid lifting gas option found.", LogLevel.LOG_ERROR);
            }

        }

        public override void OnStart(StartState state)
        {
            Log.post(this.ClassName + " OnStart-callback: " + state.ToString());

            //setupUI(); 


            if (!util.editorActive())
            {
                part.force_activate();
                if(getCurrentLiftingGas().combustible)
                part.maxTemp = getCurrentLiftingGas().maxTemperature;
            }

            damageTimer = -10.0f;

            Log.post("AirshipEnvelope Module started", LogLevel.LOG_INFORMATION);
            
        }

        void Update()
        {

            if (HighLogic.LoadedScene == GameScenes.EDITOR)
            {
                if (!firstUpdate)
                {
                    firstUpdate = true;
                    setupUI();
                }
                
                updateEnvelope();

            }

            //Log.post("world position: " + part.rigidbody.worldCenterOfMass.ToString());
        }

        public override void OnFixedUpdate()
        {
            if(!firstUpdate)
            {
                firstUpdate = true;
                setupUI();
            }
            updateEnvelope();

        }

        public void LateUpdate()
        {
            updateFlag = false;

            if ((pressureControl && !util.editorActive() && isControllable) || (pressureControl && util.editorActive() && !autofill))
            {
                if (relativePressure > idealRelPressure)
                {
                    BallonetTargetInflation -= 0.01f;
                }
                else if (relativePressure < idealRelPressure)
                {
                    BallonetTargetInflation += 0.01f;
                }

            }

        }

        public override void OnLoad(ConfigNode node)
        {

            if (!node.TryGetValue("ballonetVolume", out ballonetVolume))
                Log.post("could not load ballonetVolume", LogLevel.LOG_ERROR);
        }

        public override void OnSave(ConfigNode node)
        {
            node.AddValue("ballonetVolume", ballonetVolume);
            
        }

        double getTemperature()
        {
            if(util.editorActive())
            //if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.SPH)
            {
                return 20.0d;
            }
            else
            {
                if(vessel.isActiveVessel)
                    return part.temperature;
                else
                {
                    float alt = FlightGlobals.getAltitudeAtPos(part.rigidbody.worldCenterOfMass);
                    return FlightGlobals.getExternalTemperature(alt, FlightGlobals.currentMainBody);
                }
            }
                
            //return 20.0d;
        }

        double getAbsolutePressure()
        {
            LiftingGas gas = getCurrentLiftingGas();
            
            double n = liftingGasAmount / (gas.molarMass / 1000.0d);
            double R = util.GasConstant;
            double T = util.celsiusToKelvin(getTemperature());
            double V = envelopeVolumeNet;

            double p = (n * R * T) / V;

            return util.pascalToBar(p);
        }

        double getGasAmount(double pressure) // returns the gas amount thats needed to achieve a given pressure value in this envelope
        {

            LiftingGas gas = getCurrentLiftingGas();
            double m;
            double p = pressure * 1000.0d * 101.325d;
            double M = gas.molarMass / 1000.0d;
            double V = envelopeVolumeNet;
            double T = util.celsiusToKelvin(getTemperature());
            double R = util.GasConstant;

            m = M * ((p * V) / (R * T));

            return m;
        }

        public LiftingGas getCurrentLiftingGas()
        {
            if (liftingGas == "") return null;
            return liftingGasOptions.First<LiftingGas>(x => x.displayName == liftingGas);
        }

        
        public double requestBallonetAir(double amount)
        {
            
            if (!util.editorActive() && !isControllable)
                    return 0;

            float inflationRate = util.editorActive() ? ballonetInflationRateEditor : ballonetInflationRate;

            inflationRate *= envelopeVolume;

            float deltaTime = 0;
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                deltaTime = TimeWarp.fixedDeltaTime;
            else
                deltaTime = Time.deltaTime;

            if (Math.Abs(amount) <= inflationRate * deltaTime)
            {
                return amount;
            }
            else
            {
                return Math.Sign(amount) * inflationRate * deltaTime;
            }
        }

        private List<AirshipEnvelope> getConnectedEnvelopes(Part caller)
        {
            List<AirshipEnvelope> newList = new List<AirshipEnvelope>();
            newList.Add(this);

            foreach(AttachNode node in part.attachNodes.Where( x=> x.attachedPart != null))
            {
                if (node.attachedPart != caller)
                {
                    foreach (AirshipEnvelope other in node.attachedPart.Modules.OfType<AirshipEnvelope>())
                    {
                        if (other.getCurrentLiftingGas().displayName == getCurrentLiftingGas().displayName)
                            newList.InsertRange(0, other.getConnectedEnvelopes(part));
                    }
                }
            }

            return newList;
        }

        public List<AirshipEnvelope> getConnectedEnvelopes()
        {
            return getConnectedEnvelopes(null);
        }

        private void updateVolume()
        {
            ballonetVolumeMax = (envelopeVolume / 100.0d) * (double)ballonetPercentage;

            ballonetInflation = (float)(ballonetVolume / (ballonetVolumeMax / 100.0f));
            
            double volumeDelta = (ballonetTargetInflation - ballonetInflation) * ballonetVolumeMax / 100.0d;

            if (isControllable)
            {
                if (volumeDelta > 0.01)
                    ballonetStatus = "inflating";
                else if (volumeDelta < -0.01)
                    ballonetStatus = "deflating";
                else
                    ballonetStatus = "idle";
            }
            else
                ballonetStatus = "--no signal--";

            ballonetVolume += requestBallonetAir(volumeDelta);
            ballonetVolumeUI = ballonetVolume.ToStringExt("F3") + "m³";

            envelopeVolumeNet = envelopeVolume - ballonetVolume;
            envelopeVolumeNetUI = envelopeVolumeNet.ToStringExt("F3") + "m³";

            ballonetInflation = (float)(ballonetVolume / (ballonetVolumeMax / 100.0f));
        
        }

        private void updatePressureDamage()
        {
            damageTimer += TimeWarp.fixedDeltaTime;
            if (damageTimer >= 1.0f)
            {
                float overpressure = Overpressure;//Math.Abs((relativePressure - idealRelPressure)) - pressureTolerance;

                //Log.post("[" + part.vessel.vesselName + "] Checking for pressure damage");
                //Log.post("pressure deviation: " + (relativePressure - idealRelPressure));
                //Log.post("overpressure: " + overpressure);
                //Log.post("temperature: " + temperature);
                //Log.post("----------------------------------------------------------");

                if (overpressure > 0)
                {
                    float randomNumber = UnityEngine.Random.Range(0.0f, pressureTolerance);
                    if (randomNumber < overpressure)
                    {
                        if (null ==ProcAirships.Instance)
                        {
                            Log.post("Behaviour ProcAirships not instantiated yet");
                        }
                        else
                        if (ProcAirships.Instance.pressureDestruction)
                        {
                            part.explode();
                            FlightLogger.eventLog.Add("Envelope destroyed due to too high or low pressure.");
                            Log.post("Envelope destroyed due to too high or low pressure.", LogLevel.LOG_INFORMATION);
                        }
                        Log.post("Envelope destroyed due to too high or low pressure. Destruction prevented by settings", LogLevel.LOG_DEBUG);
                    }
                }

                damageTimer -= 1.0f;
            }
        }

        private void updateEnvelope()
        {

            if(!updateFlag) // do this once per update cycle, and before any envelopes gets updated
            {
                List<AirshipEnvelope> connectedEnvelopes = getConnectedEnvelopes();

                if(null == connectedEnvelopes)
                {
                    Log.post("Error getting connected envelopes", LogLevel.LOG_ERROR, this);
                    return;
                }

                double connectedVolume = 0.0;
                double connectedGas = 0.0;
                double connectedTemperature = 0.0;
                
                
                foreach(AirshipEnvelope envelope in connectedEnvelopes)
                {
                    envelope.updateVolume();
                    connectedVolume += envelope.envelopeVolumeNet;
                    connectedGas += envelope.liftingGasAmount;
                    connectedTemperature += envelope.getTemperature();
                    
                }
               
                connectedTemperature /= connectedEnvelopes.Count;

                double pressure = util.getPressure(connectedGas, getCurrentLiftingGas().molarMass, util.celsiusToKelvin(connectedTemperature), connectedVolume);

                foreach (AirshipEnvelope envelope in connectedEnvelopes)
                {
                    //envelope.absolutePressure = (float)pressure;
                    envelope.liftingGasAmount = (float)util.getGasAmount(pressure, getCurrentLiftingGas().molarMass,
                                                                            util.celsiusToKelvin(connectedTemperature),
                                                                            envelope.envelopeVolumeNet);
                    envelope.absolutePressure = (float)pressure;
                    envelope.updateFlag = true;
                }
            }

           
            temperature = (float)getTemperature();

            //absolutePressure = (float)getAbsolutePressure();
            //relativePressure = (float)(absolutePressure - athmosphere.getAirPressure());
            double airPressure = util.editorActive() ? Athmosphere.fetch().getAirPressure(EditorController.altitude) :
                Athmosphere.fetch().getAirPressure(part.rigidbody.worldCenterOfMass);

            relativePressure = (float)(absolutePressure - airPressure);

            pStatus = (relativePressure-idealRelPressure).Clamp(-pressureTolerance, pressureTolerance);

            part.mass = (dryMassPerQubicMeter * envelopeVolume) + (liftingGasAmount / 1000.0f);

            if (util.editorActive() && (autofill || EditorController.AutoFill))
                autoFill();

            if(ventGas && isControllable)
            {
                LiftingGasAmount -= ventingRate * TimeWarp.fixedDeltaTime;
            }

            if (!util.editorActive())
                updatePressureDamage();

            if(util.editorActive() && liftingGasAmount != prevLiftingGasAmount)
            {
                prevLiftingGasAmount = liftingGasAmount;

                GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship); 
            }
            

        }

        void autoFill()
        {
            //liftingGasAmount = (float)getGasAmount(athmosphere.getAirPressure() + idealRelPressure);
            liftingGasAmount = (float)getGasAmount(Athmosphere.fetch().getAirPressure(EditorController.altitude) + idealRelPressure);
        }

     
        

        // message receiving

        [PartMessageListener(typeof(PartVolumeChanged), scenes: ~GameSceneFilter.Flight)]
        public void ChangeVolume(string volumeName, float volume)
        {
            if(float.IsInfinity(volume) || float.IsNaN(volume))
            {
                Log.post("received Volume change message, but volume is not a valid number", LogLevel.LOG_ERROR);
                return;
            }

            Log.post("received ChangeVolume message for " + volumeName + " Volume: " + volume);
            if (volumeName != PartVolumes.Tankage.ToString())
                return;

            if (volume <= 0f)
            {
                Log.post("volume is: " + volume.ToString() + " thats odd... setting volume to 1 instead");
                envelopeVolume = 1.0f;
            }
            else
            {
                Log.post("tank Volume Changed to " + volume, LogLevel.LOG_INFORMATION);

                envelopeVolume = volume;
            }

            
        }

        /*
        [PartMessageListener(typeof(PartResourceInitialAmountChanged), scenes: GameSceneFilter.Flight)]
        public void ChangeInitResource(PartResource resource, double amount)
        {
            Log.post("Envelope changed init resource " + resource.resourceName + " to " + amount);
        }
         */





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
                    //uiChooser.options = liftingGasOptions.Select<LiftingGas, string>(a => a.displayName).ToArray();
                    uiChooser.options = (from lgo in liftingGasOptions
                                         where lgo.deprecated == false
                                         select lgo.displayName).ToArray();
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

            field = Fields["pStatus"];
            if (field != null)
            {
                UI_ProgressBar uiProg = (UI_ProgressBar)field.uiControlEditor;
                if (uiProg != null)
                {
                    Log.post("setting up pStatus");       
                    uiProg.maxValue = pressureTolerance;
                    uiProg.minValue = -pressureTolerance;
                }

                uiProg = (UI_ProgressBar)field.uiControlFlight;
                if (uiProg != null)
                {
                    Log.post("setting up pStatus");
                    uiProg.maxValue = idealRelPressure + pressureTolerance;
                    uiProg.minValue = idealRelPressure - pressureTolerance;
                }

            }

            field = Fields["envelopeVolume"];
            if (field != null)
            {
                Log.post("setting up ui envelope Volume");
                field.guiActive = Preferences.showVolumeInfoInFlight;
                field.guiActiveEditor = Preferences.showVolumeInfoInEditor;
            }

            field = Fields["envelopeVolumeNetUI"];
            if (field != null)
            {
                Log.post("setting up ui envelope Volume Net");
                field.guiActive = Preferences.showVolumeInfoInFlight;
                field.guiActiveEditor = Preferences.showVolumeInfoInEditor;
            }
            
            field = Fields["ballonetVolumeMax"];
            if (field != null)
            {
                Log.post("setting up ui ballonet vol max");
                field.guiActive = Preferences.showVolumeInfoInFlight;
                field.guiActiveEditor = Preferences.showVolumeInfoInEditor;
            }

            field = Fields["temperature"];
            if (field != null)
            {
                Log.post("setting up ui temperature");
                if (null == ProcAirships.Instance)
                    Log.post("behaviour ProcAirships not jet instantiated", LogLevel.LOG_ERROR);
                else
                {
                    field.guiActive = ProcAirships.Instance.showTemperatureInFlight;
                    field.guiActiveEditor = Preferences.showTemperatureInEditor;
                }
            }

            field = Fields["absolutePressure"];
            if (field != null)
            {
                Log.post("setting up ui abs pressure");
                field.guiActive = Preferences.showAbsPressureInFlight;
                field.guiActiveEditor = Preferences.showAbsPressureInEditor;
            }
            
            

           
            
        }







        [Serializable]
        public class LiftingGas : IConfigNode
        {
            [SerializeField]
            public string displayName;

            [SerializeField]
            public string resourceName;

            [SerializeField]
            public double molarMass;

            [SerializeField]
            public bool combustible;

            [SerializeField]
            public float maxTemperature;

            [SerializeField]
            public float cost;

            public bool deprecated = false;

            public void Load(ConfigNode node)
            {
                //ConfigNode.LoadObjectFromConfig(this, node);

                if (!node.TryGetValue("displayName", out displayName))
                    Log.post("Could not read displayName from ConfigNode", LogLevel.LOG_ERROR);

                if (!node.TryGetValue("resourceName", out resourceName))
                    Log.post("Could not read resourceName from ConfigNode", LogLevel.LOG_ERROR);

                if (!node.TryGetValue("molarMass", out molarMass))
                    Log.post("Could not read molarMass from ConfigNode", LogLevel.LOG_ERROR);

                if (!node.TryGetValue("combustible", out combustible))
                    Log.post("Could not read combustible from ConfigNode", LogLevel.LOG_ERROR);

                if (!node.TryGetValue("maxTemperature", out maxTemperature))
                {
                    if (combustible)
                        Log.post("Could not read maxTemperature from ConfigNode", LogLevel.LOG_ERROR);
                    else
                        maxTemperature = 0.0f;
                }

                if (!node.TryGetValue("cost", out cost))
                    Log.post("Could not read cost from ConfigNode", LogLevel.LOG_ERROR);

                if (!node.TryGetValue("deprecated", out deprecated))
                    Log.post("Could not read deprecated from ConfigNode", LogLevel.LOG_WARNING);

            }
            public void Save(ConfigNode node)
            {
                //ConfigNode.CreateConfigFromObject(this, node);

                node.SetValue("displayName", displayName);
                node.SetValue("resourceName", resourceName);
                node.SetValue("molarMass", molarMass.ToString());
                node.SetValue("cost", cost.ToString());
            }

        }


        public float GetModuleMass(float defaultMass)
        {
            return part.mass - defaultMass;
        }
    } // class
}
