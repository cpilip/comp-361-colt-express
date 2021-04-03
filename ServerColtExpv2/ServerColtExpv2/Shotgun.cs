
namespace GameUnitSpace{

    class Shotgun : GameUnit
    {
        private static Shotgun aShotGun = new Shotgun();

        public static Shotgun getInstance()
        {
            return aShotGun;
        }

        
    
    
    }
}