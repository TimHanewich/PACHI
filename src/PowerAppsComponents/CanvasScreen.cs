using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PACHI
{
    public class CanvasScreen : IDescribable
    {
        public string Name {get; set;}
        public List<CanvasControl> Controls {get; set;}

        public CanvasScreen()
        {
            Name = "";
            Controls = new List<CanvasControl>();
        }

        public static CanvasScreen FromYAML(string yaml)
        {
            CanvasScreen ToReturn = new CanvasScreen();

            //Convert YAML to JSON
            JObject AsJson = YAMLParser.Parse(yaml);
            
            foreach (JProperty property in AsJson.Properties())
            {
                if (property.Name.EndsWith(" As screen"))
                {
                    //Get screen name
                    ToReturn.Name = property.Name.Substring(0, property.Name.Length - 10); //trim off the ending " As screen" (10 characters)

                    //The screen itself is going to have its value set to another Jobject which contains indidividual UI elements
                    JObject ScreenChildren = (JObject)property.Value;
                    foreach (JProperty ScreenProperty in ScreenChildren.Properties())
                    {                        
                        ToReturn.Controls.Add(CanvasControl.FromJProperty(ScreenProperty));
                    }

                    //Sort controls by Y position
                    if (ToReturn.Controls.Count > 0)
                    {
                        //Prepare list of them sorted
                        List<CanvasControl> ControlsSorted = new List<CanvasControl>();
                        while (ToReturn.Controls.Count > 0)
                        {
                            //Find the lowest
                            CanvasControl LowestY = ToReturn.Controls[0];
                            foreach (CanvasControl cc in ToReturn.Controls)
                            {
                                if (cc.Properties.ContainsKey("Y") && LowestY.Properties.ContainsKey("Y"))
                                {
                                    int lowY = Convert.ToInt32(LowestY.Properties["Y"].ToString().Replace("=", ""));
                                    int thisY = Convert.ToInt32(cc.Properties["Y"].ToString().Replace("=", ""));
                                    if (thisY < lowY)
                                    {
                                        LowestY = cc;
                                    }
                                }
                            }

                            //Add it
                            ToReturn.Controls.Remove(LowestY); //Remove from list
                            ControlsSorted.Add(LowestY); //Add to sorted list
                        }
                        ToReturn.Controls = ControlsSorted;
                    }

                }
            }

            return ToReturn;
        }

        public string Describe()
        {
            string ToReturn = "A screen named '" + Name + "' with the following controls on it:";
            foreach (CanvasControl cc in Controls)
            {
                ToReturn = ToReturn + "\n- " + cc.Describe();
            }
            return ToReturn;
        }

        
    }
}