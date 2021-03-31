using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Sockets;

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
    private Boolean endOfGame;
    private GameStatus aGameStatus;
    private Round currentRound;
    private Turn currentTurn;
    private List<Round> rounds;
    private List<Player> players;
    private Player currentPlayer;
    private int currentPlayerIndex;
    private List<TrainCar> myTrain;
    private Marshal aMarshal;

    private GameController()
    {
        this.players = new List<Player>();
        this.myTrain = new List<TrainCar>();
        this.rounds = new List<Round>();
        totalPlayer = 1;
        this.endOfGame = false;
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
        Player tmp = new Player(aChar);
        this.players.Add(tmp);

        MyTcpListener.addPlayerWithClient(tmp);

        Console.WriteLine("A player picked a character.");
        
        //if all players are here (HARD-CODED, usually is players.Count == totalPlayers )
        if (players.Count == 1)
        {

            initializeGameBoard();

            //setting players' positions
            for (int i = 0; i < players.Count; i++)
            {
                if (i % 2 == 0)
                {
                    myTrain[myTrain.Count - 2].moveInsideCar(players[i]);
                }
                else
                {
                    myTrain[myTrain.Count - 1].moveInsideCar(players[i]);
                }
            }

            //Send all Player objects
            CommunicationAPI.sendMessageToClient(null, "updatePlayers", players);

            initializeLoot();

            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateTrain", myTrain);

            intializeRounds();

            this.aGameStatus = GameStatus.Schemin;
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", true);

            this.currentRound = rounds[0];
            
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateCurrentRound", currentRound);

            this.currentTurn = currentRound.getTurns()[0];
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));

            players[0].setWaitingForInput(true);

            this.currentPlayer = players[0];
            //TO ALL PLAYERS
            //Send the current player as index and value for waiting for input for that index/player
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), currentPlayer.getWaitingForInput());

            Console.WriteLine("Finished initialization.");

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
                //TODO NEED TO SEE WITH CRISTINA
                //TO SPECIFIC PLAYER
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(p), "updatePlayerHand", p.getBandit(), cardsToAdd);
                 
            }
        }
    }

    public Player getCurrentPlayer()
    {
        return this.currentPlayer;
    }

    public Player getPlayerByCharacter(Character aChar)
    {
        foreach (Player p in players)
        {
            if (p.getBandit().Equals(aChar))
            {
                return p;
            }
        }
        return null;
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
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "addCards", c, c1, c2);

        endOfTurn();
    }

    public void chosenPosition(Position p)
    {
        ActionCard topOfPile = this.currentRound.seeTopOfPlayedCards();

        //if the action card is a Move Marshall action
        if (topOfPile.getKind().Equals(ActionKind.Marshal))
        {
            this.aMarshal.setPosition(p);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", this.aMarshal, p);

            //check for all players at position p 
            foreach (Player aPlayer in p.getPlayers())
            {
                BulletCard b = new BulletCard();
                aPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(aPlayer);

                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aPlayer, p.getTrainCar().getRoof());

            }
        }
        //if the action card is a Move action
        if (topOfPile.getKind().Equals(ActionKind.Move))
        {
            currentPlayer.setPosition(p);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p);

            //if the marshal is at position p, bullet card in deck + sent to the roof 
            if (p.hasMarshal(aMarshal))
            {
                BulletCard b = new BulletCard();
                currentPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(currentPlayer);
                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p.getTrainCar().getRoof());
            }
        }
    }

    public void chosenPunchTarget(Player victim, GameItem loot, Position dest)
    {

        //drop the loot at victim position, sends victim to destination 
        loot.setPosition(victim.getPosition());
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "moveGameItem", loot, victim.getPosition());

        victim.setPosition(dest);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", victim, dest);


        //loot is removed from victime possessions
        victim.possessions.Remove(loot);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "decrement", loot);

        //if the marshal is at position dest, victim: bullet card in deck + sent to the roof 
        if (dest.hasMarshal(aMarshal))
        {
            BulletCard b = new BulletCard();
            victim.addToDiscardPile(b);
            dest.getTrainCar().moveRoofCar(victim);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", victim, dest.getTrainCar().getRoof());
        }
    }

    public void chosenShootTarget(Player target)
    {
        //A BulletCard is transfered from bullets of currentPlayer to target's discardPile
        BulletCard aBullet = currentPlayer.getABullet();
        target.addToDiscardPile(aBullet);
        this.currentPlayer.shootBullet();
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "decrement", this.currentPlayer.bullets);
    }

    public void chosenLoot(GameItem loot)
    {
        //the loot is transfered from the position to the currentPlayer possensions
        loot.setPosition(null);
        currentPlayer.addToPossessions(loot);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "increment", loot);
    }

    public void readyForNextMove()
    {
        ActionCard top = this.currentRound.seeTopOfPlayedCards();
        this.currentPlayer = top.belongsTo();
        CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", this.currentPlayerIndex);

        this.currentPlayer.setWaitingForInput(false);
        Boolean waiting = true;

        // foreach (Player p in this.players)
        // {
        //     if (p.getWaitingForInput())
        //         waiting = false;

        // }

        if (waiting)
        {
            // Get the top of the played cards from the schemin phase

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
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateMovePositions", moves);
                        }
                        else
                        {
                            chosenPosition(moves[0]);
                        }
                        break;
                    }
                case ActionKind.ChangeFloor:
                    {
                        if (this.currentPlayer.getPosition().isInside())
                        {
                            this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                            //TO ALL PLAYERS
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getRoof());
                        }
                        else
                        {
                            this.currentPlayer.getPosition().getTrainCar().moveInsideCar(this.currentPlayer);
                            //TO ALL PLAYERS
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getInside());

                            if (this.currentPlayer.getPosition().hasMarshal(this.aMarshal))
                            {
                                this.currentPlayer.addToDiscardPile(new BulletCard());
                                this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                                //TO ALL PLAYERS
                                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getRoof());
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
                            //TO SPECIFIC PLAYER
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePossTarget", possTargets);
                        }
                        break;
                    }
                case ActionKind.Rob:
                    {
                        
                        List<GameItem> atLocation = this.currentPlayer.getPosition().getItems();
                        this.aGameStatus = GameStatus.FinalizingCard;
                        this.currentPlayer.setWaitingForInput(true);
                        //TO SPECIFIC PLAYER
                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateLootAtLocation", atLocation);
                        break;
                    }
                case ActionKind.Marshal:
                    {
                        List<Position> possPosition = this.aMarshal.getPossiblePositions();
                        if (possPosition.Count == 1)
                        {
                            this.chosenPosition(possPosition[0]);
                        }
                        else
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO ALL PLAYERS
                            CommunicationAPI.sendMessageToClient(null, "updateMovePositions", possPosition);

                        }
                        break;
                    }
                case ActionKind.Punch:
                    {
                        
                        List<Player> atLocation = this.currentPlayer.getPosition().getPlayers();
                        this.aGameStatus = GameStatus.FinalizingCard;
                        this.currentPlayer.setWaitingForInput(true);
                        //TO SPECIFIC PLAYER
                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePossTarget", atLocation);
                        break;
                    }
            }
            this.endOfCards();
        }
    }



    /**
    *   Private helper methods
    */

    private void endOfTurn()
    {
        //if the player has another action, then the anotherAction flag is set to false
        if (this.currentPlayer.isGetsAnotherAction())
        {
            //TO SPECIFIC PLAYER
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", currentPlayerIndex, true);
            this.currentPlayer.setGetsAnotherAction(false);
        }

        else
        {
            this.currentPlayer.setWaitingForInput(false);
            //TO SPECIFIC PLAYER 
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);

            //if this is not the last turn of the round
            if (!this.currentTurn.Equals((this.currentRound.getTurns()[this.currentRound.getTurns().Count - 1])))
            {

                //determining the next player 
                //if the turn is Switching, order of players is reversed, so next player is previous in the list
                if (this.currentTurn.getType() == TurnType.Switching)
                {
                    this.currentPlayerIndex = this.currentPlayerIndex - 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer) - 1 % this.totalPlayer];
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayerIndex);
                }
                //otherwise, it is the next player in the list 
                else
                {
                    this.currentPlayerIndex = this.currentPlayerIndex + 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer) + 1 % this.totalPlayer];
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayerIndex);
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
                    // p.setWaitingForInput(true);
                    this.aGameStatus = GameStatus.Stealin;
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateGameStatus", false);
                }
            }
        }
    }

    private void endOfCards()
    {
        Card c = this.currentRound.getTopOfPlayedCards();
        this.currentPlayer.addToDiscardPile(c);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "removeTopCard", c);

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
                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));

                //setting the next player and game status of the game 
                this.currentPlayer = this.players[this.rounds.IndexOf(currentRound)];
                this.currentPlayerIndex = this.players.IndexOf(currentPlayer);
                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", this.currentPlayerIndex);

                this.aGameStatus = GameStatus.Schemin;
                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient(null, "updateGameStatus", true);

                this.currentPlayer.setWaitingForInput(true);
                //TO SPECIFIC PLAYER
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);

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
                    //TODO NEED TO SEE WITH CRISTINA
                    //TO SPECIFIC PLAYER
                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePlayerHand", currentPlayer.getBandit(), cardsToAdd);
                   
                }
            }
        }
        readyForNextMove();
    }

    private void calculateGameScore()
    {

        Dictionary<Player, int> scores = new Dictionary<Player, int>();
        int max = -1;
        Player maxPlayer = null;

        foreach (Player pl in this.players)
        {
            scores.Add(pl, pl.getPossesionsValue());
            if (pl.getNumOfBulletsShot() > max)
            {
                max = pl.getNumOfBulletsShot();
                maxPlayer = pl;
            }
        }
        scores[maxPlayer] = scores[maxPlayer] + 1000;

        //Sorted list to send to clients
        var myList = scores.ToList();
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "finalGameScore", myList);

        this.endOfGame = true;
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
        for (int i = 0; i < totalPlayer; i++)
        {

            switch (i)
            {
                //intializing loots in first wagon
                case 0:
                    {
                        GameItem anItem = new GameItem(ItemType.Purse, 500);
                        anItem.setPosition(myTrain[1].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[1].getInside());
                        GameItem anItem2 = new GameItem(ItemType.Purse, 250);
                        anItem2.setPosition(myTrain[1].getInside());
                        break;
                    }
                //intializing loots in second wagon
                case 1:
                    {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[2].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Ruby, 500);
                        anItem1.setPosition(myTrain[2].getInside());
                        GameItem anItem2 = new GameItem(ItemType.Ruby, 500);
                        anItem2.setPosition(myTrain[2].getInside());
                        break;
                    }
                //intializing loots in third wagon
                case 2:
                    {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[3].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                        anItem1.setPosition(myTrain[3].getInside());
                        break;
                    }
                //intializing loots in 4th wagon
                case 3:
                    {
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
                case 4:
                    {
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
                case 5:
                    {
                        GameItem anItem = new GameItem(ItemType.Purse, 500);
                        anItem.setPosition(myTrain[6].getInside());
                        break;
                    }
            }
        }
    }

    private void intializeRounds()
    {
        for (int i = 0; i < 4; i++)
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

    public Boolean getEndOfGame()
    {
        return this.endOfGame;
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

    public Position getPositionByIndex(int index, Boolean inside)
    {
        if (inside)
        {
            return this.myTrain[index].getInside();
        }
        else
        {
            return this.myTrain[index].getRoof();
        }
    }

    public GameItem getItemfromTypePosition(ItemType aType)
    {
        List<GameItem> al = this.currentPlayer.getPosition().getItems();
        foreach (GameItem anItem in al)
        {
            if (anItem.getType().Equals(aType))
            {
                return anItem;
            }
        }
        return null;
    }

    public GameItem getItemfromTypePossession(ItemType aType)
    {
        List<GameItem> al = this.currentPlayer.possessions;
        foreach (GameItem anItem in al)
        {
            if (anItem.getType().Equals(aType))
            {
                return anItem;
            }
        }
        return null;
    }

    public ActionCard getCardByIndex(int index)
    {
        List<Card> al = this.currentPlayer.hand;
        return (ActionCard)al[index];
    }


}

