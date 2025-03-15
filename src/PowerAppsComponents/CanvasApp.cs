using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PACHI
{
    public class CanvasApp
    {
        public string Name {get; set;}
        public List<CanvasScreen> Screens {get; set;}
        
        public CanvasApp()
        {
            Name = "";
            Screens = new List<CanvasScreen>();
        }

        //Parse from an unpacked .msapp file
        public static CanvasApp FromMSAPP(string dir_path)
        {
            CanvasApp ToReturn = new CanvasApp();

            //Get canvas manifest
            string CanvasManifestPath = Path.Combine(dir_path, "CanvasManifest.json");
            if (File.Exists(CanvasManifestPath) == false)
            {
                throw new Exception("File 'CanvasManifest.json' was not found within the provided .msapp directory.");
            }
            string CanvasManifestStr = System.IO.File.ReadAllText(CanvasManifestPath);
            JObject CanvasManifest = JObject.Parse(CanvasManifestStr);

            //Get app name
            JToken? AppName = CanvasManifest.SelectToken("PublishInfo.AppName");
            if (AppName != null)
            {
                ToReturn.Name = AppName.ToString();
            }


            //Get screens
            string SrcFolder = Path.Combine(dir_path, "Src");
            if (Directory.Exists(SrcFolder) == false)
            {
                throw new Exception("'Src' folder with screen data was not found in provided .msapp directory.");
            }
            string[] SrcFiles = Directory.GetFiles(SrcFolder);
            foreach (string FilePath in SrcFiles)
            {
                string extension = Path.GetExtension(FilePath);
                if (extension.ToLower() == ".yaml")
                {
                    string FileName = Path.GetFileName(FilePath);
                    if (FileName != "App.fx.yaml" && FileName != "Themes.json") // There is a standard App.fx.yaml file that is NOT a screen
                    {
                        string ScreenYaml = System.IO.File.ReadAllText(FilePath);
                        CanvasScreen ThisScreen = CanvasScreen.FromYAML(ScreenYaml);
                        ToReturn.Screens.Add(ThisScreen);
                    }
                }
            }





            return ToReturn;

        }

    }
}