using System;

namespace PACHI
{
    public class CanvasSimulator : IDescribable
    {
        private CanvasApp app;
        private CanvasScreen onScreen;

        public CanvasSimulator(CanvasApp canvas_app)
        {
            app = canvas_app;
            onScreen = canvas_app.Screens[0]; //start with first screen (default for the app)
        }

        public string Describe()
        {
            string ToReturn = "You are using an app named '" + app.Name + "' and you are on a screen named '" + onScreen.Name + "' within the app.";

            ToReturn = ToReturn + "\n\n" + "This is what you see on the screen:";
            foreach (CanvasControl cc in onScreen.Controls)
            {
                ToReturn = ToReturn + "\n- " + cc.Describe();
            }

            return ToReturn;
        }

        public void Click(string control_name)
        {
            CanvasControl? ToClick = null;
            foreach (CanvasControl cc in onScreen.Controls)
            {
                if (cc.Name == control_name)
                {
                    ToClick = cc;
                }
            }
            if (ToClick == null)
            {
                throw new Exception("You cannot click a control with name '" + control_name + "' because no control on the current screen has that name!");
            }
            Click(ToClick);
        }

        public void Click(CanvasControl cc)
        {
            //OnSelect property?
            if (cc.Properties.ContainsKey("OnSelect"))
            {
                string PowerFxCode = cc.Properties["OnSelect"];
                if (PowerFxCode != "" && PowerFxCode != "false")
                {
                    //Trim out leading = sign
                    if (PowerFxCode.StartsWith("="))
                    {
                        PowerFxCode = PowerFxCode.Substring(1);
                    }

                    //Execute the code
                    ExecutePowerFx(PowerFxCode);
                }
            }
        }

        public void ExecutePowerFx(string powerfx)
        {
            string[] formulas = powerfx.Split(";");
            foreach (string formula in formulas)
            {
                if (formula.StartsWith("Navigate"))
                {
                    //Get destination screen name
                    int loc1 = formula.IndexOf("(");
                    int loc2 = formula.IndexOf(")", loc1 + 1);
                    string NavigateTo = formula.Substring(loc1 + 1, loc2 - loc1 - 1);

                    //Is there a screen transition in it (second parameter)? If so, trim that portion out
                    if (NavigateTo.Contains(","))
                    {
                        loc1 = NavigateTo.IndexOf(",");
                        NavigateTo = NavigateTo.Substring(0, loc1 - 1);
                    }

                    //Find the destination
                    CanvasScreen? DestinationScreen = null;
                    foreach (CanvasScreen cs in app.Screens)
                    {
                        if (cs.Name == NavigateTo)
                        {
                            DestinationScreen = cs;
                        }
                    }

                    //Navigate?
                    if (DestinationScreen != null)
                    {
                        onScreen = DestinationScreen;
                    }
                    else
                    {
                        throw new Exception("PowerFx code '" + powerfx + "' calls for navigating to a screen with name '" + NavigateTo + "' but a screen with that name is not present in the app.");
                    }
                }
            }
        }
    }
}