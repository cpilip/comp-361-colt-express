using Newtonsoft.Json;
using System;

namespace CardSpace
{
    class BulletCard : Card
    {
        [JsonProperty]
        private readonly int numBullets;

        public BulletCard(int num)
        {
            this.numBullets = num;
        }

        public int getNumBullets()
        {
            return this.numBullets;
        }
    }
}
