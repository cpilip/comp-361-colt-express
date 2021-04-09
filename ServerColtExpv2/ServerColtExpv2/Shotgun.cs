
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
    
    
    }
}