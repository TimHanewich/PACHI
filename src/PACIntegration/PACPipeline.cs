using System;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PACHI
{
    public class PACPipeline
    {
        public static string[] ListCanvasApps()
        {
            string cmd = "pac canvas list";
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

        public static void DownloadCanvasApp(string app_name, string output_path)
        {
            string cmd = "pac canvas download --name \"" + app_name + "\" --file-name \"" + output_path + "\"";
            ExecuteCmd(cmd);
        }

        public static void UnpackCanvasApp(string msapp_path, string output_directory)
        {
            //Validate
            if (System.IO.File.Exists(msapp_path) == false)
            {
                throw new Exception(".msapp file at '" + msapp_path + "' does not exist.");
            }
            if (System.IO.Directory.Exists(output_directory) == false)
            {
                throw new Exception("Output directory of '" + output_directory + "' does not exist!");
            }

            string cmd = "pac canvas unpack --msapp \"" + msapp_path + "\" --sources \"" + output_directory + "\"";
            ExecuteCmd(cmd);
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