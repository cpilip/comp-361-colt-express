using System;
using System.IO;
using Newtonsoft.Json;

namespace PositionSpace{


    class StageCoach : TrainCar {

        private TrainCar adjacentCar;

        public StageCoach (bool isLocomotive, TrainCar aCar) : base (isLocomotive)
        {
            adjacentCar = aCar;
        }

        public TrainCar getAdjacentCar(){
            return adjacentCar;
        }

        public void setAdjacentCar(TrainCar aTC){
            adjacentCar = aTC;
        }

        public void serialiazation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(filePath);
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            var defination = new
            {
                className = "StageCoach",
                adjacentCar = adjacentCar

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