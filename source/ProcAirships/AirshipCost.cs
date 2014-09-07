using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSPAPIExtensions;
using KSPAPIExtensions.PartMessage;

namespace ProcAirships
{
    public class AirshipCost : PartModule, IPartCostModifier
    {

        [KSPField]
        public float costPerCubicMeter = 0.0f;

        [KSPField(isPersistant=true)]
        public float overallCost = 0.0f;

        private float envelopeVolume=0.0f;



        public override void OnAwake()
        {
            Log.post(this.ClassName + " OnAwake-callback: ");

            base.OnAwake();
            PartMessageService.Register(this);
            Log.post(this.ClassName + "end of OnAwake-callback: ");
        }



        // message receiving

        [PartMessageListener(typeof(PartVolumeChanged), scenes: ~GameSceneFilter.Flight)]
        public void ChangeVolume(string volumeName, float volume)
        {
            Log.post("AirshipCost received ChangeVolume message for " + volumeName + " Volume: " + volume);
            if (volumeName != PartVolumes.Tankage.ToString())
                return;

            if (volume <= 0f)
                throw new ArgumentOutOfRangeException("volume");
            Log.post("AirshipCost - tank Volume Changed to " + volume, LogLevel.LOG_INFORMATION);

            envelopeVolume = volume;
            
        }


        // interface

        public float GetModuleCost()
        {
            if (util.editorActive())
            {
                //Log.post(this.ClassName + " getModuleCost");
                float cost = costPerCubicMeter * envelopeVolume;

                //Log.post(this.ClassName + " volume: " + envelopeVolume);
                //Log.post(this.ClassName + " cost per m³: " + costPerCubicMeter);
                //Log.post(this.ClassName + " volume costs: " + cost);

                foreach (PartResource resource in part.Resources)
                {
                    cost += (float)(resource.info.unitCost * resource.amount);
                }
                //Log.post(this.ClassName + " cost after resource costs: " + cost);

                foreach (AirshipEnvelope e in part.Modules.OfType<AirshipEnvelope>())
                {
                    AirshipEnvelope.LiftingGas lg = e.getCurrentLiftingGas();
                    if (lg != null)
                        cost += e.getCurrentLiftingGas().cost * e.LiftingGasAmount;
                }
                //Log.post(this.ClassName + " cost after lifting gas costs: " + cost);

                //Log.post(this.ClassName + " end of getModuleCost");
                overallCost = cost;
            }

            return overallCost;
        }
    }
}
