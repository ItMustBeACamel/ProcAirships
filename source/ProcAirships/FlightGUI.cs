

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ProcAirships.GUI
{


    class RenderItem
    {
        public Mesh mesh;
        public Transform translation;
        public Material material;
    }

    

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class FlightGUI : MonoBehaviour
    {

        GameObject cameraObject;
        Camera cam;

        RenderTexture tex1;
        void Awake()
        {
            Log.post("airship GUI awake");

            tex1 = new RenderTexture(300, 300, 16);

            cameraObject = new GameObject();

            cam = cameraObject.AddComponent<Camera>();

            cam.projectionMatrix = Matrix4x4.Ortho(-10, 10, -10, 10, -10, 10);
            if (!tex1.IsCreated())
                tex1.Create();

            cam.targetTexture = tex1;
            cam.enabled = false;
            //cam.SetTargetBuffers(tex1.colorBuffer, tex1.depthBuffer);

            
            
            
        }

        void LateUpdate()
        {
            //tex1 = RenderTexture.GetTemporary(300, 300, 16);
            string partModelName = "stretchyTank";

            string sidesName = "sides";

            string endsName = "ends";
            string collisionName = "collisionMesh";

            List<Mesh> meshList = new List<Mesh>();
            List<RenderItem> items = new List<RenderItem>();

            Bounds box = new Bounds( new Vector3(0, 0, 0), new Vector3(0, 0, 0));

            foreach(Part part in FlightGlobals.ActiveVessel.parts)
            {
                if (part == null) continue;
                if(part.GetComponent<Buoyancy>() != null)
                {
                    //Log.post("found buoyancy");
                    Transform partModel = part.FindModelTransform(partModelName);

                    Transform sides = part.FindModelTransform(sidesName);
                    Transform ends = part.FindModelTransform(endsName);
                    Transform colliderTr = part.FindModelTransform(collisionName);

                    Material SidesMaterial = sides.renderer.material;
                    Material EndsMaterial = ends.renderer.material;

                                       

                    // Instantiate meshes. The mesh method unshares any shared meshes.
                    Mesh SidesMesh = sides.GetComponent<MeshFilter>().sharedMesh;
                    Mesh EndsMesh = ends.GetComponent<MeshFilter>().sharedMesh;
                    Mesh colliderMesh = colliderTr.GetComponent<MeshCollider>().sharedMesh;

                    RenderItem newItem = new RenderItem();

                    newItem.mesh = colliderMesh;
                    newItem.translation = sides;
                    newItem.material = SidesMaterial;
                    newItem.material.shader = Shader.Find("Unlit");
                    items.Add(newItem);

                    box.Encapsulate(SidesMesh.bounds);
                    //box.Encapsulate(EndsMesh.bounds);

                    //foreach (Vector3 v in SidesMesh.vertices)
                    //{
                    //    box.Encapsulate(v);
                    //}

                    
                    
                    //foreach (Vector3 v in EndsMesh.vertices)
                    //{
                    //    box.bounds.Encapsulate(v);
                    //}

                    
                    
                    //Matrix4x4 projMatrix = Matrix4x4.Ortho(-3, 3, -3, 3, -10, 10);

                    //for (int i = 0; i < SidesMaterial.passCount; ++i)
                    //{
                    //    SidesMaterial.SetPass(0);
                    //    Graphics.DrawMeshNow(SidesMesh, projMatrix);
                    //}
                    
                }
            }

            Matrix4x4 projMatrix = Matrix4x4.Ortho(-10, 10, -10, 10, -10, 10);
            //Matrix4x4.Ortho(box.min.x, box.max.x, box.min.y, box.max.y, box.min.z, box.max.z);
            
            //FlightCamera.fetch.camera
            if (!tex1.IsCreated())
                tex1.Create();

            RenderTexture.active = tex1;
            GL.Clear(true, true, Color.blue);

            GL.LoadProjectionMatrix(projMatrix);
            GL.LoadIdentity();
            GL.PushMatrix();

            GL.Begin(GL.TRIANGLES);
            foreach(RenderItem ri in items)
            {
                ri.material.SetPass(0);

                GL.Color(Color.grey);

                Graphics.DrawMeshNow(ri.mesh, projMatrix);

                foreach(int i in ri.mesh.triangles)
                {
                    GL.Vertex(ri.mesh.vertices[i]);
                    GL.TexCoord(ri.mesh.uv[i]);
                }

         
            }

            GL.End();

            GL.PopMatrix();


            //Graphics.DrawTexture(new Rect(100, 100, 300, 300), tex1);
           

            //foreach (Mesh m in meshList)
            //{
                
            //    Graphics.DrawMeshNow(m, FlightGlobals.ActiveVessel.rootPart.rigidbody.worldCenterOfMass, Quaternion.identity);
            //}


        }

        void OnGUI()
        {
            //Log.post("gui");
            UnityEngine.GUI.DrawTexture(new Rect(100, 100, 300, 300), tex1, ScaleMode.ScaleToFit, false);

            UnityEngine.GUI.Box(new Rect(10, 10, 100, 90), "Loader Menu");

            
        }

        

    }
}
