using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
    [KSPAddon(KSPAddon.Startup.EditorAny,false)]
    class EditorUIManager : WindowManager
    {
        static EditorUIManager instance=null;

        EditorUI editorUI = null;


        public static EditorUIManager Instance
        {
            get
            {
                return instance;
            }

        }

        public override void OnStart()
        {
            
            instance = this;
        }

        public override void OnDestroy()
        {
            instance = null;
        }

        public void ShowEditorWindow()
        {
            if (editorUI == null)
            {
                editorUI = AddWindow<EditorUI>();
                editorUI.Position = new Vector2(300.0f, 300.0f);
                editorUI.Height = 500.0f;
            }

            editorUI.Visible = true;
            
        }

        public void HideEditorWindow()
        {
            if (editorUI == null)
            {
                editorUI = AddWindow<EditorUI>();
                editorUI.Position = new Vector2(300.0f, 300.0f);
            }

            editorUI.Visible = false;

        }

    }
}
