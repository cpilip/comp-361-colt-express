using System;
using System.IO;
using Newtonsoft.Json;
using PositionSpace;

namespace GameUnitSpace
{
    abstract class GameUnit
    {

        [JsonIgnore]
        private Position aPosition;
        public void setPosition(Position pPosition){
            if (this.aPosition != null)
            {
                this.aPosition.removeUnit(this);
            }

            aPosition = pPosition; 

            if(pPosition != null)
            {

                this.aPosition.addUnit(this);
            }
        }

         public Position getPosition(){
            return aPosition;
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
