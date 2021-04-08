using GameUnitSpace;
using Newtonsoft.Json;
using System;

namespace CardSpace
{
    class BulletCard : Card
    {
        [JsonProperty]
        private readonly int numBullets;

        public BulletCard(Player pPlayer, int num) : base(assignPlayer(pPlayer))
        {
            this.numBullets = num;
        }

        public int getNumBullets()
        {
            return this.numBullets;
        }
    }
}
