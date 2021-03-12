using System;
using System.Collections;


namespace Player {

    public enum Character {
        Ghost, 
        Doc, 
        Tuco,
        Cheyenne, 
        Belle,
        Django
    }

    public class Player : GameUnit{
        private readonly Character bandit;
        private bool waitingForInput;
        private bool getsAnotherAction;

        public ArrayList<Card> hand;
        public ArrayList<Card> discardPile;
        public ArrayList<BulletCard> bullets;

        public ArrayList<GameItem> possessions;

        /// Constructor for the Player class, initializes a Player object.
        public Player(Character c, Position p) {

            // Initialize the character
            this.bandit = c;

            // Initialize the cards
            this.initializeCards();

            // Initialize the position of the Player.
            this.position = p;

            // Initialize the possessions
            possessions = new ArrayList<GameItem>();
        }

        /**
        * Private helper method
        */

        private void initializeCards() {
            // Create and add 6 bullet cards
            for (int i = 0 ; i < 6 ;i++) {
                bullets.add(new BulletCard(this));
            }

            // Create and add all necessary action cards
            discardPile.add(new ActionCard(Move));
            discardPile.add(new ActionCard(Move));
            discardPile.add(new ActionCard(ChangeFloor));
            discardPile.add(new ActionCard(ChangeFloor));
            discardPile.add(new ActionCard(Shoot));
            discardPile.add(new ActionCard(Shoot));
            discardPile.add(new ActionCard(Punch));
            discardPile.add(new ActionCard(Loot));
            discardPile.add(new ActionCard(Loot));
            discardPile.add(new ActionCard(Marshall));

        }

        /**
        * Getters and Setters
        */

        /// Returns the type of the selected bandit
        public void getBandit() {
            return this.bandit;
        }

        /// Set the state of waiting for input flag.
        public void setWaitingForInput(bool waitingForInput) {
            this.waitingForInput = waitingForInput;
        }

        /// Returns the current state of the waiting for input flag.
        public boolean getWaitingForInput() {
            return this.waitingForInput;
        }

        /// Returns the current state of the gets another action flag.
        public boolean isGetsAnotherAction() {
            return this.getsAnotherAction;
        }

        /// Update the state of the get another action flag.
        public void setGetsanotherAction(boolean getAnotherAction) {
            this.getsAnotherAction = getAnotherAction;
        }

        public void addToDiscardPile(Card c) { 
            this.discardPile.add(c);
        }

    }

}