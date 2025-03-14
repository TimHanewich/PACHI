using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PACHI
{
    public class YamlParser
    {

        public static JObject ParseYAML(string yaml)
        {
            return ParseYAML(yaml.Split(Environment.NewLine));
        }

        public static JObject ParseYAML(string[] lines)
        {

            //Hoppers for collecting stuff about a child element
            string? ChildObjectName = "";
            List<string>? ChildObject = null;

            JObject ToReturn = new JObject();

            foreach (string line in lines)
            {
                if (line != "") //ignore empty lines
                {
                    if (ChildObjectName != null && ChildObject != null) //if we are collecting for a child object right now, throw it in the batch
                    {
                        int LeadingTabsOfDeclaringLine = CountLeadingSpaces(ChildObjectName); //just using spaces as I've seen that format used mostly
                        int ThisLinesLeadingTabs = CountLeadingSpaces(line); //just using spaces as I've seen that format used mostly
                        if (ThisLinesLeadingTabs > LeadingTabsOfDeclaringLine) //We are still WITHIN the child
                        {
                            Console.WriteLine("Still more than " + LeadingTabsOfDeclaringLine.ToString() + " indents: " + line);
                            ChildObject.Add(line);
                            continue;
                        }
                        else //We are no longer in it... the indent just decreased
                        {                       
                            //Add it
                            JObject ThisChildObject = ParseYAML(ChildObject.ToArray());
                            ToReturn.Add(ChildObjectName.Trim(), ThisChildObject);

                            //Clear it
                            ChildObjectName = null;
                            ChildObject = null;
                        }
                    }

                    //Handle!
                    if (line.EndsWith(":")) //we are not collecting for a child right now, but this line here is declaring the start of a child, so start collecting for a child
                    {
                        ChildObjectName = line.Substring(0, line.Length - 1); //trim out the trailing ":"
                        ChildObject = new List<string>(); //Create a list of sub-lines
                        Console.WriteLine("New child for '" + ChildObjectName + "' started!");
                    }
                    else //it must be a property add it as a property
                    {
                        
                        //each yaml property should have a colon where the property name is separated from the value
                        int ColonLocation = line.IndexOf(":");
                        if (ColonLocation == -1)
                        {
                            throw new Exception("YAML parsing error! Property line '" + line + "' did not split into a name:value pair");
                        }
                        
                        //Get property name + value
                        string PropName = line.Substring(0, ColonLocation).Trim();
                        string PropVal = line.Substring(ColonLocation + 2).Trim(); //add 2 to skip the colon and then the obligatory empty space after the colon
                        ToReturn.Add(PropName, PropVal);
                    }
                }
                
            }

            //Add any child object that we were building but is still in the hopper
            // (there wasnt a line w/ a lesser indent after it that clearly indicated the end, so it wasn't triggered above... this could happen at the end of file for example)
            if (ChildObject != null && ChildObjectName != null)
            {
                JObject ThisChildObject = ParseYAML(ChildObject.ToArray());
                ToReturn.Add(ChildObjectName.Trim(), ThisChildObject);
            }

            return ToReturn;
        }

        public static int CountLeadingSpaces(string line)
        {
            int count = 0;
            foreach (char c in line)
            {
                if (c == ' ')
                {
                    count = count + 1;
                }
                else
                {
                    break;
                }
            }
            return count;
        }
    }
}