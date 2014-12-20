using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
    class SelectionGrid : Control
    {

        public SelectionGrid(IComposition parent, int xCount = 1)
            : base(parent)
        {
            XCount = xCount;
        }

        public override void OnDraw()
        {
            GUIContent[] itemsArray = items.ToArray();
            int newSelection = GUILayout.SelectionGrid(selection, itemsArray, XCount);

            if(newSelection != selection)
            {
                selection = newSelection;
                SendOnChange(selection);
            }
        }

        public void AddItem(GUIContent content)
        {
            if(null != content)
                items.Add(content);
        }

        public void AddItem(String caption, Texture image = null, String tooltip = "")
        {
            GUIContent c = new GUIContent(caption, image, tooltip);

            items.Add(c);
        }


        public event EventHandler<ChangeEventArgs<int>> OnChange;
        protected void SendOnChange(int index)
        {
            if (null != OnChange)
            {
                foreach (EventHandler<ChangeEventArgs<int>> eh in OnChange.GetInvocationList())
                {
                    try
                    {
                        ChangeEventArgs<int> args = new ChangeEventArgs<int>(index);
                        OnChange(this, args);
                    }
                    catch (Exception e)
                    {
                        Log.postException(e);
                    }
                }
            }
        }      
        
        
        private List<GUIContent> items = new List<GUIContent>();

        private int xCount = 0;
        public int XCount
        {
            get { return xCount; }
            set { xCount = value >= 1 ? value : 1; }
        }

        public int YCount
        {
            get { return items.Count / XCount; }
        }

        private int selection = 0;
        public int Selection
        {
            get { return selection; }
            set { selection = value.Clamp(0, items.Count); }
        }

        public IEnumerable<GUIContent> Items
        {
            get { return items; }
        }


    }
}
