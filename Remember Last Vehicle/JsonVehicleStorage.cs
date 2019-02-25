using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Remember_Last_Vehicle
{
    public static class JsonVehicleStorage
    {
        private static string BasePath = string.Empty;
        private static string FileName = "saved-vehicles.json";

        public static void InitializeBasePath(string path)
        {
            //Creëer het pad wanneer deze nog niet bestaat
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //Zet het basispad met het pad als argument
            BasePath = path;
        }

        public static void SaveVehicle(SaveableVehicle saveableVehicle)
        {
            //Gooi een exception waneer het basispad niet is geinitialiseerd
            ThrowExceptionWhenBasePathDoesNotExist();

            //Haal opgeslagen auto's op
            var vehicles = GetVehicles().ToList();

            //Voeg auto om op te slaan toe aan de andere opgeslagen auto's
            vehicles.Add(saveableVehicle);

            //Zet de opgeslagen auto's om naar json
            var json = JsonConvert.SerializeObject(vehicles, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            //Schrijf de json string naar het bestand
            File.WriteAllText(GetFilePath(), json);
        }

        public static void RemoveVehicle(SaveableVehicle saveableVehicle)
        {
            //Gooi een exception waneer het basispad niet is geinitialiseerd
            ThrowExceptionWhenBasePathDoesNotExist();

            //Haal opgeslagen auto's op
            var vehicles = GetVehicles().ToList();

            //Verwijder de auto om te verwijderen uit de lijst van opgeslagen auto's
            vehicles.RemoveAll(x => x == saveableVehicle);

            //Sla de auto's weer in het bestand
            ConvertAndWriteVehiclesToFile(vehicles);
        }

        public static void ConvertAndWriteVehiclesToFile(IEnumerable<SaveableVehicle> vehicles)
        {
            //Zet de opgeslagen auto's om naar json
            var json = JsonConvert.SerializeObject(vehicles, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            //Schrijf de json string naar het bestand
            File.WriteAllText(GetFilePath(), json);
        }

        public static void ThrowExceptionWhenBasePathDoesNotExist()
        {
            //Gooi een exception waneer het basispad niet is geinitialiseerd
            if(BasePath == string.Empty)
            {
                throw new BasePathNotInitialized();
            }
        }

        public static SaveableVehicle[] GetVehicles()
        {
            try
            {
                //Haal de json string op uit de file
                var json = File.ReadAllText(GetFilePath());

                //Zet de json string om naar auto objecten
                return JsonConvert.DeserializeObject<SaveableVehicle[]>(json, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            catch
            {
                //Als er een exception wordt gegooid wordt er een lege lijst teruggegeven
                return new SaveableVehicle[0];
            }
        }

        private static string GetFilePath()
        {
            //Geeft het pad terug van de file met de json string erin
            return $"{BasePath.TrimEnd('/')}/{FileName}";
        }

        public class BasePathNotInitialized : Exception { }
    }
}