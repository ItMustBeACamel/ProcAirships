using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcAirships.UI
{
    

    abstract class Control
    {
        public Control(IComposition parent)
        {
            this.Parent = parent;
            if (null != parent)
                Parent.AddChild(this);
        }

        public readonly IComposition Parent;


        public abstract void OnDraw();

        public void Draw()
        {
            if (!Enabled)
                UnityEngine.GUI.enabled = false;

            OnDraw();
        }


        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private bool enabled = true;
        
    }
}
