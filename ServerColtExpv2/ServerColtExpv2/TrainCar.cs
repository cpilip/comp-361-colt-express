using System.Collections.Generic;
using GameUnitSpace;
using System.IO;
using System;
using Newtonsoft.Json;

namespace PositionSpace
{
    class TrainCar
    {
        //[JsonProperty]
        private readonly bool isLocomotive;
        [JsonProperty]
        private Position inside;
        [JsonProperty]
        private Position roof;
        private bool hasAHorse;
        private int numHorses;
        public TrainCar(bool isLocomotive)
        {
            this.inside = new Position(this, Floor.Inside);
            this.roof = new Position(this, Floor.Roof);
            this.isLocomotive = isLocomotive;
            hasAHorse = false;
        }

        public Position getInside()
        {
            return this.inside;
        }

        public Position getRoof()
        {
            return this.roof;
        }

        // Move input GameUnit inside the car 
        public void moveInsideCar(GameUnit fig)
        {
            fig.setPosition(inside);
            // this.inside.addUnit(fig); **should be handled in setPosition
        }

        // Move input GameUnit inside the car 
        public void moveRoofCar(GameUnit fig)
        {
            fig.setPosition(roof);
            // this.roof.addUnit(fig); **should be handled in setPosition
        }

        // Inititialize the car's item at the beginning of the game
        // **replaces Position's "initializeRandomLayout"
        public void initializeItems(HashSet<GameItem> items)
        {
            foreach (GameItem item in items)
            {
                this.inside.addUnit(item);
            }
        }

        public void setHasAHorse(bool b){
            hasAHorse = b;
        }

        public void addAHorse()
        {
            this.numHorses++;
        }

        public void removeAHorse()
        {
            this.numHorses--;
        }

        public int getNumOfHorses()
        {
            return this.numHorses;
        }

        public bool hasHorseAtCarLevel(){
            return (numHorses == 0) ? false : true;
        }

        public void serialiazation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(filePath);
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            var defination = new
            {
                className = "TrainCar",
                inside = inside,
                isLocomotive = isLocomotive,
                roof = roof

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
