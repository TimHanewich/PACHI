using System;
using Newtonsoft.Json.Linq;

namespace PACHI
{
    public class CanvasControl
    {
        public string Name {get; set;}
        public string ControlType {get; set;}
        public Dictionary<string, string> Properties {get; set;}
        public List<CanvasControl> Children {get; set;}

        public CanvasControl()
        {
            Name = "";
            ControlType = "";
            Properties = new Dictionary<string, string>();
            Children = new List<CanvasControl>();
        }

        public static CanvasControl FromJProperty(JProperty prop)
        {
            CanvasControl ToReturn = new CanvasControl();

            //Get prop name to use (sometimes it using leading and trailing quotes, trim those out)
            string PropName = prop.Name;
            if (PropName.StartsWith("\"") && PropName.EndsWith("\""))
            {
                PropName = PropName.Substring(1, PropName.Length - 2); //trim out the starting and ending "
            }

            //Get the name of the property (the name of the property contains the name of the UI element AND the type, i.e. button)
            int AsLoc = PropName.LastIndexOf(" As ");
            ToReturn.Name = PropName.Substring(0, AsLoc);
            ToReturn.ControlType = PropName.Substring(AsLoc + 4);

            //Trim the name of leading and trailing "'" character if there is one
            if (ToReturn.Name.StartsWith("'") && ToReturn.Name.EndsWith("'"))
            {
                ToReturn.Name = ToReturn.Name.Substring(1, ToReturn.Name.Length - 2);
            }

            //Add each property in this control component into the dict
            JObject ControlProperties = (JObject)prop.Value;
            foreach (JProperty ControlProperty in ControlProperties.Properties())
            {

                //If it is an object in and of itself, it is a CanvasControl WITHIN a CanvasControl... so parse it recursively!
                if (ControlProperty.Value.Type == JTokenType.Object)
                {
                    ToReturn.Children.Add(CanvasControl.FromJProperty(ControlProperty));
                }
                else //It is normal text or whatever, just add it to the dict
                {
                    ToReturn.Properties.Add(ControlProperty.Name, ControlProperty.Value.ToString());
                }
            }

            return ToReturn;
        }
    }
}