
namespace GameUnitSpace
{
    class Marshal : GameUnit
    {
        private static Marshal aMarshal = new Marshal();

        public static Marshal getInstance()
        {
            return aMarshal;
        }


        public Position getPossiblePositions()
        {
            // where can he go
        }

    }

}
