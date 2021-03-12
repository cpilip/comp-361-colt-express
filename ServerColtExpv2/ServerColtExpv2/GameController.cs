using System;
using System.Collections.Generic;

enum GameStatus {
    ChoosingBandits,
    Schemin,
    Stealin,
    FinilizingCard,
    Completed
}

class GameController {
    
    private static GameController myGameController = new GameController();
    private readonly int totalPlayer;
    private GameStatus aGameStatus;
    private Round currentRound;
    private Turn currentTurn;
    private List <Round> rounds;
    private List <Player> players; 
    private Player currentPlayer;
    private int currentPlayerIndex;
    //added this for Switching turn
    private List <TrainCar> myTrain;
    private Marshal aMarshal;



    private GameController(){
        this.players = new List <Player> ();
        this.myTrain = new List <TrainCar> ();
        this.rounds = new List <Round> ();
    }

    public static GameController getInstance(){
        return this;
    }


    /**
        * Public utility methods
    */

    public void chosenCharacter(Character aChar){
        
        //adding a new player to the list of players 
        this.players.Add(new Player(aChar));
        
        //if all players are here 
        if (players.Count == totalPlayer){
            
            initializeGameBoard();
            
            //setting players' positions
            for(int i=0; i<myTrain.Count; i++){
                
                if (i%2==0){
                    myTrain[myTrain.Count - 2].moveInsideCar(players[i]);
                }
                else {
                    myTrain[myTrain.Count - 1].moveInsideCar(players[i]);
                }

            }
            
            initializeLoot();

            intializeRounds();

            this.aGameStatus = GameStatus.Schemin;

            this.currentRound =rounds[0];

            //TODO get method for turns
            this.currentTurn = currentRound.turns[0];

            players[0].setWaitingForInput(true);

            this.currentPlayer = players[0];
        }
    }

    public void playActionCard(ActionCard c){
        
        //adding the action card to the playedCard pile and removind it from player's hand
        //TODO use get method 
        this.currentRound.playedCards.Add(c);
        //TODO use get method 
        this.currentPlayer.hand.Remove(c);

        endOfTurn();
    }

    public void drawCards(){
        Random rnd = new Random();
        
        //taking three random cards from player's discardPile and adding them to the player's hand
        for (int i=0; i<3; i++){
        
            //TODO use get methods for discardPile and hand 
            int rand = rnd.Next(0,this.currentPlayer.discardPile.Count);
            this.currentPlayer.hand.Add(p.discardPile[rand]);
            this.currentPlayer.discardPile.Remove(rand);
        }
        //in TouchCore was EndOfMove()
        endOfTurn();
    }

    public void chosenPosition(Position p){
        //TODO is played card a Stack or a List ?
        Card topOfPile = this.currentRound.playedCards.pop();
        
        //if the action card is a Move Marshall action
        if (topOfPile.getKind == Card.ActionKind.Marshall){ 
            this.aMarshal.setPosition(p);

            //check for all game units at position p 
            foreach (GameUnit g in p.getUnits()){
         
                //if it is a player, bullet card in deck + sent to the roof 
                if (g.getType() == typeof(Player)){
                    Bulletcard b = new Bulletcard();
                    g.discardPile.Add(b);
                    g.setPosition(p.TrainCar.roof);
                }
            }
        }
        //if the action card is a Move action
        if (topOfPile.getKind == Card.ActionKind.Move){
            currentPlayer.setPosition(p);

            //if the marshal is at position p, bullet card in deck + sent to the roof 
            if(p.units.contains(this.aMarshal)){
                    
                    Bulletcard b = new Bulletcard();
                    currentPlayer.discardPile.Add(b);
                    currentPlayer.setPosition(p.TrainCar.roof);
            }
        }

        endOfCards();

    }

    public void chosenPunchTarget(Player victim, GameItem loot, Position dest){
        
        //drop the loot at victim position, sends victim to destination 
        loot.setPosition(victim.getPosition());
        victim.setPosition(dest);

        //TODO use get method
        //loot is removed from victime possessions
        victim.possessions.Remove(loot);

        //if the marshal is at position dest, bullet card in deck + sent to the roof 
        if (dest.units.contains(this.aMarshal)){
                Bulletcard b = new Bulletcard();
                victim.discardPile.Add(b);
                victim.setPosition(dest.TrainCar.roof);
        }
       
        endOfCards();
    }

    public void chosenShootTarget(Player target){
        //TOCHECK name of get, add methods 
        //A BulletCard is transfered from bullets of currentPlayer to target's discsrdPile
        BulletCard aBullet = currentPlayer.getBullets()[0];
        currentPlayer.getBullets().Remove(aBullet);
        target.getDiscardPile().Add(aBullet);
        endOfCards();
    }

    public void chosenLoot(GameItem loot){
        //the loot is transfered from the position to the currentPlayer possensions
        loot.setPosition(null);
        currentPlayer.getPossessions().Add(loot);
        endOfCards();
    }

    public void readyForNextMove(){
        //TO DO
    }
 


    /**
        Private helper methods
    */

    private void endOfTurn(){

        //if the player has another action, then the anotherAction flag is set to false
        if (this.currentPlayer.isGetsAnotherAction()){

            this.currentPlayer.setsGetAnotherAction(false);
        }

        else {
            this.currentPlayer.setWaitingForInput(false);

            //if this is not the last turn of the round
            //TODO use get method for turns 
            if (!this.currentTurn.equals(this.currentRound.turns[this.currentRound.turns.Count-1])){
                
                //determining the next player 
                //if the turn is Switching, order of players is reversed
                if (this.currentTurn.getType().equals(TurnType.Switching)){

                    this.currentPlayerIndex = this.currentPlayerIndex - 1 % this.totalPlayer;
                    //don't use playerIndex
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer)];
                }
                //otherwise, it is the next player in the list 
                else {
                    this.currentPlayerIndex = this.currentPlayerIndex + 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer)];
                }

                //if the turn is Speeding up, the next player has another action 
                if (this.currentTurn.getType().equals(TurnType.SpeedingUp)){
                    this.currentPlayer.setAnotherAction(true);
                }
            }
            // if it is the last turn of the round 
            else {
                
                //prepare for Stealing phase 
                foreach (Player p in this.players){
                    
                    p.moveCardsToDiscard();
                    p.setWaitingForInput(true);
                    this.aGameStatus = GameStatus.Stealin;
                }
            }
        }
    }
    
    private void endOfCards(){
        
        //TODO use get method for playedCards 
        //TODO is played Card a stack ?
        this.currentPlayer.discardPile.Add(this.currentRound.playedCards.pop());

        //if all cards in the pile have been played 
        //TODO use get method for playedCards 
        if(this.currentRound.playedCards.isEmpty()){

            //if this is the last round 
            if(this.currentRound.equals(this.rounds[-1])){
                calculateGameScore();
            }
            else {
                //setting the next round, setting the first turn of the round 
                this.currentRound = this.rounds[this.rounds.IndexOf(this.currentRound + 1)];
                //TODO get method for turns
                this.currentTurn = this.currentRound.turns[0];

                //setting the next player and game status of the game 
                this.currentPlayer = this.players[this.rounds.IndexOf(currentRound)];
                this.currentPlayer.setWaitingForInput(true);
                this.aGameStatus = GameStatus.Schemin;

                //for each player, getting 6 cards from their Pile at randomn and adding them to their hand 
                foreach (Player p in this.players){
                    Random rnd = new Random();
                    for (int i=0; i<6; i++){
                        //TODO use get method for discardPile and hand
                        int rand = rnd.Next(0, p.discardPile.Count);
                        p.hand.Add(p.discardPile[rand]);
                        p.discardPile.Remove(rand);
                    }
                }
            }
        }


    }

    private void initializeGameBoard(){
        
        //initializing Locomotive
        this.myTrain.Add(new TrainCar(true));

        //initializing train cars
        for (int i=0; i<myTrain.Count; i++){
            this.myTrain.Add(new TrainCar(false));
        }

        // initializing the marshall and init his position
        this.aMarshal = Marshall.getInstance();
        myTrain[0].moveInsideCar(aMarshal);

    }
     
    private void initializeLoot(){
        //TO DO
    }
    
    private void intializeRounds(){
        //TO DO
    }

    private Player getBandits(){
        //TO DO
    }

    private void calculateGameScore(){

    }
}
