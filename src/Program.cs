using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PACHI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string YamlTxt = System.IO.File.ReadAllText(@"C:\Users\timh\Downloads\ContactEntryApp\Src\WelcomeScreen.fx.yaml");
            JObject doc = YamlParser.ParseYAML(YamlTxt);
            Console.WriteLine(doc.ToString());    
        }
    }
}