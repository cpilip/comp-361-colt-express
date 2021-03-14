using System.Collections.Generic;
using Newtonsoft.Json;
using PositionSpace;

namespace GameUnitSpace
{
    class Marshal : GameUnit
    {
        [JsonProperty]
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
