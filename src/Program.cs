using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimHanewich.AgentFramework;

namespace PACHI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestAsync().Wait();
        }

        public static async Task TestAsync()
        {
            CanvasApp app = CanvasApp.FromMSAPP(@"C:\Users\timh\Downloads\ContactEntryApp");
           
            CanvasSimulator sim = new CanvasSimulator(app);

            Console.WriteLine(sim.Describe());

            sim.Click("EnterButton");

        }
    }
}