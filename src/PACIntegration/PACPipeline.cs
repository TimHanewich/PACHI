using System;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PACHI
{
    public class PACPipeline
    {
        public static string[] ListCanvasApps(string environment_id)
        {
            string cmd = "pac canvas list --environment " + environment_id;
            string pac_response = ExecuteCmd(cmd);
            Console.WriteLine(pac_response);
            string[] lines = pac_response.Split(Environment.NewLine);

            //Figure out starting position of the "Created by" columns
            int PosCreatedBy = lines[1].IndexOf("Created by");

            //Parse out
            List<string> ToReturn = new List<string>();
            for (int i = 2; i < lines.Length; i++)
            {
                if (lines[i] != "")
                {
                    string AppName = lines[i].Substring(0, PosCreatedBy);
                    ToReturn.Add(AppName.Trim());
                }
            }

            return ToReturn.ToArray();
        }

        public static string ExecuteCmd(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.Arguments = "/C " + command; // "/C" runs the command and then closes the console
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process p = new Process();
            p.StartInfo = psi;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();

            return output + error;
        }
    }
}