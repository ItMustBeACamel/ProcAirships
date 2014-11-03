using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ProcAirships
{
    class AirshipsTutorial : TutorialScenario
    {
        public override void OnAwake()
        {
            Log.post("AirshipsTutorial OnAwake", LogLevel.LOG_DEBUG);
            base.OnAwake();
        }

        protected override void OnAssetSetup()
        {
            instructorPrefabName = "Instructor_Gene";
        }

        protected override void OnTutorialSetup()
        {
            Log.post("AirshipsTutorial OnTutorialSetup", LogLevel.LOG_DEBUG);
           

            TutorialPage page1 = new TutorialPage("page1");
            page1.windowTitle = "Airships Tutorial";
            page1.OnDrawContent = () =>
            {
                GUILayout.Label("Welcome to the Procedural Airships tutorial. \n" +
                    "Today we will learn how to control a simple airship by using ballonets.", GUILayout.ExpandHeight(true));

                if (GUILayout.Button("Next")) Tutorial.GoToNextPage();
            };  
            this.Tutorial.AddPage(page1);

            TutorialPage page2 = new TutorialPage("page2");
            page2.windowTitle = "Airships Tutorial";
            page2.OnDrawContent = () =>
            {
                GUILayout.Label("Lets have a look at our airship. Its envelope is composed of three parts. Two pointy 'envelope caps', and one cylindrical envelope between those two caps.\n" +
                    "It is important to understand that stacked Envelopes automatically form a connection between each other. Gas can flow freely between them. " +
                    "So everything you do on one envelope part will affect connected envelopes.\n", GUILayout.ExpandHeight(true));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous")) Tutorial.GoToLastPage();
                if (GUILayout.Button("Next")) Tutorial.GoToNextPage();
                GUILayout.EndHorizontal();
            };
            this.Tutorial.AddPage(page2);

            TutorialPage page3 = new TutorialPage("page3");
            page3.windowTitle = "Airships Tutorial";
            page3.OnDrawContent = () =>
            {
                GUILayout.Label("Now please do a right-click on the cylindric envelope between the two pointy caps. "+
                    "Wow thats a lot of complicated stuff isn't it? Well, it's not that hard actually. First thing you have to learn about is "+
                    "'net. Buoyancy'. It is the airships static lift after substracting the gravitational force. It is negative at the moment, "+
                    "and thats the reason why our airship sits patiently on the runway instead of floating around. Gravity holds it on the ground.\n"+
                    "Lets see how we can change that.", GUILayout.ExpandHeight(true));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous")) Tutorial.GoToLastPage();
                if (GUILayout.Button("Next")) Tutorial.GoToNextPage();
                GUILayout.EndHorizontal();
            };
            this.Tutorial.AddPage(page3);

            TutorialPage page4 = new TutorialPage("page4");
            page4.windowTitle = "Airships Tutorial";

            page4.OnUpdate = () =>
            {
                if (FlightGlobals.ActiveVessel.altitude >= 200.0)
                    Tutorial.GoToNextPage();
            };

            page4.OnDrawContent = () =>
            {
                GUILayout.Label("To change an envelopes buoyancy, you can use it's ballonet. A ballonet is an inflatable balloon that limits the space "+
                    "that the lifting gas can use to expand. Inflating the ballonet results in less buoyancy. Deflating the ballonet results in more "+
                    "buoyancy. But be careful! The ballonet direclty affects the pressure of the gas in the envelope. Please make sure you have selected the "+
                "cylindrical envelope (not the caps) and carefully deflate the ballonet until the airship rises to an altitude of 200 meters.", GUILayout.ExpandHeight(true));
            };
            this.Tutorial.AddPage(page4);

            TutorialPage page5 = new TutorialPage("page5");
            page5.windowTitle = "Airships Tutorial";

            page5.OnEnter = (KFSMState st) => { instructor.PlayEmote(instructor.anim_true_thumbsUp); };

            page5.OnDrawContent = () =>
            {
                GUILayout.Label("Well done! But what will happen if we alter the ballonets of the envelope caps? More power to you! Try it. "+
                "But be careful. Before going wild on the ballonets. Have a look at the 'press. Status' bar. It shows you how much the pressure in your envelope "+
                "differs from the ideal pressure. A half filled bar is good. An empty or completely filled bar is bad. Feel free to experiment. But make sure to set the "+
                "inflation of both caps back to 50% when you are ready to continue the tutorial.", GUILayout.ExpandHeight(true));

                
                if(fiftyfifty())
                    if (GUILayout.Button("Next")) Tutorial.GoToNextPage();
                
            };
            this.Tutorial.AddPage(page5);

            TutorialPage page6 = new TutorialPage("page6");
            page6.windowTitle = "Airships Tutorial";

            page6.OnEnter = (KFSMState st) => { instructor.PlayEmote(instructor.anim_true_smileA); };

            page6.OnDrawContent = () =>
            {
                GUILayout.Label("Alright. You might have noticed that you can use the ballonets on the caps to pitch your airship up and down. "+
                    "Using the right click menus is a good way to do it. But professional airship instructors, like me, know a better way: "+
                "Action Groups! I have already assigned some. Use the 1 Key to pitch the airship up and 2 to pitch it down. Try it, it works quite well, doesn't it?", GUILayout.ExpandHeight(true));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous")) Tutorial.GoToLastPage();
                if (GUILayout.Button("Next")) Tutorial.GoToNextPage();
                GUILayout.EndHorizontal();

            };
            this.Tutorial.AddPage(page6);


            TutorialPage page7 = new TutorialPage("page7");
            page7.windowTitle = "Airships Tutorial";
            page7.OnDrawContent = () =>
            {
                GUILayout.Label("There is one more important thing before we can start flying around. The higher you get, the less pressure the air will exert on your envelopes. "+
                "You must balance that out by deflating a ballonet or... well pressure will rise and bad things will happen. Fortunately there is a built-in automatic to assist "+
                "us. 'Pressure Control'. The Pressure Control system can take over control of a ballonet to assure pressure is always near the perfect spot to prevent damage to the envelopes. "+
                "Please activate Pressure Control now. Do it on the cylindric envelope but NOT on the caps, since we need to control the caps ourself to pitch the airship.", GUILayout.ExpandHeight(true));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous")) Tutorial.GoToLastPage();
                if (GUILayout.Button("Next")) Tutorial.GoToNextPage();
                GUILayout.EndHorizontal();

            };
            this.Tutorial.AddPage(page7);

            TutorialPage page8 = new TutorialPage("page8");
            page8.windowTitle = "Airships Tutorial";
            page8.OnDrawContent = () =>
            {
                GUILayout.Label("Alright, you are ready to go. Start your engines and throttle up to 100% Trim your pitch with '1' and '2' and use WASD to steer around. "+
                "Be careful though. It is a very maneuverable airship and turning too fast can cause it to spin out of control.", GUILayout.ExpandHeight(true));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous")) Tutorial.GoToLastPage();
                if (GUILayout.Button("Next")) Tutorial.GoToNextPage();
                GUILayout.EndHorizontal();

            };
            this.Tutorial.AddPage(page8);


            TutorialPage page9 = new TutorialPage("page9");
            page9.windowTitle = "Airships Tutorial";
            page9.OnDrawContent = () =>
            {
                GUILayout.Label("Congartulations. You just learned the basic airship controls. My Job here is done. But here are some challenges if you want to hang around a bit:\n"+
                "- Try to land on the VAB\n-See if you can land on the Island Runway\n-Find out how high you can get the airship without destroying the envelopes\n\n"+
                "You may also load the tutorial airship in a sandbox game and see if you can improve it.", GUILayout.ExpandHeight(true));

               
            };
            this.Tutorial.AddPage(page9);


            this.Tutorial.StartTutorial(page1);
            
        }

        bool fiftyfifty()
        {
            foreach( Part p in FlightGlobals.ActiveVessel.Parts)
            {
                if(p != FlightGlobals.ActiveVessel.rootPart)
                {
                    foreach(AirshipEnvelope ae in p.GetComponents<AirshipEnvelope>())
                    {
                        if (ae.ballonetInflation != 50.0f) return false;
                    }
                }
            }
            return true;
        }

         
    }
}
