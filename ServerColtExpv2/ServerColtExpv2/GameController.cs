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

    public GameController getInstance(){
        return this;
    }


    public void chosenCharacter(Character aChar){
        
        this.players.Add(new Player(aChar));
        
        if (players.Count == totalPlayer){
            
            initializeGameBoard();
            
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

            //get method for turns
            this.currentTurn = currentRound.turns[0];

            players[0].setWaitingForInput(true);

            //this.currentPlayer = players[0];
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
        this.aMarshal = Marshal.getInstance();
        myTrain[0].moveInsideCar(aMarshal);

    }

    private void initializeLoot(){
        //TO DO
    }
    
    private void intializeRounds(){
        //TO DO
    }

    private void endOfTurn(){

        if (this.currentPlayer.isGetsAnotherAction()){

            this.currentPlayer.setsGetAnotherAction(false);
        }
        else {

            this.currentPlayer.setWaitingForInput(false);

                //not sure if this correct, why -1 ?
            if (!this.currentTurn.equals(this.currentRound.turns[-1])){
                
                if (this.currentTurn.getType().equals(TurnType.Switching)){

                    this.currentPlayerIndex = this.currentPlayerIndex - 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.currentPlayerIndex];
                }
                else {
                    this.currentPlayerIndex = this.currentPlayerIndex + 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.currentPlayerIndex];
                }

                if (this.currentTurn.getType().equals(TurnType.SpeedingUp)){
                    this.currentPlayer.setAnotherAction(true);
                }
            }
            else {
                
                foreach (Player p in this.players){
                    
                    p.moveCardsToDiscard();
                    p.setWaitingForInput(true);
                    this.aGameStatus = GameStatus.Stealin;
                }
            }
        }
    }
    
    private void endOfCars(){
        
        this.currentPlayer.discardPile.Add(this.currentRound.playedCards.pop());

        if(this.currentRound.playedCards.isEmpty()){

            if(this.currentRound.equals(this.rounds[-1])){
                calculateGameScore();
            }
            else {
                this.currentRound = this.rounds[this.rounds.IndexOf(this.currentRound + 1)];
                this.currentTurn = this.currentRound.turns[0];
                //why?
                this.currentPlayer = this.players[this.rounds.IndexOf(currentRound)];
                this.currentPlayer.setWaitingForInput(true);
                this.aGameStatus = GameStatus.Schemin;

                foreach (Player p in this.players){
                    Random rnd = new Random();
                    for (int i=0; i<6; i++){
                        //TO CHECK
                        int rand = rnd.Next(0, this.p.discardPile.Count);
                        this.p.hand.Add(this.p.discardPile[rand]);
                        this.p.discardPile.Remove(rand);
                    }
                }
            }
        }


    }

    public void playActionCard(ActionCard c){
        //TO DO
    }

    public void drawCards(){
        //TO DO
    }

    public void chosenPosition(Player answeringBandit, Position p){
        //TO DO
    }

    public void chosenPunchTarget(Player p, GameItem loot, Position dest){
        //TO DO
    }

    public void chosenShootTarget(Player target ){
        //TO DO
    }

    public void chosenLoot(Player answeringBandit, GameItem loot){
        //TO DO
    }

    // Milo TODO
    public void readyForNextMove(){
        //TO DO
    }
 
    private Player getBandits(){
        //TO DO
    }

    // Milo TODO
    private ArrayList<Position> getPossibleMoves(Player p) {
        ArrayList<Position> possPos = new ArrayList<Position>();

        // Check if on a roof or not
        if (this.position.floor == Roof) { 
            // Add 1-3 distance forward or backwards
            possPos.add();
        } else { 
            // Add adjacent positions
            possPos.add();
        }
    }

    // Milo TODO
    private ArrayList<Player> getPossibleShootTarget() {
        
    }

    // need calculateGameScore() method 

}
