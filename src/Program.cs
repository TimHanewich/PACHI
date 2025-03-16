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
            AnsiConsole.Markup("[gray]done![/]");





        }
    }
}