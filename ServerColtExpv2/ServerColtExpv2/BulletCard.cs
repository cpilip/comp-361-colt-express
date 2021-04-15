using GameUnitSpace;
using Newtonsoft.Json;
using System.IO;
using System;

namespace CardSpace
{
    class BulletCard : Card
    {
        [JsonProperty]
        private readonly int numBullets;

        public BulletCard(Player pPlayer, int num) : base(assignPlayer(pPlayer))  
        {
            this.numBullets = num;
        }

        public int getNumBullets()
        {
            return this.numBullets;
        }

        public void serialiazation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(filePath);
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            var defination = new
            {
                className = "BulletCard",
                numBullets = numBullets,
                myPlayer = myPlayer


            };

            jsonSerializer.Serialize(jsonWriter, defination);
            jsonWriter.Close();
            sw.Close();
        }

        public override object deserialization<T>(string filePath)
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
