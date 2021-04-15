using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;


namespace HostageSpace
{

    public enum HostageChar
    {
        LadyPoodle,
        Banker,
        Minister,
        Teacher,
        Zealot,
        OldLady,
        PokerPlayer,
        Photographer
    }

    class Hostage
    {
        [JsonProperty]
        private readonly HostageChar aCharacter;

        private Hostage (HostageChar aC){
            aCharacter = aC;
        }
        
        public HostageChar getHostageChar(){
            return aCharacter;
        }

        //returns a list of nbOfPlayers - 1 hostages, taken at random.
        public static  List <Hostage> getSomeHostages(int nbOfPlayers){


            List <Hostage> aList = new List<Hostage>();
            aList.Add(new Hostage(HostageChar.Banker));
            aList.Add(new Hostage(HostageChar.LadyPoodle));
            aList.Add(new Hostage(HostageChar.Minister));
            aList.Add(new Hostage(HostageChar.Teacher));
            aList.Add(new Hostage(HostageChar.PokerPlayer));
            aList.Add(new Hostage(HostageChar.Zealot));
            aList.Add(new Hostage(HostageChar.OldLady));
            aList.Add(new Hostage(HostageChar.Photographer));

            List <Hostage> bList = new List<Hostage>();
            
            //TESTING
            bList.Add(new Hostage(HostageChar.Photographer));
            bList.Add(new Hostage(HostageChar.PokerPlayer));
            /*
            Random rnd = new Random ();
            int rand = rnd.Next(0,8);
            
            for (int i=0; i<nbOfPlayers-1; i++){
                bList.Add(aList[(rand+i) % 8]);
            }
            */
            return bList;
        }

        public void serialiazation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(filePath);
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            var defination = new
            {
                className = "Hostage",
                aCharacter = aCharacter


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