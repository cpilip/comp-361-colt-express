using System;

namespace CardSpace
{
    class BulletCard : Card
    {
        private int numBullets;

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
