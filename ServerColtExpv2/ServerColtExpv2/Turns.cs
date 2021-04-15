using System;
using System.IO;
using Newtonsoft.Json;

namespace RoundSpace {

    enum TurnType {
        Standard,
        Tunnel,
        SpeedingUp,
        Switching,
        Turmoil
    }
    
    class Turn{
        [JsonProperty]
        private readonly TurnType type;

        public Turn(TurnType t) {
            this.type = t;
        }

        public TurnType getType() { 
            return this.type;
        }

        public void serialiazation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(filePath);
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            var defination = new
            {
                className = "Turn",
                type = type

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