using System;

namespace PACHI
{
    public class CanvasControl
    {
        public string Name {get; set;}
        public string ControlType {get; set;}
        public Dictionary<string, string> Properties {get; set;}

        public CanvasControl()
        {
            Name = "";
            ControlType = "";
            Properties = new Dictionary<string, string>();
        }
    }
}