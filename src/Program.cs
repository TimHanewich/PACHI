using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PACHI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string YamlTxt = System.IO.File.ReadAllText(@"C:\Users\timh\Downloads\ContactEntryApp\Src\ContactDetailsScreen.fx.yaml");
            JObject doc = YAMLParser.Parse(YamlTxt);
            Console.WriteLine(doc.ToString());    
        }
    }
}