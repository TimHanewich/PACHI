using System;
using System.Collections.Generic;

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
    }
}