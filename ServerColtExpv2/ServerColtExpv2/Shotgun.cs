
namespace GameUnitSpace{

    class Shotgun : GameUnit
    {
        private static Shotgun aShotGun = new Shotgun();
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