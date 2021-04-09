using System.Collections.Generic;
using Newtonsoft.Json;
using PositionSpace;

namespace GameUnitSpace
{
    class Marshal : GameUnit
    {
        [JsonIgnore]
        private static Marshal aMarshal = new Marshal();

        public static Marshal getInstance()
        {
            return aMarshal;
        }

    }

}
