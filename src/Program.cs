using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimHanewich.AgentFramework;
using Spectre.Console;

namespace PACHI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DemoAsync().Wait();
        }

        public static async Task DemoAsync()
        {

            //Print titles

            Markup title1 = new Markup("[bold][underline][blue]PACHI[/][/][/]");
            title1.Centered();
            AnsiConsole.Write(title1);

            Markup title2 = new Markup("[grey35][bold]Power Apps x Copilot Headless Integration[/][/]");
            title2.Centered();
            AnsiConsole.Write(title2);

            Markup title3 = new Markup("[gray][italic]For more information, visit https://github.com/TimHanewich/PACHI[/][/]");
            title3.Centered();
            AnsiConsole.Write(title3);

            Console.WriteLine();
            Console.WriteLine();


            //Parse out Azue OpenAI credetials
            AnsiConsole.Markup("[gray]Collecting Azure OpenAI credentials... [/]");
            string aoai_path = "../keys.json";
            if (System.IO.File.Exists(aoai_path) == false)
            {
                System.IO.File.WriteAllText(aoai_path, JsonConvert.SerializeObject(JsonConvert.SerializeObject(new AzureOpenAICredentials()))); 
                AnsiConsole.MarkupLine("[red]Place your Azure OpenAI endpoint and API key in this file before proceeding: " + System.IO.Path.GetFullPath(aoai_path) + "[/]");
                return;
            }
            AzureOpenAICredentials? aoai = JsonConvert.DeserializeObject<AzureOpenAICredentials>(System.IO.File.ReadAllText(aoai_path));
            if (aoai == null)
            {
                AnsiConsole.MarkupLine("[red]Unable to parse your Azure OpenAI credentials out of keys.json.[/]");
                return;
            }
            if (aoai.URL == "" || aoai.ApiKey == "")
            {
                AnsiConsole.MarkupLine("[red]Populate '" + Path.GetFullPath(aoai_path) + "' with your Azure OpenAI endpoint and API key before proceeding.[/]");
                return;
            }
            AnsiConsole.MarkupLine("[gray]done![/]");

            //Validate that the power platform command line tool (PAC) is installed, logged in, and an environment is selected
            AnsiConsole.Markup("[gray]Validating PAC... [/]");
            if (PACPipeline.ConfirmPAC() == false)
            {
                AnsiConsole.MarkupLine("[red]I was not able to validate that you have the PAC CLI installed, logged in, with an environment selected.[/]");
                AnsiConsole.MarkupLine("[red]Visit here to download the PAC CLI: https://learn.microsoft.com/en-us/power-platform/developer/howto/install-cli-msi[/]");
                AnsiConsole.MarkupLine("[red]Use 'pac auth create' to log in.[/]");
                AnsiConsole.MarkupLine("[red]Use 'pac org select' to select an environment to use.[/]");
                AnsiConsole.MarkupLine("[red]And then come back here and try again![/]");
                return;
            }
            AnsiConsole.MarkupLine("[gray]done![/]");
            Console.WriteLine();
            Console.WriteLine();


            //Create the agent
            Agent a = new Agent();
            a.Model = aoai;
            a.Messages.Add(new Message(Role.system, "You are an AI agent that is meant to demonstrate how an AI can interact with a Power Apps app in a headless environment, performing the intent of a user against the app."));
            
            //Add tool: list apps
            Tool tool_list_apps = new Tool("list_apps", "List the Power Apps canvas apps that the user has available to use.");
            a.Tools.Add(tool_list_apps);

            //Add tool: use app
            Tool tool_use_app = new Tool("use_app", "Use an app to satisfy the user's request.");
            tool_use_app.Parameters.Add(new ToolInputParameter("app_name", "The name of the Power App that the user wants to use."));
            tool_use_app.Parameters.Add(new ToolInputParameter("task", "The task that the user is requesting we do in the app."));
            a.Tools.Add(tool_use_app);

            //Add opening message
            string OpeningMsg = "Hello, I'm an AI agent that is here to demonstrate how Microsoft Copilot can be used to interact with any Power Apps in a virtual headless environment! How can I help you?";
            a.Messages.Add(new Message(Role.assistant, OpeningMsg));
            AnsiConsole.MarkupLine("[blue]" + OpeningMsg + "[/]");
            Console.WriteLine();

            //Loop
            while (true)
            {

                //Input
                Console.WriteLine();
                string? input = null;
                while (input == null || input == "")
                {
                    Console.Write("> ");
                    input = Console.ReadLine();
                    Console.WriteLine();
                }
                a.Messages.Add(new Message(Role.user, input));

                //Prompt
                Prompt:
                AnsiConsole.Markup("[gray][italic]thinking... [/][/]");
                Message response = await a.PromptAsync();
                a.Messages.Add(response);
                Console.WriteLine();

                //Write content if there is some
                if (response.Content != null)
                {
                    if (response.Content != "")
                    {
                        Console.WriteLine();
                        AnsiConsole.MarkupLine("[blue]" + response.Content + "[/]");
                    }
                }

                //Handle tool calls
                if (response.ToolCalls.Length > 0)
                {
                    //Handle each tool call
                    foreach (ToolCall tc in response.ToolCalls)
                    {
                        string tool_call_response_payload = "";

                        //Fetch what the agent is asking for in the tool call
                        if (tc.ToolName == "list_apps")
                        {
                            AnsiConsole.Markup("[gray][italic]checking your apps... [/][/]");
                            string[] canvas_apps = PACPipeline.ListCanvasApps();
                            tool_call_response_payload = JsonConvert.SerializeObject(canvas_apps);
                        }
                        else if (tc.ToolName == "use_app")
                        {
                            AnsiConsole.Markup("[gray][italic]accessing app... [/][/]");
                        }

                        //Append tool response to messages
                        Message ToolResponseMessage = new Message();
                        ToolResponseMessage.Role = Role.tool;
                        ToolResponseMessage.ToolCallID = tc.ID;
                        ToolResponseMessage.Content = tool_call_response_payload;
                        a.Messages.Add(ToolResponseMessage);

                        //Confirm completion of tool call
                        AnsiConsole.MarkupLine("[gray][italic]complete[/][/]");
                    }

                    //Go straight to prompting again (do not ask user)
                    goto Prompt;
                }

            } //end of infinite loop chat


        }
    }
}