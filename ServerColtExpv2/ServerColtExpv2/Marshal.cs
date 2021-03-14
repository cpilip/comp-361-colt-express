using System.Collections.Generic;

using PositionSpace;

namespace GameUnitSpace
{
    class Marshal : GameUnit
    {
        private static Marshal aMarshal = new Marshal();

        public static Marshal getInstance()
        {
            return aMarshal;
        }


        public List<Position> getPossiblePositions()
        {
            List<Position> possPos = new List<Position>();
            return possPos;
        }

    }

}
