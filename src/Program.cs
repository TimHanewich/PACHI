using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PACHI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CanvasApp app = CanvasApp.FromMSAPP(@"C:\Users\timh\Downloads\ContactEntryApp");
            
            foreach (CanvasScreen cs in app.Screens)
            {
                Console.WriteLine(cs.Describe());
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}