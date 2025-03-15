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


            //Now that we have the screens parsed and constructed, now ensure at least the first screen is brought first
            JProperty? ScreenOrder = CanvasManifest.Property("ScreenOrder");
            if (ScreenOrder != null)
            {
                string[]? ScreenOrderParsed = JsonConvert.DeserializeObject<string[]>(ScreenOrder.Value.ToString());
                if (ScreenOrderParsed != null)
                {
                    string FirstScreeName = ScreenOrderParsed[0];

                    //Select the screen
                    CanvasScreen? FirstScreen = null;
                    foreach (CanvasScreen screen in ToReturn.Screens)
                    {
                        if (screen.Name == FirstScreeName)
                        {
                            FirstScreen = screen;
                        }
                    }

                    //Not found?
                    if (FirstScreen == null)
                    {
                        throw new Exception("CanvasManifest specifies that the first screen in order is called '" + FirstScreeName + "' but I was unable to find a screen with that exact names in the parsed screens.");
                    }

                    //move it!
                    ToReturn.Screens.Remove(FirstScreen); //Remove it
                    ToReturn.Screens.Insert(0, FirstScreen); //Insert it at the front
                }
            }



            return ToReturn;

        }

    }
}