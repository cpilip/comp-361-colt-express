
using System;

namespace Server_Hector
{
    class Driver
    {
        static void Main(string[] args)
        {
            GameItem GI1 = new GameItem(ItemType.Purse, 200);
            Marshall myM = getInstance();

            Console.WriteLine("ALL GOOD");

        }
    }
}


