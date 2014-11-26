using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
    class WindowManager: MonoBehaviour
    {    
        RootWindow rootWindow = new RootWindow();
        
        
        public T AddWindow<T>(Window parent=null) where T : Window, new()
        {
            if (null == parent)
                return rootWindow.AddChild<T>();
            else
            {
                if (rootWindow.FindInChildren(parent.ID) == parent)
                    return parent.AddChild<T>();
                else
                    return null;
            }
        }
       
        
        #region Callback

        private void Start()
        {
            OnStart();
        }

        private void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos.y = Screen.height - mousePos.y;
            rootWindow.updateInputLock(mousePos);
        }

        private void OnGUI()
        {
            rootWindow.Draw();
        }

        private void Destroy()
        {
            rootWindow.Terminate();
            OnDestroy();
        }

        #endregion

        // userland

        public virtual void OnStart()
        {

        }

        public virtual void OnDestroy()
        {

        }


        private class RootWindow : Window
        {
            public new void Draw()
            {
                foreach(Window w in Children)
                {
                    w.Draw();
                }
            }

            public new void updateInputLock(Vector2 mousePos)
            {
                foreach (Window child in Children)
                    child.updateInputLock(mousePos);
            }

            public override void Close()
            {
                // do nothing, never delete the root node
            }

            public void Terminate()
            {
                base.Close();
            }

        }
    }
}
