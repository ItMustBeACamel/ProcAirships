using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
    class Window : IEquatable<Window>
    {
        
        public Window Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if (parent == value) return;
                if(parent != null)
                    parent.children.Remove(this);
                    
                parent = value;
                if (parent != null)
                    parent.children.AddLast(this);
            }
        }

        public Surface Surface
        {
            get { return surface; }
        }

        public int ID
        {
            get
            {
                return id;
            }
        }

        public string Caption
        {
            get
            { return caption; }
            set
            { caption = value; }
        }

        public IEnumerable<Window> Children
        {
            get
            {
                return children;
            }
        }

        public bool Draggable
        {
            get
            { return draggable; }
            set
            { draggable = value; }

        }

        public Vector2 Position
        {
            get
            { return bounds.position; }
            set
            { bounds.position = value; }
        }

        public Rect Rectangle
        {
            get
            { return bounds; }
            set
            { bounds = value; }
        }

        public float Height
        {
            get
            { return bounds.height; }
            set
            { bounds.height = value; }
        }

        public float Width
        {
            get
            { return bounds.width; }
            set
            { bounds.width = value; }
        }

        public bool Visible
        {
            get
            { return visible; }
            set
            { visible = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }       

        static IEnumerable<Window> Windows
        {
            get 
            {
                return windows.Values;
            }
        }

        private Window parent = null;
        private int id;
        private Rect bounds = new Rect(0, 0, 300, 300);
        private string caption;
        private bool draggable = true;
        private String lockName = "";
        private bool visible = true;
        private bool enabled = true;
        private Surface surface;
        private LinkedList<Window> children = new LinkedList<Window>();


        private static Dictionary<int, Window> windows = new Dictionary<int, Window>();
        static int IDCounter = 215693;

        public Window()
        {
            id = IDCounter;
            ++IDCounter;

            surface = new Surface(this);

            windows.Add(id, this);
        }

        ~Window()
        {
            Close();
            
            Log.post("Window <" + id + "> <" + caption + "> destroyed");
        }

        protected void AddChildren(params Window[] children)
        {
            foreach (Window w in children)
                AddChild(w);
        }

        protected void AddChild(Window child)
        {
                child.Parent = this;
                child.OnCreate();
        }

        protected Window GetRoot()
        {
            if (Parent == null)
                return this;
            else
                return Parent.GetRoot();
        }

        public T AddChild<T>() where T : Window, new()
        {
            T newWindow = new T();
            
            AddChild(newWindow);
            
            return newWindow;
        }

        public virtual void Close()
        {
            if (lockName != "")
            {
                InputLockManager.RemoveControlLock(lockName);
                lockName = "";
            }


            while(children.Count > 0)
                children.First.Value.Close();
            
            OnClose();
            Parent = null;
            windows.Remove(id);
        }

        public InputBox InputBox(String text, String title, params InputBoxOption[] options)
        {
            InputBox inputBox = new InputBox(text, title, options);
            AddChild(inputBox);
            return inputBox;
        }

        
        public void Draw()
        {
            if (visible)
            {
                UnityEngine.GUI.skin = HighLogic.Skin;
                
                bounds = UnityEngine.GUILayout.Window(id, bounds, WndFunc, caption);
                
                foreach (Window c in children)
                {
                    c.Draw();
                }
            }
        }

        private void WndFunc(int id)
        {
            UnityEngine.GUI.enabled = enabled;
            //OnDraw(id);
            OnPreDraw();
            if(Surface.Children.Length == 0)
                OnDraw(id);
            else
                Surface.Draw();
            OnPostDraw();
            if (Draggable)
                UnityEngine.GUI.DragWindow();
        }

        public Window FindInChildren(int id, bool recursive = true)
        {
            if (ID == id)
                return this;
            else
            {
                if(recursive)
                {
                    Window result = null;
                    foreach(Window child in children)
                    {
                        result = child.FindInChildren(id);
                        if (result != null)
                            return result;
                    }
                    return null;
                }
                else
                {
                    foreach(Window child in children)
                    {
                        if (child.ID == id)
                            return child;
                    }
                    return null;
                }
                
            }
        }


        public void updateInputLock(Vector2 mousePos)
        {
            if(Visible && bounds.Contains(mousePos))
            {
                if (lockName == "")
                {
                    lockName = "RadarGUI" + id;
                    InputLockManager.SetControlLock(ControlTypes.EDITOR_LOCK, lockName);
                }

            }
            else
            {
                if (lockName != "")
                {
                    InputLockManager.RemoveControlLock(lockName);
                    lockName = "";
                }
                    
            }

            foreach (Window child in children)
                child.updateInputLock(mousePos);
            
        }

        // userland

        protected virtual void OnDraw(int id) // override this to do UI stuff
        {

        }

        protected virtual void OnPreDraw()
        { }

        protected virtual void OnPostDraw()
        { }


        public virtual void OnCreate() // called after window creation
        {

        }

        public virtual void OnClose() // called before Window gets destroyed
        {

        }


        public bool Equals(Window other)
        {
            return id == other.ID;
        }

    }
}
