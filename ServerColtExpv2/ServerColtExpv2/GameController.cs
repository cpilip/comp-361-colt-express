using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

using RoundSpace;
using CardSpace;
using GameUnitSpace;
using PositionSpace;


enum GameStatus
{
    ChoosingBandits,
    Schemin,
    Stealin,
    FinalizingCard,
    Completed
}

class GameController
{

    private static GameController myGameController = new GameController();
    private readonly int totalPlayer;
    private GameStatus aGameStatus;
    private Round currentRound;
    private Turn currentTurn;
    private List<Round> rounds;
    private List<Player> players;
    private Player currentPlayer;
    private int currentPlayerIndex;
    //added this for Switching turn
    private List<TrainCar> myTrain;
    private Marshal aMarshal;

    private GameController()
    {
        this.players = new List<Player>();
        this.myTrain = new List<TrainCar>();
        this.rounds = new List<Round>();
    }

    public static GameController getInstance()
    {
        return myGameController;
    }


    /**
        * Public utility methods
    */

    public void chosenCharacter(Character aChar)
    {

        //adding a new player to the list of players 
        this.players.Add(new Player(aChar));

        //if all players are here 
        if (players.Count == totalPlayer)
        {

            initializeGameBoard();

            //setting players' positions
            for (int i = 0; i < myTrain.Count; i++)
            {
                if (i%2 == 0)
                {
                    myTrain[myTrain.Count - 2].moveInsideCar(players[i]);
                }
                else
                {
                    myTrain[myTrain.Count - 1].moveInsideCar(players[i]);
                }
            }
            MyTcpListener.sendToClient(JsonConvert.SerializeObject(myTrain));

           
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient("updatePlayers", players);
            
            initializeLoot();

            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient("updateTrai", myTrain);

            intializeRounds();

            this.aGameStatus = GameStatus.Schemin;
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient("updateGameStatus", true);

            this.currentRound = rounds[0];
            //TO CHECK, do we send all rounds ?
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient("updateCurrentRound", currentRound);

            this.currentTurn = currentRound.getTurns()[0];
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient("updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));

            players[0].setWaitingForInput(true);

            this.currentPlayer = players[0];
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient("updateWaitingForInput", this.players.IndexOf(currentPlayer), currentPlayer.getWaitingForInput());
        }
    }

    public void playActionCard(ActionCard c)
    {
        
        //adding the action card to the playedCard pile and removind it from player's hand
        this.currentRound.addToPlayedCards(c);
        this.currentPlayer.hand.Remove(c);

        endOfTurn();
    }

    public void drawCards()
    {
        Random rnd = new Random();

        //taking three random cards from player's discardPile and adding them to the player's hand

        int rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
        Card c = this.currentPlayer.discardPile[rand];
        this.currentPlayer.moveFromDiscardToHand(c);

        rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
        Card c1 = this.currentPlayer.discardPile[rand];
        this.currentPlayer.moveFromDiscardToHand(c);

        rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
        Card c2 = this.currentPlayer.discardPile[rand];
        this.currentPlayer.moveFromDiscardToHand(c);


        //TO SPECIFIC PLAYER 
        CommunicationAPI.sendMessageToClient("addCards", c, c1, c2);
        
        endOfTurn();
    }

    public void chosenPosition(Position p, ActionKind aKind)
    {
        //ActionCard topOfPile = this.currentRound.topOfPlayedCards();

        //if the action card is a Move Marshall action
        if (aKind.Equals(ActionKind.Marshal))
        {
            this.aMarshal.setPosition(p);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient("moveGameUnit", this.aMarshal, p);

            //check for all players at position p 
            foreach (Player aPlayer in p.getPlayers())
            {
                BulletCard b = new BulletCard();
                aPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(aPlayer);
                
                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient("moveGameUnit", aPlayer, p.getTrainCar().getRoof());

            }
        }
        //if the action card is a Move action
        if (aKind.Equals(ActionKind.Move))
        {
            currentPlayer.setPosition(p);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient("moveGameUnit", currentPlayer, p);

            //if the marshal is at position p, bullet card in deck + sent to the roof 
            if (p.hasMarshal(aMarshal))
            {
                BulletCard b = new BulletCard();
                currentPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(currentPlayer);
                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient("moveGameUnit", currentPlayer, p.getTrainCar().getRoof());
            }
        }

        endOfCards();

    }

    public void chosenPunchTarget(Player victim, GameItem loot, Position dest)
    {

        //drop the loot at victim position, sends victim to destination 
        loot.setPosition(victim.getPosition());
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient("moveGameItem", loot, victim.getPosition()); 

        victim.setPosition(dest);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient("moveGameUnit", victim, dest); 


        //loot is removed from victime possessions
        victim.possessions.Remove(loot);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient("decrement", loot); 

        //if the marshal is at position dest, victim: bullet card in deck + sent to the roof 
        if (dest.hasMarshal(aMarshal))
        {
            BulletCard b = new BulletCard();
            victim.addToDiscardPile(b);
            dest.getTrainCar().moveRoofCar(victim);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient("moveGameUnit", victim, dest.getTrainCar().getRoof()); 
        }

        endOfCards();
    }

    public void chosenShootTarget(Player target)
    {
        //A BulletCard is transfered from bullets of currentPlayer to target's discardPile
        BulletCard aBullet = currentPlayer.getABullet();
        target.addToDiscardPile(aBullet);
        this.currentPlayer.shootBullet();
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient("decrement", this.currentPlayer.bullets);
        endOfCards();
    }

    public void chosenLoot(GameItem loot)
    {
        //the loot is transfered from the position to the currentPlayer possensions
        loot.setPosition(null);
        currentPlayer.addToPossessions(loot);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient("increment", loot);
        endOfCards();
    }

    public void readyForNextMove()
    {
        this.currentPlayer.setWaitingForInput(false);
        Boolean waiting = true;

        foreach (Player p in this.players)
        {
            if (p.getWaitingForInput())
                waiting = false;
            
        }

        if (waiting)
        {
            // Get the top of the played cards from the schemin phase
            ActionCard top = this.currentRound.topOfPlayedCards();

            switch (top.getKind())
            {
                case ActionKind.Move:
                    {
                        List<Position> moves = this.getPossibleMoves(this.currentPlayer);

                        if (moves.Count > 1)
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYERS
                            CommunicationAPI.sendMessageToClient("updateMovePositions", moves);
                        }
                        else
                        {
                            chosenPosition(moves[0], ActionKind.Move);
                        }
                        break;
                    }
                case ActionKind.ChangeFloor:
                    {
                        if (this.currentPlayer.getPosition().isInside())
                        {
                            this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                            //TO ALL PLAYERS
                            CommunicationAPI.sendMessageToClient("moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getRoof());
                        }
                        else
                        {
                            this.currentPlayer.getPosition().getTrainCar().moveInsideCar(this.currentPlayer);
                            CommunicationAPI.sendMessageToClient("moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getInside());

                            if (this.currentPlayer.getPosition().hasMarshal(this.aMarshal))
                            {
                                this.currentPlayer.addToDiscardPile(new BulletCard());
                                this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                                CommunicationAPI.sendMessageToClient("moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getRoof());
                            }
                        }
                        break;
                    }
                case ActionKind.Shoot:
                    {
                        List<Player> possTargets = this.getPossibleShootTarget(this.currentPlayer);
                        if (possTargets.Count == 1)
                        {
                            this.chosenShootTarget(possTargets[0]);
                        }
                        else
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            CommunicationAPI.sendMessageToClient("updatePossTarget", possTargets);
                        }
                        break;
                    }
                case ActionKind.Rob:
                    {
                        //TODO Do we need to check if there is olny one loot ?
                        List<GameItem> atLocation = this.currentPlayer.getPosition().getItems();
                        this.aGameStatus = GameStatus.FinalizingCard;
                        this.currentPlayer.setWaitingForInput(true);
                        CommunicationAPI.sendMessageToClient("updateLootAtLocation", atLocation);
                        break;
                    }
                case ActionKind.Marshal:
                    {
                        List<Position> possPosition = this.aMarshal.getPossiblePositions();
                        if (possPosition.Count == 1)
                        {
                            this.chosenPosition(possPosition[0], ActionKind.Marshal);
                        }
                        else
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            CommunicationAPI.sendMessageToClient("updateMovePositions", possPosition);

                        }
                        break;
                    }
                case ActionKind.Punch:
                    {
                        //TODO Do we need to check if there is olny one loot ?
                        List<Player> atLocation = this.currentPlayer.getPosition().getPlayers();
                        this.aGameStatus = GameStatus.FinalizingCard;
                        this.currentPlayer.setWaitingForInput(true);
                        CommunicationAPI.sendMessageToClient("updatePossTarget", atLocation);
                        break;
                    }
            }
            this.endOfCards();
        }
    }



    /**
        Private helper methods
    */

    private void endOfTurn()
    {

        //if the player has another action, then the anotherAction flag is set to false
        if (this.currentPlayer.isGetsAnotherAction())
        {
            CommunicationAPI.sendMessageToClient("updateHasAnotherAction", currentPlayerIndex, true);
            this.currentPlayer.setGetsAnotherAction(false);
        }

        else
        {
            this.currentPlayer.setWaitingForInput(false);
            CommunicationAPI.sendMessageToClient("updateWaitingForInput", currentPlayerIndex, false);

            //if this is not the last turn of the round
            if (!this.currentTurn.Equals((this.currentRound.getTurns()[this.currentRound.getTurns().Count - 1])))
            {

                //determining the next player 
                //if the turn is Switching, order of players is reversed, so next player is previous in the list
                if (this.currentTurn.getType() == TurnType.Switching)
                {
                    this.currentPlayerIndex = this.currentPlayerIndex - 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer) - 1 % this.totalPlayer];
                    CommunicationAPI.sendMessageToClient("updateCurrentPlayer", currentPlayerIndex);
                }
                //otherwise, it is the next player in the list 
                else
                {
                    this.currentPlayerIndex = this.currentPlayerIndex + 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer) + 1 % this.totalPlayer];
                    CommunicationAPI.sendMessageToClient("updateCurrentPlayer", currentPlayerIndex);
                }

                //if the turn is Speeding up, the next player has another action 
                if (this.currentTurn.getType() == TurnType.SpeedingUp)
                {
                    this.currentPlayer.setGetsAnotherAction(true);
                }
            }
            // if it is the last turn of the round 
            else
            {
                //prepare for Stealing phase 
                foreach (Player p in this.players)
                {
                    p.moveCardsToDiscard();
                    //NEED MESSAGE HERE
                    p.setWaitingForInput(true);
                    this.aGameStatus = GameStatus.Stealin;
                    CommunicationAPI.sendMessageToClient("updateGameStatus", false);
                }
            }
        }
    }
    
    private void endOfCards()
    {
        Card c = this.currentRound.topOfPlayedCards();
        this.currentPlayer.addToDiscardPile(c);
        CommunicationAPI.sendMessageToClient("removeTopCardVaddCards", c);

        //if all cards in the pile have been played 
        if (this.currentRound.getPlayedCards().Count == 0)
        {

            //if this is the last round 
            if (this.currentRound.getIsLastRound())
            {
                calculateGameScore();
            }
            else
            {
                //setting the next round, setting the first turn of the round 
                this.currentRound = this.rounds[this.rounds.IndexOf(this.currentRound) + 1];
                this.currentTurn = this.currentRound.getTurns()[0];
                CommunicationAPI.sendMessageToClient("updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));

                //setting the next player and game status of the game 
                this.currentPlayer = this.players[this.rounds.IndexOf(currentRound)];
                this.currentPlayerIndex = this.players.IndexOf(currentPlayer);
                CommunicationAPI.sendMessageToClient("updateCurrentPlayer", this.currentPlayerIndex);

                this.currentPlayer.setWaitingForInput(true);
                CommunicationAPI.sendMessageToClient("updateWaitingForInput", this.currentPlayerIndex, true);
                
                this.aGameStatus = GameStatus.Schemin;
                CommunicationAPI.sendMessageToClient("updateGameStatus", true);

                //for each player, getting 6 cards from their Pile at randomn and adding them to their hand 
                foreach (Player p in this.players)
                {
                    List<Card> cardsToAdd = new List<Card>();
                    int index = this.players.IndexOf(p);

                    Random rnd = new Random();
                    for (int i = 0; i < 6; i++)
                    {
                        int rand = rnd.Next(0, p.discardPile.Count);
                        Card aCard = p.discardPile[rand];
                        p.hand.Add(aCard);
                        cardsToAdd.Add(aCard);
                        p.discardPile.Remove(aCard);
                    }
                    //NEED TO SEE WITH CRISTINA
                    CommunicationAPI.sendMessageToClient("updatePlayerHand", currentPlayerIndex, cardsToAdd);
                }
            }
        }
    }

    /*
    *   HECTOR: change this function to void because only need to send results to clients. 
    */
    private void calculateGameScore() {
        
        Dictionary <Player, int> scores = new Dictionary <Player, int>();
        int max = -1;
        Player maxPlayer = null;
        
        foreach (Player pl in this.players) {
            scores.Add(pl, pl.getPossesionsValue());
            if (pl.getNumOfBulletsShot() > max) {
                max = pl.getNumOfBulletsShot();
                maxPlayer = pl;
            }
        }
        scores[maxPlayer] = scores[maxPlayer] + 1000;

        //Sorted list to send to clients
        var myList = scores.ToList();
        myList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));

        CommunicationAPI.sendMessageToClient("finalGameScore", myList);
        
        //return scores;
    }

    private void initializeGameBoard()
    {

        //initializing Locomotive
        this.myTrain.Add(new TrainCar(true));
        
        //initializing train cars
        for (int i = 0; i < players.Count; i++)
        {
            this.myTrain.Add(new TrainCar(false));
        }

        // initializing the marshall and init his position to inside locomotive
        this.aMarshal = Marshal.getInstance();
        myTrain[0].moveInsideCar(aMarshal);

    }

    private void initializeLoot()
    {
        //initializaing a Strongbox in the locomotive 
        GameItem locomotiveStongBox = new GameItem(ItemType.Strongbox, 1000);
        locomotiveStongBox.setPosition(myTrain[0].getInside());

        // depending on the number of player, 
        // we'll initialize loots for totalPlayer number of wagon
        for (int i=0; i<totalPlayer; i++){
            
            switch (i) {
                //intializing loots in first wagon
                case 0 :{
                    GameItem anItem = new GameItem(ItemType.Purse, 500);
                    anItem.setPosition(myTrain[1].getInside());
                    GameItem anItem1 = new GameItem(ItemType.Purse, 250);
                    anItem1.setPosition(myTrain[1].getInside());
                    GameItem anItem2 = new GameItem(ItemType.Purse, 250);
                    anItem2.setPosition(myTrain[1].getInside());
                    break;
                }
                //intializing loots in second wagon
                case 1 :{
                    GameItem anItem = new GameItem(ItemType.Ruby, 500);
                    anItem.setPosition(myTrain[2].getInside());
                    GameItem anItem1 = new GameItem(ItemType.Ruby, 500);
                    anItem1.setPosition(myTrain[2].getInside());
                    GameItem anItem2 = new GameItem(ItemType.Ruby, 500);
                    anItem2.setPosition(myTrain[2].getInside());
                    break;
                }
                //intializing loots in third wagon
                case 2 :{
                    GameItem anItem = new GameItem(ItemType.Ruby, 500);
                    anItem.setPosition(myTrain[3].getInside());
                    GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                    anItem1.setPosition(myTrain[3].getInside());
                    break;
                }
                //intializing loots in 4th wagon
                case 3 :{
                    GameItem anItem = new GameItem(ItemType.Ruby, 500);
                    anItem.setPosition(myTrain[4].getInside());
                    GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                    anItem1.setPosition(myTrain[4].getInside());
                    GameItem anItem2 = new GameItem(ItemType.Purse, 250);
                    anItem1.setPosition(myTrain[4].getInside());
                    GameItem anItem3 = new GameItem(ItemType.Purse, 250);
                    anItem1.setPosition(myTrain[4].getInside());
                    break;
                }
                //intializing loots in 5th wagon
                case 4 :{
                    GameItem anItem = new GameItem(ItemType.Ruby, 500);
                    anItem.setPosition(myTrain[5].getInside());
                    GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                    anItem1.setPosition(myTrain[5].getInside());
                    GameItem anItem2 = new GameItem(ItemType.Purse, 250);
                    anItem1.setPosition(myTrain[5].getInside());
                    GameItem anItem3 = new GameItem(ItemType.Purse, 250);
                    anItem1.setPosition(myTrain[5].getInside());
                    GameItem anItem4 = new GameItem(ItemType.Purse, 250);
                    anItem1.setPosition(myTrain[5].getInside());
                    break;
                }
                //intializing loots in 6th wagon
                case 5:{
                    GameItem anItem = new GameItem(ItemType.Purse, 500);
                    anItem.setPosition(myTrain[6].getInside());
                    break;
                }
            }
        }
    }

    private void intializeRounds()
    {
        for (int i=0; i<4; i++)
        {
            Round aRound = new Round(false, totalPlayer);
            this.rounds.Add(aRound);
        }
        Round aFinalRound = new Round(true, totalPlayer);
        this.rounds.Add(aFinalRound);
    }

    private List<Position> getPossibleMoves(Player p)
    {
        List<Position> possPos = new List<Position>();
        TrainCar playerCar = p.getPosition().getTrainCar();
        // Check if on a roof or not
        if (!p.getPosition().isInside())
        {
            // Add 1-3 distance forward or backwards
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    // Add adjacent positions
                    possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) - i].getRoof());
                }
                catch (System.IndexOutOfRangeException e)
                {
                    continue;
                }
            }

            for (int i = 1; i < 4; i++)
            {
                try
                {
                    // Add adjacent positions
                    possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + i].getRoof());
                }
                catch (System.IndexOutOfRangeException e)
                {
                    continue;
                }
            }
        }
        else
        {
            try
            {
                // Add adjacent positions
                possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) - 1].getInside());
            }
            catch (System.IndexOutOfRangeException e)
            {

            }

            try
            {
                // Add adjacent positions
                possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside());
            }
            catch (System.IndexOutOfRangeException e)
            {

            }

        }
        return possPos;
    }

    private List<Player> getPossibleShootTarget(Player p)
    {
        List<Player> possPlayers = new List<Player>();
        TrainCar playerCar = p.getPosition().getTrainCar();

        if (!p.getPosition().isInside())
        {
            // Look for the players in line of sight forward on roof
            for (int i = this.myTrain.IndexOf(playerCar) + 1; i < myTrain.Count; i++)
            {
                List<Player> playersOnWagon = this.myTrain[i].getInside().getPlayers();
                if (playersOnWagon.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon);
                    break;
                }
            }

            // Look for the players in line of sight backwards on roof
            for (int i = this.myTrain.IndexOf(playerCar) - 1; i >= 0; i--)
            {
                List<Player> playersOnWagon = this.myTrain[i].getInside().getPlayers();
                if (playersOnWagon.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon);
                    break;
                }
            }
        }
        else
        {
            // Look for the players in the next wagon backwards
            try
            {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) - 1].getInside().getPlayers());
            }
            catch (System.IndexOutOfRangeException e)
            {

            }

            // Loof for the players in the next wagon forward
            try
            {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside().getPlayers());
            }
            catch (System.IndexOutOfRangeException e)
            {

            }
        }

        // If there is more than one possible player, we remove Belle.
        if (possPlayers.Count > 1)
        {
            foreach (Player pl in possPlayers)
            {
                if (pl.getBandit() == Character.Belle)
                {
                    possPlayers.Remove(pl);
                }
            }
        }

        return possPlayers;

    }


}

