using System.IO;
using Newtonsoft.Json;

namespace GameUnitSpace{

    class Shotgun : GameUnit
    {
        [JsonIgnore]
        private static Shotgun aShotGun = new Shotgun();
        [JsonProperty]
        private bool isOnStageCoach = true;

        public static Shotgun getInstance()
        {
            return aShotGun;
        }

        public bool getIsOnStageCoach(){
            return isOnStageCoach;
        }

        public void hasBeenPunched(){
            isOnStageCoach = false;
        }

        public void serialiazation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(filePath);
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            var defination = new
            {
                className = "Shotgun",
                isOnStageCoach = isOnStageCoach

            };

            jsonSerializer.Serialize(jsonWriter, defination);
            jsonWriter.Close();
            sw.Close();
        }

        public Object deserialization<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string txt = File.ReadAllText(filePath);
                //Console.WriteLine(txt);
                var obj = JsonConvert.DeserializeObject<T>(txt);
                return obj;
            }
            else
            {
                Console.WriteLine("Debug: file does not exist in deserialization");
                return null;
            }

        }


    }
}