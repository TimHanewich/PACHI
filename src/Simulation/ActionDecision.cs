using System;

namespace PACHI
{
    public class ActionDecision
    {
        public Action Action {get; set;}
        public string? Control {get; set;}
        public string? Text {get; set;}

        public ActionDecision(Action action)
        {
            Action = action;
        }

        public ActionDecision(Action action, string control)
        {
            Action = action;
            Control = control;
        }

        public ActionDecision (Action action, string control, string text)
        {
            Action = action;
            Control = control;
            Text = text;
        }
    }
}