using System;

namespace PACHI
{
    public class CanvasSimulator
    {
        private CanvasApp app;
        private CanvasScreen onScreen;

        public CanvasSimulator(CanvasApp canvas_app)
        {
            app = canvas_app;
            onScreen = canvas_app.Screens[0]; //start with first screen (default for the app)
        }
    }
}