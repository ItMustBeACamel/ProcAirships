using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships
{

    struct force
    {
        public force(Vector3 pos, Vector3 dir)
        {
            Pos = pos;
            Dir = dir;
        }

        public Vector3 Pos;
        public Vector3 Dir;
    }

    public class COBMarker : EditorMarker_CoL
    {

        void Update()
        {   
            List<force> forces = new List<force>();
            float BuoyancySum = 0.0f;

            if (EditorLogic.fetch.ship == null) return;

            foreach (Part p in EditorLogic.fetch.ship.parts)
            {
                //Log.post("part found");
                if (p.isConnected)
                {
                    //Log.post("part is connected");
                    foreach (Buoyancy b in p.Modules.OfType<Buoyancy>())
                    {
                        //force f = new force(b.part.rigidbody.worldCenterOfMass, b.getBuoyancyForce());

                        forces.Add(new force(b.part.rigidbody.worldCenterOfMass ,b.getBuoyancyForce()));
                        BuoyancySum += b.getBuoyancyForce().magnitude;
                    }
                }
            }

            //Vector3 CoB = new Vector3();

            force result = new force( Vector3.zero, Vector3.up);

            foreach(force f in forces)
            {
                result.Pos += f.Dir.magnitude * f.Pos;
                result.Dir += f.Dir;
                //CoB += f.Dir.magnitude * f.Pos;
            }
            //CoB = (1.0f / BuoyancySum) * CoB;
            result.Pos = (1.0f / BuoyancySum) * result.Pos;

            this.posMarkerObject.transform.position = result.Pos; //CoB; //UpdatePosition();
  
            dirMarkerObject.transform.rotation = Quaternion.LookRotation(result.Dir.normalized);
            

        }

        protected override Vector3 UpdatePosition()
        {
            Log.post("COB marker update direction");
            return Vector3.zero;
        }

        protected override Vector3 UpdateDirection()
        {
            Log.post("COB marker update direction");
            Vector3 direction = Vector3.zero;

            foreach (Part p in EditorLogic.fetch.ship.parts)
            {
                Log.post("part found");
                if (p.isConnected)
                {
                    Log.post("part is connected");
                    foreach (Buoyancy b in p.Modules.OfType<Buoyancy>())
                    {
                        direction += b.getBuoyancyForce();
                        
                    }
                }
            }
            Log.post(direction);
            return direction.normalized;
        }

       

    }
}
