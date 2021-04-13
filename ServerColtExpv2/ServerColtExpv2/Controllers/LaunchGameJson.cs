using System;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    class LaunchGameJson
    {
        public string creator;
        public string gameServer;
        public List<UserObject> players;
        public string savegame;
    }

    class UserObject
    {
        public string name;
        public string preferredColour;
    }
}
