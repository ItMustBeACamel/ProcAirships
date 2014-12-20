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
using KSPAPIExtensions;
using KSPAPIExtensions.PartMessage;

namespace ProcAirships
{
    public class AirshipCost : PartModule, IPartCostModifier
    {

        [KSPField]
        public float costPerCubicMeter = 0.0f;

        [KSPField]
        public float costPerSquareMeter = 0.0f;

        [KSPField(isPersistant=true, guiActiveEditor=true, guiName="cost")]
        public float overallCost = 0.0f;

        [KSPField(isPersistant=true)]
        public float envelopeVolume=0.0f;



        public override void OnAwake()
        {
            Log.post(this.ClassName + " OnAwake-callback: ");

            base.OnAwake();
            PartMessageService.Register(this);
            Log.post(this.ClassName + " end of OnAwake-callback: ");
        }

        
        public void updateCost()
        {
            float cost = 0;
            if(envelopeVolume > 0)
            { 
                //Log.post(this.ClassName + " getModuleCost");
                cost += costPerCubicMeter * envelopeVolume;

                // approximating by assuming a cubic shape
                cost += costPerSquareMeter * (float)(Math.Pow(envelopeVolume, 1.0d/3.0d) * 6.0d);
            }
           
            //Log.post(this.ClassName + " volume: " + envelopeVolume);
            //Log.post(this.ClassName + " cost per m³: " + costPerCubicMeter);
            //Log.post(this.ClassName + " volume costs: " + cost);


            foreach (PartResource resource in part.Resources)
            {
                cost += (float)(resource.info.unitCost * resource.maxAmount);
            }

            //Log.post(this.ClassName + " cost after resource costs: " + cost);

            foreach (AirshipEnvelope e in part.Modules.OfType<AirshipEnvelope>())
            {
                AirshipEnvelope.LiftingGas lg = e.getCurrentLiftingGas();
                if (lg != null)
                {
                    float liftingGasCost = e.getCurrentLiftingGas().cost * e.LiftingGasAmount;
                    cost += liftingGasCost;
                    Log.post(this.ClassName + " lifting gas cost: " + liftingGasCost);
                }
            }
            //Log.post(this.ClassName + " cost after lifting gas costs: " + cost);

            //Log.post(this.ClassName + " end of getModuleCost");

            
            overallCost = cost;
            Log.post(this.ClassName + " cost per m³: " + costPerCubicMeter);
            Log.post(this.ClassName + " cost per m²: " + costPerSquareMeter);
            Log.post(this.ClassName + " new cost: " + overallCost);

        }



        // message receiving

        [PartMessageListener(typeof(PartVolumeChanged), scenes: ~GameSceneFilter.Flight)]
        public void ChangeVolume(string volumeName, float volume)
        {
            Log.post("AirshipCost received ChangeVolume message for " + volumeName + " Volume: " + volume);
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

            if(util.editorActive())
                GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);    
        }
    
        // interface

        public float GetModuleCost(float stdCost)
        {
            //if (util.editorActive())
            //{
                updateCost();
            //}

            return overallCost;
        }
    }
}
