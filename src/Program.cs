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

            //Set up
            CanvasApp app = CanvasApp.FromMSAPP(@"C:\Users\timh\Downloads\ContactEntryApp");
            CanvasSimulator sim = new CanvasSimulator(app);

            //Parse creds
            AzureOpenAICredentials? creds = JsonConvert.DeserializeObject<AzureOpenAICredentials>(System.IO.File.ReadAllText(@"C:\Users\timh\Downloads\PACHI\keys.json"));
            if (creds == null)
            {
                throw new Exception("Unable to extract Azure OpenAI creds from file.");
            }

            PACHI p = new PACHI("Create a contact record with first name 'Tim' and last name 'Hanewich'.", sim, creds);

            await p.CompleteAsync();
            

        }
    }
}