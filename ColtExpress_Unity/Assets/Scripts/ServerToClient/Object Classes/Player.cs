using System;
using System.Collections.Generic;

using PositionSpace;
using CardSpace;
using Newtonsoft.Json;
using UnityEngine;

namespace GameUnitSpace
{

    public enum Character
    {
        Ghost,
        Doc,
        Tuco,
        Cheyenne,
        Belle,
        Django,
        Marshal,
        Shotgun
    }

    class Player : GameUnit
    {
        [JsonProperty]
        private readonly Character bandit;
        //[JsonProperty]
        private bool waitingForInput;
        //[JsonProperty]
        private bool getsAnotherAction;
        //[JsonProperty]
        private int numOfBulletsShot;

        public List<System.Object> hand;
        public List<System.Object> discardPile;
        public List<BulletCard> bullets;
        public List<GameItem> possessions;

        /// Constructor for the Player class, initializes a Player object.
        public Player(Character c)
        {

            // Initialize the character
            this.bandit = c;

            // Initialize the cards
            this.initializeCards();

            // Initialize the possessions
            possessions = new List<GameItem>();
            numOfBulletsShot = 0;

            possessions.Add(new GameItem(ItemType.Purse, 250));
        }

        /**
        * Private helper method
        */

        private void initializeCards()
        {
            // Create and add 6 bullet cards
            

        }

        /**
        * Getters and Setters
        */

        /// Returns the type of the selected bandit
        public Character getBandit()
        {
            return this.bandit;
        }

        /// Set the state of waiting for input flag.
        public void setWaitingForInput(bool waitingForInput)
        {
            this.waitingForInput = waitingForInput;
        }

        /// Returns the current state of the waiting for input flag.
        public Boolean getWaitingForInput()
        {
            return this.waitingForInput;
        }

        /// Returns the current state of the gets another action flag.
        public Boolean isGetsAnotherAction()
        {
            return this.getsAnotherAction;
        }

        /// Update the state of the get another action flag.
        public void setGetsAnotherAction(Boolean getAnotherAction)
        {
            this.getsAnotherAction = getAnotherAction;
        }

        public void addToDiscardPile(Card c)
        {
            this.discardPile.Add(c);
        }

        public void moveFromDiscardToHand(Card c)
        {
            this.discardPile.Remove(c);
            this.hand.Add(c);
        }

        public BulletCard getABullet()
        {
            BulletCard tmp = this.bullets[0];
            this.bullets.RemoveAt(0);
            return tmp;
        }

        public void addToPossessions(GameItem anItem)
        {
            this.possessions.Add(anItem);
        }
        public void moveFromHandToDiscard(Card c)
        {
            this.hand.Remove(c);
            this.discardPile.Add(c);
        }

        public void moveCardsToDiscard()
        {
            foreach (Card c in this.hand)
            {
                moveFromHandToDiscard(c);
            }
        }

        public int getPossesionsValue()
        {
            int total = 0;
            foreach (GameItem it in this.possessions)
            {
                total = total + it.getValue();
            }
            return total;
        }

        public void shootBullet()
        {
            this.numOfBulletsShot = this.numOfBulletsShot + 1;
        }

        public int getNumOfBulletsShot()
        {
            return this.numOfBulletsShot;
        }
    }
}