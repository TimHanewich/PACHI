using System;
using Newtonsoft.Json.Linq;

namespace PACHI
{
    public class CanvasControl : IDescribable
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
    
        public string Describe()
        {
            string ToReturn = "A " + ControlType + " named '" + Name + "'";
           
            //Append what it says?
            if (Properties.ContainsKey("Text"))
            {
                string TextValue = Properties["Text"];
                if (TextValue.StartsWith("="))
                {
                    TextValue = TextValue.Substring(1);
                }
                if (TextValue.StartsWith("\"") && TextValue.EndsWith("\""))
                {
                    TextValue = TextValue.Substring(1, TextValue.Length - 2);
                }

                ToReturn = ToReturn + " that says '" + TextValue + "'";   
            }

            //If it is a form, append the fields
            if (ControlType.ToLower() == "form")
            {
                //Mapped to data source?
                string DataSource = Properties["DataSource"];
                if (DataSource.StartsWith("="))
                {
                    DataSource = DataSource.Substring(1); //strip out lead "=" sign
                }


                ToReturn = ToReturn + " mapped to the '" + DataSource + "' data source with the following fields (text input boxes):";
                foreach (CanvasControl DataFieldCard in Children)
                {
                    string DataField = DataFieldCard.Properties["DataField"];

                    //Strip out the leading =" and the ending "
                    if (DataField.StartsWith("=\""))
                    {
                        DataField = DataField.Substring(2);
                    }

                    //Strip out trailing "
                    if (DataField.EndsWith("\""))
                    {
                        DataField = DataField.Substring(0, DataField.Length - 1);
                    }

                    ToReturn = ToReturn + "\n\t- " + "Field '" + DataField + "'";

                    //Does it contain any text?
                    //I do not know how the text would be stored in YAML, or even if it would be (it probably wouldnt be)
                    //However, but for the sake of this demo, I will store it in the "VALUE" property as a placeholder.
                    if (DataFieldCard.Properties.ContainsKey("VALUE") == false)
                    {
                        ToReturn = ToReturn + " containing \"\"";
                    }
                    else //It has a value! (I must have put it there)
                    {
                        string VALUE = DataFieldCard.Properties["VALUE"];
                        ToReturn = ToReturn + " containing text \"" + VALUE + "\"";
                    }

                }
            }

            return ToReturn;
        }
    }
}