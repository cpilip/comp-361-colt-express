using System.Collections.Generic;
using Newtonsoft.Json;
using PositionSpace;

namespace GameUnitSpace
{
    class Marshal : GameUnit
    {
        [JsonProperty]
        private static Marshal aMarshal = new Marshal();

    }

}
