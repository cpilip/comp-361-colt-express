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
        public ArrayList<Bulletcard> bullets;

        public ArrayList<GameItem> possessions;

        /// Constructor for the Player class, initializes a Player object.
        public Player(Character c) {
            this.bandit = c;
            this.initializeCards();
        }

        public Position getPossibleMoves() {
            // TODO
        }

        public Player getPossibleShootTarget() {

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


    }

}