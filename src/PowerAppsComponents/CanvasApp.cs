using System;
using System.Collections.Generic;

namespace PACHI
{
    public class CanvasApp
    {
        public string Name {get; set;}
        public List<CanvasScreen> Screens {get; set;}
        
        public CanvasApp()
        {
            Name = "";
            Screens = new List<CanvasScreen>();
        }
    }
}