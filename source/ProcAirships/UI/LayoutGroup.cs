using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
    class LayoutGroup : CompositionControl
    {

        private void DrawChildren()
        {
            foreach (Control c in Children)
                c.Draw();
        }

        public override void OnDraw()
        {
            LayoutOrientation o = Orientation;

            switch(o)
            {
                case LayoutOrientation.Horizontal:
                    GUILayout.BeginHorizontal();
                    DrawChildren();
                    GUILayout.EndHorizontal();
                    break;

                case LayoutOrientation.Vertical:
                    GUILayout.BeginVertical();
                    DrawChildren();
                    GUILayout.EndVertical();
                    break;
            }        
        }

        public enum LayoutOrientation
        {
            Horizontal,
            Vertical
        }

        public LayoutGroup(IComposition parent, LayoutOrientation orientation = LayoutOrientation.Horizontal)
            : base(parent)
        {
            this.orientation = orientation;
        }

        private LayoutOrientation orientation;

        public LayoutOrientation Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }


    }
}
