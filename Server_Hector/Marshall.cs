
namespace Server_Hector
{
    class Marshall : GameUnit
    {
        private static Marshall aMarshall = new Marshall();

        public static Marshall getInstance()
        {
            return aMarshall;
        }


        public Position getPossiblePositions()
        {
            // where can he go
        }

    }

}
