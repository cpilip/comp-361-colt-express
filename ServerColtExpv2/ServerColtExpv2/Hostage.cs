using System;
using System.Collections.Generic;


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
            
            Random rnd = new Random ();
            int rand = rnd.Next(0,8);
            
            for (int i=0; i<nbOfPlayers-1; i++){
                bList.Add(aList[(rand+i) % 8]);
            }
            
            return bList;
        }


    }


}