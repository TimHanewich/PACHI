using System;
using TimHanewich.AgentFramework;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PACHI
{
    public class PACHI
    {
        private string task;
        private CanvasSimulator sim;
        private IModelConnection model;

        public PACHI(string desired_task, CanvasSimulator app_simulator, IModelConnection model_to_use)
        {
            task = desired_task;
            sim = app_simulator;
            model = model_to_use;
        }

        //Continuously asks the model to act until the job is done!
        public async Task CompleteAsync()
        {
            while (true)
            {
                bool DoneAfterNextAction = await ActAsync();
                if (DoneAfterNextAction)
                {
                    return;
                }
            }
        }

        //Asks model to decide what to do next and then does what it says to do.
        //Returns TRUE if it indicates the job is now done, FALSE if it did something but there is more work to do.
        public async Task<bool> ActAsync()
        {
        
            //Get system prompt
            string system = System.IO.File.ReadAllText(@"C:\Users\timh\Downloads\PACHI\prompts\system.md");

            //Produce user prompt
            string user = sim.Describe();
            user = user + "\n\n" + "Your task is: " + task;
            user = user + "\n\n" + "What do you do next?";

            //Construct agent
            Agent a = new Agent();
            a.Model = model;
            a.Messages.Add(new Message(Role.system, system));
            a.Messages.Add(new Message(Role.user, user));
            
            //Ask for its decision
            Message ModelResponseMsg = await a.PromptAsync(json_mode: true);
            if (ModelResponseMsg.Content == null)
            {
                throw new Exception("Model responded with an empty content! Something went wrong. Did it trigger a function that we aren't handling?");
            }

            //Parse its decision
            JObject ModelResponse = JObject.Parse(ModelResponseMsg.Content);

            //Get action
            JProperty? prop_action = ModelResponse.Property("action");
            if (prop_action == null)
            {
                throw new Exception("Property 'action' not found in model's response, but that is a required property no matter what. Something went wrong, the model did not respond as expected.");
            }
            string action = prop_action.Value.ToString();

            //Handle acton
            if (action == "complete")
            {
                Console.WriteLine("Model indicated the job is done!");
                return true;
            }
            else if (action == "click" || action == "type")
            {
                //Get control
                JProperty? prop_control = ModelResponse.Property("control");
                if (prop_control == null)
                {
                    throw new Exception("Model did not provide property 'control' despite it choosing action '" + action + "'. control is a required property.");
                }
                string control = prop_control.Value.ToString();

                if (action == "click")
                {
                    Console.WriteLine("Model decided to click '" + control + "'");
                    sim.Click(control);
                }
                else if (action == "type")
                {
                    //Get text property
                    JProperty? prop_text = ModelResponse.Property("text");
                    if (prop_text == null)
                    {
                        throw new Exception("Model did not provide property 'text' which is a required property when it decides to 'type', which it did.");
                    }
                    string text = prop_text.Value.ToString();

                    Console.WriteLine("Model decided to type '" + text + "' into '" + control + "'");
                    sim.Type(control, text);
                }
            }
            else
            {
                throw new Exception("Model responded with the action '" + action + "' which is not a valid action we gave it.");
            }

            return false; //Return false because it is not done
        }

    }
}