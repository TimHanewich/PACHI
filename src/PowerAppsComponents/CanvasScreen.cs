using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PACHI
{
    public class CanvasScreen
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
            Console.WriteLine(AsJson.ToString(Newtonsoft.Json.Formatting.Indented));

            return ToReturn;
        }

        
    }
}