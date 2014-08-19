using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace ProcAirships
{
    public class BallastTank : PartModule
    {

        [KSPField(isPersistant = false, guiActive = true, guiName = "dump ballast")]
        public bool dumpBallast = false;


        public override void OnFixedUpdate()
        {
            if (dumpBallast)
            {
                
                part.RequestResource("BallastWater", Time.deltaTime * 10.0f);  
            }


        }

    }
}
