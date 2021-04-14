using System;
using System.Collections.Generic;

using PositionSpace;
using CardSpace;
using Newtonsoft.Json;
using System.Linq;
using HostageSpace;

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
       [JsonProperty]
        private readonly Character bandit;
        [JsonIgnore]
        private bool waitingForInput;
        [JsonIgnore]
        private bool getsAnotherAction;
        [JsonIgnore]
        private int numOfBulletsShot;
        [JsonIgnore]
        public List <Card> hand;
        [JsonIgnore]
        public List <Card> discardPile;
        [JsonIgnore]
        public List <BulletCard> bullets;
        [JsonIgnore]
        public List<GameItem> possessions;
        [JsonIgnore]
        private Hostage capturedHostage;
        [JsonIgnore]
        private bool onAHorse;
        private bool hasSpecialAbility;

        /// Constructor for the Player class, initializes a Player object.
        public Player(Character c) {

            // Initialize the character
            this.bandit = c;

            // Initialize the cards
            this.initializeCards();

            // Initialize the possessions
            possessions = new List <GameItem>();
            numOfBulletsShot = 0;

            possessions.Add(new GameItem(ItemType.Purse, 250));

            capturedHostage = null;

            //TODO may update this
            onAHorse=true;

            hasSpecialAbility=true;
        }

        /** 
        * Private helper method
        */

        private void initializeCards() {
            // Create and add 6 bullet cards
            bullets = new List<BulletCard>();

            for (int i = 1 ; i <= 6 ;i++) {
                bullets.Add(new BulletCard(this, i));
            }
            discardPile = new List<Card>();
            hand = new List<Card>();
            // Create and add all necessary action cards
            discardPile.Add(new ActionCard(this, ActionKind.Move));
            discardPile.Add(new ActionCard(this, ActionKind.Move));
            discardPile.Add(new ActionCard(this, ActionKind.ChangeFloor));
            discardPile.Add(new ActionCard(this, ActionKind.ChangeFloor));
            discardPile.Add(new ActionCard(this, ActionKind.Shoot));
            discardPile.Add(new ActionCard(this, ActionKind.Shoot));
            discardPile.Add(new ActionCard(this, ActionKind.Punch));
            discardPile.Add(new ActionCard(this, ActionKind.Rob));
            discardPile.Add(new ActionCard(this, ActionKind.Rob));
            discardPile.Add(new ActionCard(this, ActionKind.Marshal));
            discardPile.Add(new ActionCard(this, ActionKind.Ride));
        }

        /**
        * Getters and Setters
        */

        /// Returns the type of the selected bandit
        public Character getBandit() {
            return this.bandit;
        }
        
        public void setCapturedHostage(Hostage h){
            capturedHostage = h;
        }

        public Hostage getHostage(){
            return capturedHostage;
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

        public void setOnAHorse(bool b){
            onAHorse = b;
        }

        public bool isPlayerOnAHorse(){
            return onAHorse;
        }

        public void setHasSpecialAbility(bool b){
            hasSpecialAbility = b;
        } 

        public bool getHasSpecialAbility(){
            return hasSpecialAbility;
        }


        /// Update the state of the get another action flag.
        public void setGetsAnotherAction(Boolean getAnotherAction) {
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
       
        public void moveFromHandToDiscard(Card c) {
            this.discardPile.Add(c);
        }

        public void moveCardsToDiscard() {
            foreach (Card c in this.hand) {
                
                moveFromHandToDiscard(c);
            }

            this.hand.Clear();
        }

        public int getPossesionsValue() {
            int total = 0;
            foreach (GameItem it in this.possessions) {
                total =  total + it.getValue();
            }
            return total;
        }

        public int getLeastPurseValue() {
            int min = 10000;
            foreach (GameItem i in this.possessions) {
                if (i.getType() == ItemType.Purse && i.getValue() < min) {
                    min = i.getValue();
                }
            }
            if (min == 10000) return 0;
            else return min;
        }

        public int getHostageValue()
        {
            int val = 0;
            if (capturedHostage != null)
            {
                switch (capturedHostage.getHostageChar())
                {
                    case HostageChar.LadyPoodle:
                        {
                            val += 1000;
                            return val;
                        }
                    case HostageChar.Minister:
                        {
                            val += 900;
                            return val;
                        }
                    case HostageChar.Teacher:
                        {
                            val += 800;
                            return val;
                        }
                    case HostageChar.Zealot:
                        {
                            val += 700;
                            return val;
                        }
                    case HostageChar.Banker:
                        {
                            if (this.hasStrongBox())
                            {
                                val += 900;
                            }
                            return val;
                        }
                    case HostageChar.OldLady:
                        {
                            val += (this.getNumOfItem(ItemType.Ruby) * 500);
                            return val;
                        }
                    case HostageChar.PokerPlayer:
                        {
                            val += (this.getNumOfItem(ItemType.Purse) * 250);
                            return val;
                        }
                    case HostageChar.Photographer:
                        {
                            val += (this.getNumOfEnemyBulletCard() * 200);
                            return val;
                        }
                    default:
                        {
                            return val;
                        }
                }
            }
            return val;
        }
        
        public void shootBullet() {
            this.numOfBulletsShot = this.numOfBulletsShot + 1;
        }

        public int getNumOfBulletsShot() {
            return this.numOfBulletsShot;
        }

        //For sending Action cards from the hand as a list of enumerations of ActionKind
        public List<ActionKind> getHand_actionCards()
        {
            List<ActionKind> l = new List<ActionKind>();
            this.hand.OfType<ActionCard>().ToList()
                .ForEach(i => l.Add(i.getKind()));
            return l;
        }

        //For sending the number of Bullet cards
        public int getHand_bulletCards()
        {
            return this.hand.OfType<BulletCard>().ToList().Count;
        }

        //Return the first full whiskey
        public Whiskey getAWhiskey(){
           foreach (GameItem g in possessions){
               if (g is Whiskey){
                    if (((Whiskey)g).getWhiskeyStatus() == WhiskeyStatus.Full)
                   return (Whiskey) g;
               }
           }
           return null;

        }

        //Return the first whiskey of desired kind that is half
        public Whiskey getAWhiskey(WhiskeyKind desiredKind)
        {
            foreach (GameItem g in possessions)
            {
                if (g is Whiskey)
                {
                    if (((Whiskey)g).getWhiskeyKind() == desiredKind && ((Whiskey)g).getWhiskeyStatus() == WhiskeyStatus.Half)
                    {
                        return (Whiskey)g;
                    }
                }
            }
            return null;

        }

        public void removeWhiskey(Whiskey aW){
            possessions.Remove(aW);
        }

        public void addWhiskey(Whiskey aW){
            possessions.Add(aW);
        }

        public void actionCantBePlayedinHand(ActionKind aKind){
            foreach (ActionCard c in hand){
                if (c.GetType() == typeof(ActionCard))
                {
                    if (((ActionCard)c).getKind().Equals(aKind))
                    {
                        ((ActionCard)c).cantBePlayedAnymore();
                    }
                }
            }
        }

        public void actionCantBePlayed(ActionKind aKind){
            foreach (Card c in discardPile){
                if (c.GetType() == typeof(ActionCard))
                {
                    if (((ActionCard)c).getKind().Equals(aKind))
                    {
                        ((ActionCard)c).cantBePlayedAnymore();
                    }
                }
            }
        }

        /**
            Private helper functions to calculate the ransom price of each hostage 
        */
        private Boolean hasStrongBox(){
            foreach (GameItem t in possessions){
                if(t.getType().Equals(ItemType.Strongbox)){
                    return true;
                }
            }
            return false;
        }
       
        private int getNumOfItem(ItemType anItemType){
            int counter = 0;
            foreach (GameItem t in possessions){
                if(t.getType().Equals(anItemType)){
                    counter ++;
                }
            }
            return counter;
        } 
      
        private int getNumOfEnemyBulletCard(){
            int counter = 0;
            foreach (Card c in discardPile){
                if(c.GetType().Equals(typeof(BulletCard))){
                    counter ++;
                }
            }
            return counter;
        }
    }

}