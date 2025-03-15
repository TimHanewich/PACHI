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
                if (cs.Name == "ContactDetailsScreen")
                {
                    foreach (CanvasControl cc in cs.Controls)
                    {
                        if (cc.ControlType == "form")
                        {
                            foreach (CanvasControl card in cc.Children)
                            {
                                card.Properties.Add("VALUE", Guid.NewGuid().ToString());
                            }
                        }
                    }
                }
            }
            
            foreach (CanvasScreen cs in app.Screens)
            {
                Console.WriteLine(cs.Describe());
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}