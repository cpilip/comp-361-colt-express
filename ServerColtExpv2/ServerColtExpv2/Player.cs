using System;
using System.Collections.Generic;

using PositionSpace;
using CardSpace;


namespace GameUnitSpace {

    public enum Character {
        Ghost, 
        Doc, 
        Tuco,
        Cheyenne, 
        Belle,
        Django
    }

    class Player : GameUnit{
        private readonly Character bandit;
        private bool waitingForInput;
        private bool getsAnotherAction;

        public List <Card> hand;
        public List <Card> discardPile;
        public List <BulletCard> bullets;
        public List<GameItem> possessions;

        /// Constructor for the Player class, initializes a Player object.
        public Player(Character c) {

            // Initialize the character
            this.bandit = c;

            // Initialize the cards
            this.initializeCards();

            // Initialize the possessions
            possessions = new List <GameItem>();
        }

        /**
        * Private helper method
        */

        private void initializeCards() {
            // Create and add 6 bullet cards
            for (int i = 0 ; i < 6 ;i++) {
                bullets.Add(new BulletCard());
            }

            // Create and add all necessary action cards
            discardPile.Add(new ActionCard(ActionKind.Move));
            discardPile.Add(new ActionCard(ActionKind.Move));
            discardPile.Add(new ActionCard(ActionKind.ChangeFloor));
            discardPile.Add(new ActionCard(ActionKind.ChangeFloor));
            discardPile.Add(new ActionCard(ActionKind.Shoot));
            discardPile.Add(new ActionCard(ActionKind.Shoot));
            discardPile.Add(new ActionCard(ActionKind.Punch));
            discardPile.Add(new ActionCard(ActionKind.Rob));
            discardPile.Add(new ActionCard(ActionKind.Rob));
            discardPile.Add(new ActionCard(ActionKind.Marshal));

        }

        /**
        * Getters and Setters
        */

        /// Returns the type of the selected bandit
        public Character getBandit() {
            return this.bandit;
        }

        /// Set the state of waiting for input flag.
        public void setWaitingForInput(bool waitingForInput) {
            this.waitingForInput = waitingForInput;
        }

        /// Returns the current state of the waiting for input flag.
        public Boolean getWaitingForInput() {
            return this.waitingForInput;
        }

        /// Returns the current state of the gets another action flag.
        public Boolean isGetsAnotherAction() {
            return this.getsAnotherAction;
        }

        /// Update the state of the get another action flag.
        public void setGetsanotherAction(Boolean getAnotherAction) {
            this.getsAnotherAction = getAnotherAction;
        }

        public void addToDiscardPile(Card c) { 
            this.discardPile.Add(c);
        }

        public void moveFromDiscardToHand(Card c) {
            this.discardPile.Remove(c);
            this.hand.Add(c);
        }

        public BulletCard getABullet(){
            BulletCard tmp = this.bullets[0];
            this.bullets.RemoveAt(0);
            return tmp;
        }

        public void addToPossessions(GameItem anItem){
            this.possessions.Add(anItem);
        }
    }

}