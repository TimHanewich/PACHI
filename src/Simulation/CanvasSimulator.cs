using System;

namespace PACHI
{
    public class CanvasSimulator : IDescribable
    {
        private CanvasApp app;
        private CanvasScreen onScreen;
        private List<string> InteractionHistory; //List of everything the user has done (each click + type)

        public CanvasSimulator(CanvasApp canvas_app)
        {
            app = canvas_app;
            onScreen = canvas_app.Screens[0]; //start with first screen (default for the app)
            InteractionHistory = new List<string>();
        }

        public string Describe()
        {
            string ToReturn = "You are using an app named '" + app.Name + "' and you are on a screen named '" + onScreen.Name + "' within the app.";

            ToReturn = ToReturn + "\n\n" + "This is what you see on the screen:";
            foreach (CanvasControl cc in onScreen.Controls)
            {
                ToReturn = ToReturn + "\n- " + cc.Describe();
            }

            //History
            if (InteractionHistory.Count > 0)
            {
                ToReturn = ToReturn + "\n\n" + "So far you have: ";
                foreach (string interaction in InteractionHistory)
                {
                    ToReturn = ToReturn + "\n- " + interaction;
                }
            }

            return ToReturn;
        }

        public void Click(string control_name)
        {
            CanvasControl? ToClick = FindControl(control_name);
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
            InteractionHistory.Add("clicked a " + cc.ControlType + " control named '" + cc.Name + "'");
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

        public void Type(string text_input_control_name, string content)
        {
            CanvasControl? ToTypeInto = FindControl(text_input_control_name);
            if (ToTypeInto == null)
            {
                throw new Exception("Unable to type into control '" + text_input_control_name + "' because a control with that name was not found on the current screen!");
            }
            Type(ToTypeInto, content);
        }

        //Type some text into a canvas control text input
        public void Type(CanvasControl cc, string content)
        {
            if (cc.ControlType != "text")
            {
                throw new Exception("Refusing to type text into control '" + cc.Name + "' because it is not a 'text' control type, it is a '" + cc.ControlType + "' control type.");
            }
            cc.Properties["VALUE"] = content; //Again, do not know where the text would be stored or even if it would be stored (likely would not be in YAML), but putting it as "VALUE" property for safe keeping in this demo.
            InteractionHistory.Add("Typed '" + content + "' into a text box named '" + cc.Name + "'");
        }


        //Finds a control on the screen by its name, searching at depth
        public CanvasControl? FindControl(string name)
        {
            return FindControl(onScreen.Controls.ToArray(), name);
        }

        //Searches the entire screen we are on right now for a control of a specific name, but at every depth (even through child controls of child controls of child controls, etc.)
        private CanvasControl? FindControl(CanvasControl[] search_through, string name)
        {
            foreach (CanvasControl cc in search_through)
            {
                if (cc.Name == name)
                {
                    return cc;
                }
                else
                {
                    CanvasControl? found = FindControl(cc.Children.ToArray(), name);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null; //If we went through all of them and none of them found it, we couldn't find it... so return null;
        }

    }
}