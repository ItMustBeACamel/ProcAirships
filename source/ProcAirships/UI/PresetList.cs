using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.ComponentModel;

namespace ProcAirships.UI
{
    class PresetList : Control
    {

        public struct Item
        {
            public Item(String caption)
            {
                if (null != caption)
                    this.caption = caption;
                else
                    this.caption = "";          
            }

            private String caption;
            public String Caption
            {
                get 
                { return null != caption ? caption : "";}
                set
                { caption = null != value ? value : ""; }
            }
        }

        public PresetList(IComposition parent)
        : base(parent)
        { }

        public override void OnDraw()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            Item[] itemArray = Items.ToArray();
            List<Item> newItemList = new List<Item>();
            for (int i = 0; i < itemArray.Length; ++i)
            {
                bool keep = true;
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(itemArray[i].Caption, GUILayout.ExpandWidth(true)))
                    SendOnSelect(i);

                if (GUILayout.Button("x", GUILayout.ExpandWidth(false)))
                    keep = SendOnRemove(i);

                GUILayout.EndHorizontal();

                if (keep)
                    newItemList.Add(itemArray[i]);
            }
            GUILayout.EndScrollView();

            items = newItemList;
        }


        private Vector2 scrollPos;

        private List<Item> items = new List<Item>();
        public Item[] Items
        {
            get { return items.ToArray(); }
            
        }

        public void AddItem(Item newItem)
        {
                items.Add(newItem);
        }

        public void AddItems(IEnumerable<Item> items)
        {
            this.items.AddRange(items);
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public void ClearItems()
        {
            items.Clear();
        }

        //private void FireEvent<T>(EventHandler<T> ev, T args) where T:EventArgs
        //{
        //    if(null != ev)
        //    {
        //        foreach(EventHandler inv in ev.GetInvocationList())
        //        {
        //            try
        //            {
        //                T a = (T)args;
        //                inv.Invoke(this, args);
        //            }
        //            catch(Exception exception)
        //            {
        //                Log.postException(exception);
        //            }
        //        }

        //    }

        //    ev(this, args);   
        //}
        
        public event EventHandler<SelectEventArgs<int>> OnSelect;
        protected void SendOnSelect(int index)
        {
            if (null != OnSelect)
            {
                SelectEventArgs<int> args = new SelectEventArgs<int>(index);
                foreach(EventHandler<SelectEventArgs<int>> inv in OnSelect.GetInvocationList())
                {
                    try
                    {
                        inv(this, args);
                    }
                    catch(Exception exception)
                    {
                        Log.postException(exception);
                    }
                }          
            }
        }

        public event EventHandler<CancelSelectEventArgs<int>> OnRemove;
        protected bool SendOnRemove(int index)
        {
            bool doIt = true;
            if (null != OnRemove)
            {
                foreach (EventHandler<CancelSelectEventArgs<int>> inv in OnRemove.GetInvocationList())
                {
                    try
                    {
                        CancelSelectEventArgs<int> args = new CancelSelectEventArgs<int>(index);  
                        inv(this, args);
                        if (args.Cancel)
                            doIt = false;
                    }
                    catch (Exception exception)
                    {
                        Log.postException(exception);
                    }
                }
            }
            return doIt;
        }
        
    }
}
