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
using HostageSpace;
using AttackSpace;
using System.Threading;

enum GameStatus
{
    ChoosingBandits,
    Schemin,
    Stealin,
    FinalizingCard,
    Completed,
    HorseAttack
}

class GameController
{
    private static GameController myGameController;
    private readonly int totalPlayer;
    private Boolean endOfGame;
    private GameStatus aGameStatus;
    private Round currentRound;
    private Turn currentTurn;
    private List<Round> rounds;
    private List<Player> players;
    private Player currentPlayer;
    private Player playerPtrForStarting;
    private int currentPlayerIndex;
    private int firstPlayerIndex;
    private List<TrainCar> myTrain;
    private StageCoach myStageCoach;
    private Marshal aMarshal;
    private Shotgun aShotGun;
    private List<Hostage> availableHostages;
    private Boolean endHorseAttack;
    private List<AttackPosition> attPos;
    
    private int horseAttackCounter;
    private int horseAttackPlayerCounter;
    private int horseAttackPlayersRemaining;

    private int endOfRoundBonusPlayerCounter;

    private bool shotGunCheckResponse = false;
    Player previousCurrentPlayerFromShotgun;

    private GameController(int numPlayers)
    {
        this.players = new List<Player>();
        this.myTrain = new List<TrainCar>();
        this.rounds = new List<Round>();
        this.availableHostages = new List<Hostage>();
        this.endOfGame = false;
        this.endHorseAttack = false;
        this.horseAttackCounter = 0;
        this.totalPlayer = numPlayers;
    }

    public static void setNumPlayers(int numPlayers){
        myGameController = new GameController(numPlayers);
    }

    public static GameController getInstance()
    {
        if (myGameController == null) {
            throw new InvalidOperationException("You need to set the number of players first.");
        }
        return myGameController;
    }

    /**
        * Public utility methods
    */


    public void chosenCharacter(Character aChar)
    {
        //adding a new player to the list of players 
        if (getPlayerByCharacter(aChar) != null)
        {
            MyTcpListener.informClient(true);
            Console.WriteLine("A player picked an already chosen character.");
        }
        else
        {
            Player tmp = new Player(aChar);
            this.players.Add(tmp);

            MyTcpListener.addPlayerWithClient(tmp);
            MyTcpListener.informClient(false);
            Console.WriteLine("A player picked a character.");
        }

        if (players.Count == totalPlayer)
        {
            MyTcpListener.allPlayersInitialized = true;
        }
    }
    public void allCharactersChosen()
    {
        //if all players are here (HARD-CODED, usually is players.Count == totalPlayers )
        if (players.Count == totalPlayer)
        {  
            initializeGameBoard();

            //Send all Player objects
            CommunicationAPI.sendMessageToClient(null, "updatePlayers", players);

            initializeLoot();

            //initialize the hostages 
            availableHostages = Hostage.getSomeHostages(totalPlayer);
            //Send message to all cient about the available hostages for this game
            CommunicationAPI.sendMessageToClient(null, "availableHostages", availableHostages);

            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateTrain", myTrain);

            intializeRounds();

            this.aGameStatus = GameStatus.HorseAttack;
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", this.aGameStatus);

            this.currentRound = rounds[0];
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateCurrentRound", currentRound, this.rounds.IndexOf(this.currentRound) + 1);

            this.currentTurn = currentRound.getTurns()[0];
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));

            players[0].setWaitingForInput(true);

            this.currentPlayer = players[0];

            //TO ALL PLAYERS
            //Send the current player to all clients 
            CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", this.currentPlayer.getBandit());
            
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

                if (p.getBandit().Equals(Character.Doc))
                {
                    int rand = rnd.Next(0, p.discardPile.Count);
                    Card aCard = p.discardPile[rand];
                    p.hand.Add(aCard);
                    cardsToAdd.Add(aCard);
                    p.discardPile.Remove(aCard);
                }

                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(p), "updatePlayerHand", p.getBandit(), cardsToAdd);
            }

            // Initialize a List with an AttackPosition for each Player
            this.attPos = new List<AttackPosition>();

            for (int i = 0; i < this.totalPlayer; i++)
            {
                attPos.Add(new AttackPosition(this.players[i].getBandit(), this.totalPlayer));
            }

            CommunicationAPI.sendMessageToClient(null, "updateHorses", attPos, players);
            this.horseAttackPlayersRemaining = players.Count;

            //intializing the first player 
            this.currentPlayer = players[0];
            this.currentPlayerIndex = 0;
            this.firstPlayerIndex = 0;

            CommunicationAPI.sendMessageToClient(null, "updateFirstPlayer", this.currentPlayer.getBandit());

            currentPlayer.setWaitingForInput(true);
            //TO ALL PLAYERS
            //Send the current player as index and value for waiting for input for that index/player
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), currentPlayer.getWaitingForInput());

            Console.WriteLine("Finished initialization.");

            CommunicationAPI.sendMessageToClient(null, "gameInitialized");
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

    public void playActionCard(ActionCard c, bool ghostChoseToHide, bool photographerHideDisabled)
    {
        if (c == null)
        {
            endOfTurn();
            return;
        }
        //adding the action card to the playedCard pile and removind it from player's hand
        if (c.isCanBePlayed())
        {
            this.currentRound.addToPlayedCards(c);
            if (this.currentPlayer.getBandit() == Character.Ghost && this.currentPlayer.getHasSpecialAbility())
            {
                CommunicationAPI.sendMessageToClient(null, "updateTopCard", c.belongsTo().getBandit(), c.getKind(), ghostChoseToHide, false);
            } 
            else if (this.currentPlayer.getHostage() != null)
            {
                if (this.currentPlayer.getHostage().getHostageChar() == HostageChar.Photographer)
                {
                    CommunicationAPI.sendMessageToClient(null, "updateTopCard", c.belongsTo().getBandit(), c.getKind(), false, true);
                }
                else
                {
                    CommunicationAPI.sendMessageToClient(null, "updateTopCard", c.belongsTo().getBandit(), c.getKind(), false, false);
                }
            }
            else
            {
                CommunicationAPI.sendMessageToClient(null, "updateTopCard", c.belongsTo().getBandit(), c.getKind(), false, false);
            }
            this.currentPlayer.hand.Remove(c);
        }
        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);

        endOfTurn();
    }

    public void useWhiskey(WhiskeyKind? aKind)
    {
        if (aKind == null)
        {
            endOfTurn();
            return;
        }

        //If the kind is normal/old, such a whiskey of this kind must already exist in the player's possessions and is half
        if (aKind.Equals(WhiskeyKind.Normal)){

            CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", this.currentPlayer.getBandit(), aKind);

            Whiskey aW = currentPlayer.getAWhiskey(aKind.Value);
            aW.drinkASip();
            //TODO see with Christina
            CommunicationAPI.sendMessageToClient(null, "drinkWhiskey", currentPlayer, aKind);
            if (aW.isEmpty())
            {
                currentPlayer.removeWhiskey(aW);
                CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", currentPlayer.getBandit(), aKind);
            }

            currentPlayer.setGetsAnotherAction(true);
            drawCards();
            //Falls into endOfTurn(), where hasAnotherAction is sent as true + both

        }
        else if (aKind.Equals(WhiskeyKind.Old))
        {
            CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", this.currentPlayer.getBandit(), aKind);

            Whiskey aW = currentPlayer.getAWhiskey(aKind.Value);
            aW.drinkASip();
            CommunicationAPI.sendMessageToClient(null, "drinkWhiskey", currentPlayer, aKind);
            if (aW.isEmpty())
            {
                currentPlayer.removeWhiskey(aW);
                CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", currentPlayer.getBandit(), aKind);
            }

            currentPlayer.setGetsAnotherAction(true);
            //TO SPECIFIC PLAYER
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "play");
            //CardMessage, which then goes playActionCard() and then endOfTurn() (hasAnotherAction is sent as true + both)

        }
        //If the kind is unknown, the player has at least one full whiskey
        else
        {
            CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", this.currentPlayer.getBandit(), WhiskeyKind.Unknown);

            //Retrieve the first full whiskey the player has and do the appropriate action depending on its kind
            Whiskey aW = currentPlayer.getAWhiskey();
            aW.drinkASip();

            //Increment the whiskey
            CommunicationAPI.sendMessageToClient(null, "incrementWhiskey", this.currentPlayer.getBandit(), aW.getWhiskeyKind());

            if (aW.getWhiskeyKind().Equals(WhiskeyKind.Normal))
            {

                currentPlayer.setGetsAnotherAction(true);
                drawCards();
            }
            else if (aW.getWhiskeyKind().Equals(WhiskeyKind.Old))
            {
                currentPlayer.setGetsAnotherAction(true);
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "play");

            }
        }
    }

    public void drawCards()
    {
        Random rnd = new Random();

        //taking three random cards from player's discardPile and adding them to the player's hand while the discardpile has cards
        List<Card> cardsToAdd = new List<Card>();

        try
        {
            int rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
            Card c = this.currentPlayer.discardPile[rand];
            this.currentPlayer.moveFromDiscardToHand(c);
            cardsToAdd.Add(c);

            rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
            c = this.currentPlayer.discardPile[rand];
            this.currentPlayer.moveFromDiscardToHand(c);
            cardsToAdd.Add(c);

            rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
            c = this.currentPlayer.discardPile[rand];
            this.currentPlayer.moveFromDiscardToHand(c);
            cardsToAdd.Add(c);
        } catch (Exception e) when(e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
        { 
            //Out of cards to draw
        }

        //TO SPECIFIC PLAYER 
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "addCards", cardsToAdd);

        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);


        endOfTurn();
    }

    public Boolean getEndHorseAttack()
    {
        return this.endHorseAttack;
    }

    /*public void chosenStartinPostion(Position p)
    {
        playerPtrForStarting.setPosition(p);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", playerPtrForStarting, p, getIndexByTrainCar(p.getTrainCar()));
        p.getTrainCar().setHasAHorse(true);
        playerPtrForStarting.setWaitingForInput(false);

    }*/

    public void chosenHostage(HostageChar aHostage)
    {
        //TODO send messages 
        Hostage retrievedHostage = availableHostages.Find(x => x.getHostageChar() == aHostage);
        availableHostages.Remove(retrievedHostage);
        currentPlayer.setCapturedHostage(retrievedHostage);

        if (retrievedHostage.getHostageChar().Equals(HostageChar.Teacher))
        {
            currentPlayer.actionCantBePlayed(ActionKind.Punch);
            //TODO new message 
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "actionCantBePlayed", ActionKind.Punch);
        }

        if (retrievedHostage.getHostageChar().Equals(HostageChar.Zealot))
        {
            currentPlayer.actionCantBePlayed(ActionKind.Ride);
            //TODO new message 
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "actionCantBePlayed", ActionKind.Ride);
        }

        if (retrievedHostage.getHostageChar().Equals(HostageChar.PokerPlayer))
        {
            currentPlayer.setHasSpecialAbility(false);
            //TODO new message 
            CommunicationAPI.sendMessageToClient(null, "specialAbilityDisabled", this.currentPlayer.getBandit());
        }

        if (retrievedHostage.getHostageChar().Equals(HostageChar.Photographer))
        {
            currentPlayer.setHasSpecialAbility(false);
            //TODO new message 
            CommunicationAPI.sendMessageToClient(null, "hideDisabled", this.currentPlayer.getBandit());
        }

        currentPlayer.setWaitingForInput(false);

        CommunicationAPI.sendMessageToClient(null, "updateHostageName", currentPlayer.getBandit(), retrievedHostage.getHostageChar());
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();

    }

    public void chosenPosition(bool canEscape)
    {
        //Card removed]
        if (canEscape)
        {
            this.currentPlayer.setHasEscaped();
            ActionCard topOfPile = this.currentRound.getTopOfPlayedCards();
            CommunicationAPI.sendMessageToClient(null, "roundEventEscape", "Escape", this.currentPlayer.getBandit(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()));
            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
            this.endOfCards();
        }

    }

    public void chosenPosition(Position p)
    {
        if (shotGunCheckResponse == true)
        {
            //Must have been a shotgun resolution 
            currentPlayer.setPosition(p);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p, getIndexByTrainCar(p.getTrainCar()));
            shotGunCheckResponse = false;

            currentPlayer.setWaitingForInput(false);
            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
            this.endOfCards();
            throw new InvalidOperationException();
        }

        //Card removed
        ActionCard topOfPile = this.currentRound.getTopOfPlayedCards();

        //if the action card is a Move Marshall action
        if (topOfPile.getKind().Equals(ActionKind.Marshal))
        {
            this.aMarshal.setPosition(p);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", this.aMarshal, p, getIndexByTrainCar(p.getTrainCar()));
            //check for all players at position p 
            foreach (Player aPlayer in p.getPlayers())
            {
                BulletCard b = new BulletCard(null, -1);

                if (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.MortalBullet)
                {
                    aPlayer.addMortalBullet();
                }

                aPlayer.addToDiscardPile(b);

                if(this.currentRound.getIsLastRound()) aPlayer.incrementBulletShootInLastRound();

                p.getTrainCar().moveRoofCar(aPlayer);
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aPlayer, p.getTrainCar().getRoof(), getIndexByTrainCar(p.getTrainCar()));
            }

            currentPlayer.setWaitingForInput(false);
            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
            this.endOfCards();
        }
        //if the action card is a Move action
        else if (topOfPile.getKind().Equals(ActionKind.Move))
        {
            currentPlayer.setPosition(p);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p, getIndexByTrainCar(p.getTrainCar()));
            bool flag = true;

            //if the marshal is at position p, bullet card in deck + sent to the roof 
            if (p.hasMarshal(aMarshal))
            {
                BulletCard b = new BulletCard(null, -1);

                if (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.MortalBullet)
                {
                    this.currentPlayer.addMortalBullet();
                }

                currentPlayer.addToDiscardPile(b);
                if(this.currentRound.getIsLastRound()) this.currentPlayer.incrementBulletShootInLastRound();

                p.getTrainCar().moveRoofCar(currentPlayer);

                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p.getTrainCar().getRoof(), getIndexByTrainCar(p.getTrainCar()));
            }

            try
            {
                //Doing the check and throwing an exception upon resolution to return here (HOPEFULLY)
                //Bad but ahhhhh.
                previousCurrentPlayerFromShotgun = this.currentPlayer;
                shotGunCheckResponse = true;

                if (shotgunCheck(p))
                {
                    this.currentPlayer = previousCurrentPlayerFromShotgun;
                    shotGunCheckResponse = false;
                    CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                    this.endOfCards();
                }

            }
            catch (Exception e) when (e is InvalidOperationException)
            {
                this.currentPlayer = previousCurrentPlayerFromShotgun;
                shotGunCheckResponse = false;
                CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                this.endOfCards();
            }

        }

        else if (topOfPile.getKind().Equals(ActionKind.Ride))
        {
            
            currentPlayer.setPosition(p);
            currentPlayer.getPosition().getTrainCar().setHasAHorse(true);
            currentPlayer.getPosition().getTrainCar().addAHorse();
            
            int index = getIndexByTrainCar(p.getTrainCar());
            //Player must have ended up in the stagecoach
            if (index == -1)
            {
                index = getIndexByTrainCar(myStageCoach.getAdjacentCar());
            }

            CommunicationAPI.sendMessageToClient(null, "updateCarHasAHorse", index, currentPlayer.getBandit());
            currentPlayer.setOnAHorse(false);

            bool flag = true;

            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p, myTrain.IndexOf(p.getTrainCar()));

            //If p is on the train
            if (p.hasMarshal(aMarshal))
            {
                BulletCard b = new BulletCard(null, -1);

                if (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.MortalBullet)
                {
                    this.currentPlayer.addMortalBullet();
                }

                currentPlayer.addToDiscardPile(b);

                if(this.currentRound.getIsLastRound()) this.currentPlayer.incrementBulletShootInLastRound();

                p.getTrainCar().moveRoofCar(currentPlayer);
                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p.getTrainCar().getRoof(), getIndexByTrainCar(p.getTrainCar()));
                CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                this.endOfCards();
            //If p is the stagecoach
            } else if (p.isInStageCoach(myStageCoach))
            {
                if (availableHostages.Count() != 0 && currentPlayer.getHostage() == null)
                {

                    CommunicationAPI.sendMessageToClient(null, "availableHostages", availableHostages);
                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateSelectHostage");
                } else
                {
                    CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                    this.endOfCards();
                }
            }
        } 

    }

    public void chosenPunchTarget(Player victim, GameItem loot, Position dest)
    {
        
        //Account for Cheyenne
        if (loot != null)
        {
            if (currentPlayer.getBandit().Equals(Character.Cheyenne) && currentPlayer.getHasSpecialAbility() && loot.getType().Equals(ItemType.Purse))
            {
                loot.setPosition(null);
                currentPlayer.addToPossessions(loot);
            }
        }
       
        //loot is removed from victime possessions
        if (loot != null)
        {
            victim.possessions.Remove(loot);
            //TO ALL PLAYERS

            if (loot is Whiskey)
            {
                if (((Whiskey)loot).getWhiskeyStatus() == WhiskeyStatus.Full)
                {
                    //Want to decrement unknown whiskey
                    Whiskey w = new Whiskey(WhiskeyKind.Unknown);
                    CommunicationAPI.sendMessageToClient(null, "decrementLoot", victim.getBandit(), w);
                } else
                {
                    CommunicationAPI.sendMessageToClient(null, "decrementLoot", victim.getBandit(), loot);
                }
            }

            loot.setPosition(victim.getPosition());
            //TO ALL PLAYERS
            if ((currentPlayer.getBandit().Equals(Character.Cheyenne) && currentPlayer.getHasSpecialAbility() && loot.getType().Equals(ItemType.Purse)) == true)
            {
                CommunicationAPI.sendMessageToClient(null, "incrementLoot", this.currentPlayer.getBandit(), loot);
            } else
            {
                if (loot is Whiskey)
                {
                    //Want to move an unknown in if Full
                    if (((Whiskey)loot).getWhiskeyStatus() == WhiskeyStatus.Full)
                    {
                        Whiskey w = new Whiskey(WhiskeyKind.Unknown);
                        CommunicationAPI.sendMessageToClient(null, "moveGameItem", w, victim.getPosition(), getIndexByTrainCar(victim.getPosition().getTrainCar()));
                    }
                    else
                    {
                        CommunicationAPI.sendMessageToClient(null, "moveGameItem", loot, victim.getPosition(), getIndexByTrainCar(victim.getPosition().getTrainCar()));
                    }
                } else
                {
                    CommunicationAPI.sendMessageToClient(null, "moveGameItem", loot, victim.getPosition(), getIndexByTrainCar(victim.getPosition().getTrainCar()));
                }
            }
        }

        victim.setPosition(dest);
        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", victim, dest, getIndexByTrainCar(dest.getTrainCar()));

        //if the marshal is at position dest, victim: bullet card in deck + sent to the roof 
        if (dest.hasMarshal(aMarshal))
        {
            BulletCard b = new BulletCard(null, -1);

            if (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.MortalBullet)
            {
                victim.addMortalBullet();
            }

            victim.addToDiscardPile(b);
            dest.getTrainCar().moveRoofCar(victim);
            if(this.currentRound.getIsLastRound()) victim.incrementBulletShootInLastRound();

            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", victim, dest.getTrainCar().getRoof(), getIndexByTrainCar(dest.getTrainCar()));
        }

        try
        {
            //Doing the check and throwing an exception upon resolution to return here (HOPEFULLY)
            //Bad but ahhhhh.
            previousCurrentPlayerFromShotgun = this.currentPlayer;
            this.currentPlayer = victim;
            shotGunCheckResponse = true;

            if (shotgunCheck(dest))
            {
                this.currentPlayer = previousCurrentPlayerFromShotgun;
                shotGunCheckResponse = false;
            }
            
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            this.currentPlayer = previousCurrentPlayerFromShotgun;
            shotGunCheckResponse = false;
        }

        currentPlayer.setWaitingForInput(false);
        this.currentRound.getTopOfPlayedCards();
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();
    }

    public void choseToPunchShootgun()
    {
        aShotGun.setPosition(myStageCoach.getAdjacentCar().getRoof());

        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aShotGun, myStageCoach.getAdjacentCar().getRoof(), getIndexByTrainCar(myStageCoach.getAdjacentCar()));

        aShotGun.hasBeenPunched();

        GameItem shotgunStrongbox = new GameItem(ItemType.Strongbox, 1000);

        shotgunStrongbox.setPosition(myStageCoach.getRoof());

        CommunicationAPI.sendMessageToClient(null, "moveGameItem", shotgunStrongbox, myStageCoach.getRoof(), getIndexByTrainCar(myStageCoach));

        currentPlayer.setWaitingForInput(false);

        this.currentRound.getTopOfPlayedCards();
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();
    }

    public void chosenShootTarget(Player target)
    {
        this.currentRound.getTopOfPlayedCards();
        //A BulletCard is transfered from bullets of currentPlayer to target's discardPile
        BulletCard aBullet = currentPlayer.getABullet();

        if (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.MortalBullet)
        {
            target.addMortalBullet();
        }

        target.addToDiscardPile(aBullet);
        if(this.currentRound.getIsLastRound()) target.incrementBulletShootInLastRound();

        this.currentPlayer.shootBullet();

        //if the bandit is Django
        if (currentPlayer.getBandit().Equals(Character.Django) && currentPlayer.getHasSpecialAbility())
        {

            TrainCar targetCar = target.getPosition().getTrainCar();
            TrainCar playerCar = currentPlayer.getPosition().getTrainCar();

            //if the target is in front of django, and is not in the locomotive, move target one car to the front.
            if (myTrain.IndexOf(targetCar) < myTrain.IndexOf(playerCar) && targetCar.Equals(myTrain[0]) == false)
            {

                if (target.getPosition().isInside())
                {
                    Position pos = myTrain[myTrain.IndexOf(targetCar) - 1].getInside();
                    target.setPosition(pos);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", target, pos, myTrain.IndexOf(pos.getTrainCar()));
                }
                else
                {
                    Position pos = myTrain[myTrain.IndexOf(targetCar) - 1].getRoof();
                    target.setPosition(pos);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", target, pos, myTrain.IndexOf(pos.getTrainCar()));
                }

            }
            //if the target is at the back of django and is not in the cabosse, moved one car at the back.
            else if (myTrain.IndexOf(targetCar) > myTrain.IndexOf(playerCar) && targetCar.Equals(myTrain[myTrain.Count - 1]) == false)
            {

                if (target.getPosition().isInside())
                {
                    Position pos = myTrain[myTrain.IndexOf(targetCar) + 1].getInside();
                    target.setPosition(pos);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", target, pos, myTrain.IndexOf(pos.getTrainCar()));
                }
                else
                {
                    Position pos = myTrain[myTrain.IndexOf(targetCar) + 1].getRoof();
                    target.setPosition(pos);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", target, pos, myTrain.IndexOf(pos.getTrainCar()));
                }
            }

            //If Django's target's new position ended up inside with the marshal, shoot'em
            if (target.getPosition().isInside())
            {
                if (target.getPosition().getTrainCar().getInside().hasMarshal(aMarshal))
                {
                    target.addToDiscardPile(new BulletCard(null, -1));

                    if (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.MortalBullet)
                    {
                        target.addMortalBullet();
                    }

                    target.getPosition().getTrainCar().moveRoofCar(target);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", target, target.getPosition().getTrainCar().getRoof(), getIndexByTrainCar(target.getPosition().getTrainCar()));
                }
            }
        }

        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "decrementBullets", this.currentPlayer.getBandit(), this.currentPlayer.getNumOfBulletsShot());
        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();
    }

    public void chosenLoot(GameItem loot)
    {
        this.currentRound.getTopOfPlayedCards();
        //the loot is transfered from the position to the currentPlayer possensions
        //TODO check with Christina, because we remove the loot from the deck
        loot.setPosition(null);

        if (loot is Whiskey)
        {
            currentPlayer.addToPossessions(loot);
            if (((Whiskey)loot).getWhiskeyStatus() == WhiskeyStatus.Full)
            {

                CommunicationAPI.sendMessageToClient(null, "incrementWhiskey", currentPlayer.getBandit(), WhiskeyKind.Unknown);
            } else
            {
                CommunicationAPI.sendMessageToClient(null, "incrementWhiskey", currentPlayer.getBandit(), ((Whiskey)loot).getWhiskeyKind());
            }
        }
        else {
            currentPlayer.addToPossessions(loot);
            CommunicationAPI.sendMessageToClient(null, "incrementLoot", currentPlayer.getBandit(), loot);
        }
        
        
        CommunicationAPI.sendMessageToClient(null, "updateLootAtLocation", this.currentPlayer.getPosition(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()), this.currentPlayer.getPosition().getItems(), true);
        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();
    }

    private bool shotgunCheck(Position p)
    {
        //Current player must be the target for resolution
        //if the shotgun is at position p, bullet card in deck + sent to adjacent car (if 2 adjacent, send current player request)
        if (p.hasShotgun(aShotGun))
        {
            BulletCard b = new BulletCard(null, -1);
            currentPlayer.addToDiscardPile(b);

            if (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.MortalBullet)
            {
                currentPlayer.addMortalBullet();
            }

            //if the shotgun is on the caboose, player sent to adjacent car
            if (p.getTrainCar().Equals(myTrain[myTrain.Count() - 1]))
            {
                Position pos = myTrain[myTrain.Count() - 2].getRoof();
                currentPlayer.setPosition(pos);
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, pos, myTrain.IndexOf(pos.getTrainCar()));
                return true;
            } 
            //If the shotgun is on the locomotive
            else if (p.getTrainCar().Equals(myTrain[0]))
            {
                Position pos = myTrain[1].getRoof();
                currentPlayer.setPosition(pos);
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, pos, myTrain.IndexOf(pos.getTrainCar()));
                return true;
            }
            //if not, the player must choose between the 2 positions 
            else
            {
                List<Position> aL = new List<Position>();
                TrainCar currentTraincar = p.getTrainCar();

                try
                {

                    Position p1 = myTrain[myTrain.IndexOf(currentTraincar) - 1].getRoof();
                    aL.Add(p1);
                } catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                {

                }

                try
                {
                    Position p2 = myTrain[myTrain.IndexOf(currentTraincar) + 1].getRoof();
                    aL.Add(p2);
                }
                catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                {

                }


                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateMovePositions", aL);
                currentPlayer.setWaitingForInput(true);
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);
                
                return false;
            }
        }
        return true;
    }


    public void readyForNextMove()
    {
        //See the top card in the queue and return it to its player's discard pile
        //Not removed yet
        ActionCard top = this.currentRound.seeTopOfPlayedCards();
        top.belongsTo().addToDiscardPile(top);

        //Figure out who is the current player
        this.currentPlayer = top.belongsTo();

       

        CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", this.currentPlayer.getBandit());

        Console.WriteLine("Resolving " + top.getKind() + " from " + top.belongsTo().getBandit());

        this.currentPlayer.setWaitingForInput(false);
        Boolean waiting = true;

        // foreach (Player p in this.players)
        // {
        //     if (p.getWaitingForInput())
        //         waiting = false;

        // }
        if (this.currentPlayer.getHasEscaped())
        {
            this.currentRound.getTopOfPlayedCards();
            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
            this.endOfCards();
            return;
        }
        if (waiting)
        {

            // Get the top of the played cards from the schemin phase
            switch (top.getKind())
            {
                case ActionKind.Move:
                    {
                        List<Position> moves = this.getPossibleMoves(this.currentPlayer);
                        List<int> indices = new List<int>();
                        
                        //Remove the stagecoach interior from the list of moves - you cannot use a Move card to go there
                        //You can move to the roof, though
                        if (moves.Contains(myStageCoach.getInside()))
                        {
                            moves.Remove(myStageCoach.getInside());
                        }

                        moves.ForEach(m => indices.Add(getIndexByTrainCar(m.getTrainCar())));

                        if (moves.Count > 1)
                        {

                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYERS
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateMovePositions", moves, indices);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);
                        }
                        else if (moves.Count == 0)
                        {
                            chosenPosition(moves[0]);
                        } else
                        {
                            this.currentRound.getTopOfPlayedCards();
                            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                            this.endOfCards();
                        }
                        break;
                    }
                case ActionKind.ChangeFloor:
                    {
                        //If the player is inside a car, goes on the roof
                        if (this.currentPlayer.getPosition().isInside())
                        {
                            bool flag = true;
                            Position pos = this.currentPlayer.getPosition().getTrainCar().getRoof();
                            this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                            //TO ALL PLAYERS
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getRoof(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()));

                            try
                            {
                                //Doing the check and throwing an exception upon resolution to return here (HOPEFULLY)
                                //Bad but ahhhhh.
                                previousCurrentPlayerFromShotgun = this.currentPlayer;
                                shotGunCheckResponse = true;

                                if (shotgunCheck(pos))
                                {
                                    this.currentPlayer = previousCurrentPlayerFromShotgun;
                                    shotGunCheckResponse = false;

                                    this.currentRound.getTopOfPlayedCards();
                                    CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                                    this.endOfCards();
                                }

                            }
                            catch (Exception e) when (e is InvalidOperationException)
                            {
                                this.currentPlayer = previousCurrentPlayerFromShotgun;
                                shotGunCheckResponse = false;

                                this.currentRound.getTopOfPlayedCards();
                                CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                                this.endOfCards();
                            }


                        }
                        //If the player is on the roof of a car, goes inside the car
                        else
                        {
                            this.currentPlayer.getPosition().getTrainCar().moveInsideCar(this.currentPlayer);
                            //TO ALL PLAYERS
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getInside(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()));

                            //if the marshal is at position p, bullet card in deck + sent to the roof 
                            if (currentPlayer.getPosition().getTrainCar().getInside().hasMarshal(aMarshal))
                            {
                                this.currentPlayer.addToDiscardPile(new BulletCard(null, -1));

                                if (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.MortalBullet)
                                {
                                    this.currentPlayer.addMortalBullet();
                                }

                                this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);

                                if(this.currentRound.getIsLastRound()) this.currentPlayer.incrementBulletShootInLastRound();

                                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getRoof(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()));

                                this.currentRound.getTopOfPlayedCards();
                                CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                                this.endOfCards();
                            } else if (this.currentPlayer.getPosition().isInStageCoach(myStageCoach))
                            {
                                if (availableHostages.Count() != 0 && this.currentPlayer.getHostage() == null)
                                {
                                    this.aGameStatus = GameStatus.FinalizingCard;
                                    //CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);
                                    this.currentPlayer.setWaitingForInput(true);
                                    //TODO new massage
                                    this.currentRound.getTopOfPlayedCards();
                                    CommunicationAPI.sendMessageToClient(null, "availableHostages", availableHostages);
                                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateSelectHostage");
                                } else
                                {
                                    this.currentRound.getTopOfPlayedCards();
                                    CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                                    this.endOfCards();
                                }
                            }
                            else
                            {
                                this.currentRound.getTopOfPlayedCards();
                                CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                                this.endOfCards();
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
                        else if (possTargets.Count > 1)
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYER
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePossTarget", possTargets);
                        } else
                        {
                            this.currentRound.getTopOfPlayedCards();
                            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                            this.endOfCards();
                        }
                        break;
                    }
                case ActionKind.Rob:
                    {
                        List<GameItem> atLocation = this.currentPlayer.getPosition().getItems();

                        if (atLocation.Count == 0)
                        {
                            //Empty
                            this.currentRound.getTopOfPlayedCards();
                            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                            this.endOfCards();
                        } else 
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYER

                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateLootAtLocation", this.currentPlayer.getPosition(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()), atLocation, false);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);
                        }
                       
                        break;
                    }
                case ActionKind.Marshal:
                    {
                        
                        List<Position> possPosition = this.getPossibleMoves(this.aMarshal);
                        List<int> indices = new List<int>();
                        
                        possPosition.ForEach(m => indices.Add(getIndexByTrainCar(m.getTrainCar())));

                        if (possPosition.Count > 1)
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYERS
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateMovePositions", possPosition, indices);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);
                        }
                        else
                        {
                            this.chosenPosition(possPosition[0]);
                        }
                        break;
                    }
                case ActionKind.Punch:
                    {
                        bool shotgunIsATarget = (this.currentPlayer.getPosition().isInStageCoach(this.myStageCoach) && this.currentPlayer.getPosition().isInside() == false && this.aShotGun.getIsOnStageCoach() == true);
                        List<Player> atLocation = this.currentPlayer.getPosition().getPlayers();

                        //Remove the player who requested the action themself, lol
                        atLocation.Remove(this.currentPlayer);
                        
                        if (atLocation.Count == 0 && shotgunIsATarget == false)
                        {
                            //Empty
                            this.currentRound.getTopOfPlayedCards();
                            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                            this.endOfCards();

                        } else 
                        {
                            //Removing belle from the list of player if there are other players. 
                            bool flagRemoveBelle = false;
                            
                            if (atLocation.Count > 1)
                            {
                                foreach (Player player in atLocation)
                                {
                                    if (player.getBandit().Equals(Character.Belle) && player.getHasSpecialAbility())
                                    {
                                        flagRemoveBelle = true;
                                    }
                                }
                            }

                            if (flagRemoveBelle)
                            {
                                atLocation.Remove(getPlayerByCharacter(Character.Belle));
                            }

                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYER
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePossTargetPunch", atLocation, shotgunIsATarget);
                        }
                       
                        break;
                    }
                case ActionKind.Ride:
                    {
                        if (currentPlayer.getPosition().getTrainCar().hasHorseAtCarLevel())
                        {

                            //set current player on a horse 
                            currentPlayer.setOnAHorse(true);

                            //geting all possible move when player is on a horse
                            List<Position> moves = getPossibleMoves(this.currentPlayer);
                            List<int> indices = new List<int>();

                            moves.ForEach(m => indices.Add(getIndexByTrainCar(m.getTrainCar())));


                            this.aGameStatus = GameStatus.FinalizingCard;

                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateRidePositions", moves, indices, this.currentPlayer.getBandit(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()), (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.Escape));

                            this.currentPlayer.setWaitingForInput(true);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);


                            //setting the on a horse action to false.
                            currentPlayer.getPosition().getTrainCar().setHasAHorse(false);
                            currentPlayer.getPosition().getTrainCar().removeAHorse();
                        } else
                        {
                            // Empty
                            this.currentRound.getTopOfPlayedCards();
                            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                            this.endOfCards();
                        }
                        break;
                    }
            }
        }
    }

    public void chosenHorseAttackAction(string haAction)
    {
        //For tracking if all players in one round of the attack have gone
        horseAttackPlayerCounter++;

        // Update Horse Attack position object for current player
        AttackPosition hap = this.getHAFromCharacter(this.currentPlayer.getBandit());
        if (haAction.Equals("ride"))
        {
            // Increment position of horse and update all players
            if (!hap.incrementPosition()) //If at end, set to false (then is true and increment)
            {
                this.horseAttackCounter++;
            } 
            //this.horseAttackCounter++;
        }
        else if (haAction.Equals("enter"))
        {
            // Set Off horse for current Player and update all the players
            hap.getOffHorse();
            this.horseAttackCounter++;
        }

       
        // Check if all players have chosen a position where to stop
        if (this.horseAttackPlayerCounter == this.totalPlayer)
        {
            this.endHorseAttack = true;


            // Set all the players' positions in the train
            foreach (Player p in this.players)
            {
                AttackPosition ap = this.getHAFromCharacter(p.getBandit());

                //Calculate correct train car - horse positions are reversed?
                int pos = (ap.getPosition() == 0) ? attPos.Count : attPos.Count - ap.getPosition();

                p.setPosition(this.myTrain[pos].getInside());
                p.setOnAHorse(false);

                this.myTrain[pos].setHasAHorse(true);
                this.myTrain[pos].addAHorse();
            }

            // Update the train for all the players
            CommunicationAPI.sendMessageToClient(null, "updateHorseAttack", this.attPos);

            aGameStatus = GameStatus.Schemin;
            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", aGameStatus);

            // Set currentPlayer back to 0 and start the game
            // Set current player as next player
            this.currentPlayer = players[0];

            CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());

            currentPlayer.setWaitingForInput(true);
            
            //Send the current player as index and value for waiting for input for that index/player
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), currentPlayer.getWaitingForInput());


        }
        else 
        {
            
            if (horseAttackPlayerCounter == horseAttackPlayersRemaining)
            {
                // Update all the players
                //Those who have entered and those who haven't
                CommunicationAPI.sendMessageToClient(null, "updateHorseAttack", this.attPos);
                horseAttackPlayerCounter = 0;

                //Count how many players are no longer on horses
                int playersThatEntered = 0;
                foreach (AttackPosition p in attPos)
                {
                    if (p.hasStopped() == true)
                    {
                        playersThatEntered++;
                    }
                }

                //Decrement remaining players on horses only if players that entered is != remaining
                if (playersThatEntered != horseAttackPlayersRemaining)
                {
                    horseAttackPlayersRemaining--;
                }
            }

            //Calculate next player; if the next player has already entered the train, calculate until the next player has not
            do
            {
                this.currentPlayer = this.players[mod((this.players.IndexOf(this.currentPlayer) + 1), this.totalPlayer)];
            } while (this.getHAFromCharacter(this.currentPlayer.getBandit()).hasStopped() == false);

            //If the next player is already at the max position, move them
            if (this.getHAFromCharacter(this.currentPlayer.getBandit()).getPosition() == myTrain.Count - 2)
            {
                this.getHAFromCharacter(this.currentPlayer.getBandit()).getOffHorse();
                CommunicationAPI.sendMessageToClient(null, "updateHorseAttack", this.attPos);

                chosenHorseAttackAction("enter");
            }
            else
            {
                CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());

                currentPlayer.setWaitingForInput(true);
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), currentPlayer.getWaitingForInput());
            }

            
        }
    }


    /**
    *   Private helper methods
    */
    private AttackPosition getHAFromCharacter(Character c)
    {
        foreach (AttackPosition ha in this.attPos)
        {
            if (ha.GetCharacter() == c)
            {
                return ha;
            }
        }
        return null;
    }

    private void endOfTurn()
    {
        //if the player has another action, then the anotherAction flag is set to false

        if (this.currentPlayer.isGetsAnotherAction())
        {
            //TO SPECIFIC PLAYER
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "both");
            this.currentPlayer.setGetsAnotherAction(false);
            
        }
        else
        {
            this.currentPlayer.setWaitingForInput(false);

           
            //determining the next player 
            //if the turn is Switching, order of players is reversed, so next player is previous in the list
            if (this.currentTurn.getType() == TurnType.Switching)
            {
                this.currentPlayerIndex = mod((this.currentPlayerIndex - 1), this.totalPlayer);
                this.currentPlayer = this.players[this.currentPlayerIndex];
            }
            //otherwise, it is the next player in the list 
            else
            {
                this.currentPlayerIndex = mod((this.currentPlayerIndex + 1), this.totalPlayer);
                this.currentPlayer = this.players[this.currentPlayerIndex];
            }

            //Verify whether the next player is the first player again; if so, go to the next turn
            if (this.currentPlayerIndex == this.firstPlayerIndex)
            {
                int nextTurnIndex = this.currentRound.getTurns().IndexOf(this.currentTurn) + 1;

                //If the next turn index is out of bounds, the current turn is the last turn
                if (nextTurnIndex == this.currentRound.getTurns().Count)
                {

                    if (this.currentRound.getEvent() == EndOfRoundEvent.PantingHorses || this.currentRound.getEvent() == EndOfRoundEvent.WhiskeyForMarshal || this.currentRound.getEvent() == EndOfRoundEvent.HigherSpeed || this.currentRound.getEvent() == EndOfRoundEvent.ShotgunRage)
                    {
                        //prepare for card save

                        CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());
                        CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);
                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);

                    } else
                    {
                        //prepare for Stealing phase 
                        foreach (Player p in this.players)
                        {
                            p.moveCardsToDiscard();
                            //Send an empty list to the client's hand
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(p), "updatePlayerHand", p.getBandit(), new List<Card>());

                        }

                        this.aGameStatus = GameStatus.Stealin;
                        //TO ALL PLAYERS
                        CommunicationAPI.sendMessageToClient(null, "updateGameStatus", this.aGameStatus);

                        Thread.Sleep(1000);

                        endOfCards();
                        return;
                    }
                    
                }
                else
                {
                    this.currentTurn = this.currentRound.getTurns()[nextTurnIndex];
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", this.currentRound.getTurns().IndexOf(this.currentTurn));

                    if (this.currentTurn.getType() == TurnType.Switching)
                    {
                        this.currentPlayerIndex = firstPlayerIndex;
                        this.currentPlayer = this.players[firstPlayerIndex];
                    }
                }
            } 
            
            if (this.currentTurn.getType() == TurnType.SpeedingUp)
            {
                this.currentPlayer.setGetsAnotherAction(true);
            }

            //TO ALL PLAYERS

            CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());
            //TO CURRENT PLAYER
            if (this.currentPlayer.isGetsAnotherAction())
            {
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "both");
            }
            else
            {
                this.currentPlayer.setWaitingForInput(true);
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);
            }

            Console.WriteLine(this.currentTurn.getType());
            Console.WriteLine(this.currentPlayerIndex);

        }
    }

    public void endOfRoundBonus(ActionCard c)
    {
        //For tracking if all players have chosen
        endOfRoundBonusPlayerCounter++;

        if (c != null)
        {
            //Remove card from the hand before moving cards to discard
            this.currentPlayer.hand.Remove(c);
            this.currentPlayer.moveCardsToDiscard();
            this.currentPlayer.hand.Add(c);
        }
        else if (c == null)
        {
            this.currentPlayer.moveCardsToDiscard();
        }

        this.currentPlayerIndex = mod((this.currentPlayerIndex + 1), this.totalPlayer);
        this.currentPlayer = this.players[this.currentPlayerIndex];

        CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());

        if (endOfRoundBonusPlayerCounter == this.totalPlayer)
        {
            endOfRoundBonusPlayerCounter = 0;
            //prepare for Stealing phase 
            foreach (Player p in this.players)
            {
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(p), "updatePlayerHand", p.getBandit(), p.hand);
            }

            this.aGameStatus = GameStatus.Stealin;
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", this.aGameStatus);

            Thread.Sleep(1000);

            endOfCards();
            return;
        } 
        else
        {
            //prepare for card save
            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);
        }
    }

    private void endOfCards()
    {
        try
        {
            Card c = this.currentRound.seeTopOfPlayedCards();
            CommunicationAPI.sendMessageToClient(null, "highlightTopCard");

            Thread.Sleep(3000);

            readyForNextMove();
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            //The queue is empty
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
                    resolveEndOfRoundEvent();

                    //setting the next round, setting the first turn of the round 
                    this.currentRound = this.rounds[this.rounds.IndexOf(this.currentRound) + 1];
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentRound", this.currentRound, this.rounds.IndexOf(this.currentRound) + 1);

                    this.currentTurn = this.currentRound.getTurns()[0];
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));

                    //setting the next First player and game status of the game
                    this.firstPlayerIndex = (this.firstPlayerIndex == totalPlayer - 1) ? 0 : (firstPlayerIndex + 1);
                    this.currentPlayer = this.players[firstPlayerIndex];
                    this.currentPlayerIndex = this.players.IndexOf(currentPlayer);
                    //TO ALL PLAYERS

                    CommunicationAPI.sendMessageToClient(null, "updateFirstPlayer", this.currentPlayer.getBandit());
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", this.currentPlayer.getBandit());

                    this.aGameStatus = GameStatus.Schemin;
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateGameStatus", this.aGameStatus);
                    checkLadyPool();

                    this.currentPlayer.setWaitingForInput(true);
                    //TO SPECIFIC PLAYER
                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);

                    //for each player, getting 6 cards from their Pile at randomn and adding them to their hand 
                    foreach (Player p in this.players)
                    {
                        List<Card> cardsToAdd = new List<Card>();

                        Random rnd = new Random();

                        int cardsToAddCount = 6;
                        if (p.hand.Count == 1)
                        {
                            cardsToAddCount = 5;
                        }

                        int ministerCount = cardsToAddCount - 1;

                        for (int i = 0; i < cardsToAddCount; i++)
                        {
                            if (p.getHostage() != null)
                            {
                                if(p.getHostage().getHostageChar().Equals(HostageChar.Minister) && i == ministerCount) break;
                            }
                            int rand = rnd.Next(0, p.discardPile.Count);
                            Card aCard = p.discardPile[rand];
                            p.hand.Add(aCard);
                            cardsToAdd.Add(aCard);
                            p.discardPile.Remove(aCard);
                        }

                        if (p.getBandit().Equals(Character.Doc) && p.getHasSpecialAbility())
                        {
                            int rand = rnd.Next(0, p.discardPile.Count);
                            Card aCard = p.discardPile[rand];
                            p.hand.Add(aCard);
                            cardsToAdd.Add(aCard);
                            p.discardPile.Remove(aCard);
                        }

                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(p), "updatePlayerHand", p.getBandit(), cardsToAdd);
                    }

                    //Stagecoach moves by one car to the back, if not at the last one of the train.
                    TrainCar curAdjacent = myStageCoach.getAdjacentCar();

                    if (curAdjacent.Equals(myTrain[myTrain.Count() - 1]) == false)
                    {
                        //Locomotive is 0, so add here
                        TrainCar newAdjacent = myTrain[myTrain.IndexOf(curAdjacent) + 1];
                        myStageCoach.setAdjacentCar(newAdjacent);
                        CommunicationAPI.sendMessageToClient(null, "moveStageCoach", newAdjacent);

                        //if Shotgun is not on the stage coach, it also moves by one car to the back
                        if (aShotGun.getIsOnStageCoach() == false)
                        {
                            aShotGun.setPosition(newAdjacent.getRoof());
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aShotGun, newAdjacent.getRoof(), myTrain.IndexOf(newAdjacent));
                        }

                    }
                }
            }
        }
    }

    private void calculateGameScore()
    {

        Dictionary<Player, int> scores = new Dictionary<Player, int>();
        int max = -1;
        Player maxPlayer = null;

        foreach (Player pl in this.players)
        {
            int totalValue = pl.getPossesionsValue() + pl.getHostageValue();

            scores.Add(pl, totalValue);
            if (pl.getNumOfBulletsShot() > max)
            {
                max = pl.getNumOfBulletsShot();
                maxPlayer = pl;
            }
        }
        scores[maxPlayer] = scores[maxPlayer] + 1000;

        EndOfRoundEvent ev = this.currentRound.getEvent();
        // Take care of Train station event
        switch (this.currentRound.getEvent()) 
        {
            case EndOfRoundEvent.MarshalsRevenge: 
                {
                    // Each bandit on the roof of the Marshal's car looses his least valuable purse
                    foreach (Player b in this.aMarshal.getPosition().getPlayers())
                    {
                        scores[b] = scores[b] - b.getLeastPurseValue();
                        if (b.getLeastPurseValue() != 0)
                        {
                            CommunicationAPI.sendMessageToClient(null, "moveGameItem", b.getLeastPurse(), this.aMarshal.getPosition().getTrainCar().getRoof(), getIndexByTrainCar(aMarshal.getPosition().getTrainCar()));
                            CommunicationAPI.sendMessageToClient(null, "decrementLoot", b.getBandit(), b.getLeastPurse());
                        }
                    }

                    break;
                }
            case EndOfRoundEvent.Pickpocketing: 
                {
                    // Each bandit that is alone takes a purse if available on his spot
                    foreach (Player b in this.players)
                    {
                        if (b.getPosition().getPlayers().Count == 1)
                        {
                            scores[b] = scores[b] + b.getPosition().getRandomPurse();

                            if (b.getPosition().getRandomPurse() != 0)
                            {
                                GameItem purse = b.getPosition().getRandomPurseItem();
                                b.getPosition().getItems().Remove(purse);
                                CommunicationAPI.sendMessageToClient(null, "updateLootAtLocation", b.getPosition(), getIndexByTrainCar(b.getPosition().getTrainCar()), b.getPosition().getItems(), true);
                                CommunicationAPI.sendMessageToClient(null, "incrementLoot", b.getBandit(), purse);
                            }
                           
                        }
                    }
                    break;
                }
            case EndOfRoundEvent.HostageConductor: {
                    // +250 to all on locomotive
                foreach (Player p in myTrain[0].getRoof().getPlayers())
                {
                    scores[p] = scores[p] + 250;
                    CommunicationAPI.sendMessageToClient(null, "incrementLoot", p.getBandit(), new GameItem(ItemType.Purse, 250));
                }
                break;
            }
            case EndOfRoundEvent.SharingTheLoot: {
                    // bandits who own a strongbox and are not alone share the value of it with neighbours
                    foreach (TrainCar t in myTrain)
                    {

                        //check roof
                        List<Player> onRoof = t.getRoof().getPlayers();
                        if (onRoof.Count >= 2)
                        {

                            foreach (Player p in onRoof)
                            {
                                if (p.hasAStrongBox())
                                {
                                    scores[p] -= 1000;
                                    foreach (Player p2 in onRoof)
                                    {
                                        scores[p2] += 1000 / onRoof.Count();
                                    }
                                }
                            }
                        }

                        //check inside 
                        List<Player> inside = t.getInside().getPlayers();
                        if (inside.Count >= 2)
                        {

                            foreach (Player p in inside)
                            {
                                if (p.hasAStrongBox())
                                {
                                    scores[p] -= 1000;
                                    foreach (Player p2 in inside)
                                    {
                                        scores[p2] += 1000 / inside.Count();
                                    }
                                }
                            }
                        }
                    }

                    //check roof of stage coach
                    List<Player> onSC = myStageCoach.getRoof().getPlayers();
                    if (onSC.Count >= 2)
                    {

                        foreach (Player p in onSC)
                        {
                            if (p.hasAStrongBox())
                            {
                                scores[p] -= 1000;
                                foreach (Player p2 in onSC)
                                {
                                    scores[p2] += 1000 / onSC.Count();
                                }
                            }
                        }
                    }

                    //check inside the stage coach
                    List<Player> inSC = myStageCoach.getInside().getPlayers();
                    if (inSC.Count >= 2)
                    {

                        foreach (Player p in inSC)
                        {
                            if (p.hasAStrongBox())
                            {
                                scores[p] -= 1000;
                                foreach (Player p2 in inSC)
                                {
                                    scores[p2] += 1000 / inSC.Count();
                                }
                            }
                        }
                    }
                    break;
            }
            case EndOfRoundEvent.Escape: {
                    // Every bandit who is in train loses
                    //for each position in the train, if there is a player, then he has lost 
                    foreach (TrainCar t in myTrain)
                    {
                        foreach (Player p1 in t.getRoof().getPlayers())
                        {
                            scores[p1] = 0;
                        }
                        foreach (Player p2 in t.getInside().getPlayers())
                        {
                            scores[p2] = 0;
                        }
                    }
                    break;
            }
            case EndOfRoundEvent.MortalBullet: {
                    // Players loose 150 per bullet received during this round

                    foreach (Player b in this.players)
                    {
                        scores[b] = scores[b] - (b.getMortalBullets()*150);
                        
                    }

                    break;
            }   
        }
        
        //Sorted list to send to clients
        var myList = scores.ToList();
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        myList.Reverse();

        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "finalGameScores", myList, maxPlayer);

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

        //initializing StageCoach
        myStageCoach = new StageCoach(false, myTrain[0]);

        // initializing the marshall and init his position to inside locomotive
        aMarshal = Marshal.getInstance();
        myTrain[0].moveInsideCar(aMarshal);

        // initializing the shotgun 
        aShotGun = Shotgun.getInstance();

        
    }

    private void initializeLoot()
    {
        //initializaing a Strongbox in the locomotive 
        GameItem locomotiveStongBox = new GameItem(ItemType.Strongbox, 1000);
        locomotiveStongBox.setPosition(myTrain[0].getInside());

        //REMARK strongbox only accessible when you punch the shotgun 
        //initializaing a Strongbox in the stage coach 
        //GameItem stageCoachStongBox = new GameItem(ItemType.Strongbox, 1000);
        //stageCoachStongBox.setPosition(myStageCoach.getInside());

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
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        aW.setPosition(myTrain[1].getInside());
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
                        Whiskey aW = new Whiskey(WhiskeyKind.Old);
                        aW.setPosition(myTrain[2].getInside());
                        break;
                    }
                //intializing loots in third wagon
                case 2:
                    {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[3].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                        anItem1.setPosition(myTrain[3].getInside());
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        aW.setPosition(myTrain[3].getInside());
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
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        aW.setPosition(myTrain[4].getInside());
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
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        aW.setPosition(myTrain[5].getInside());
                        break;
                    }
                //intializing loots in 6th wagon
                case 5:
                    {
                        GameItem anItem = new GameItem(ItemType.Purse, 500);
                        anItem.setPosition(myTrain[6].getInside());
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        aW.setPosition(myTrain[6].getInside());
                        break;
                    }
            }
        }
    }

    private void intializeRounds()
    {
        // Variables to get random round layouts
        List<int> usedRounds = new List<int>();
        Random r = new Random();
        int rand;

        // Initialize random unique layouts for 4 normal rounds
        for (int i = 0; i < 4; i++)
        {
            // Look for a random integer between 0 and 6 which has not been used before
            rand = r.Next(0, 12);
            while (true) {
                if (!usedRounds.Contains(rand)) {
                    break;
                } 
                rand = r.Next(0, 12);
            }
            usedRounds.Add(rand);
            Round aRound = new Round(false, totalPlayer);
            aRound.intializeTurn(this.totalPlayer, rand);
            this.rounds.Add(aRound);
        }

        // Initialize random layout for final round
        rand = r.Next(0, 6);
        Round aFinalRound = new Round(true, totalPlayer);
        aFinalRound.intializeTurn(this.totalPlayer, rand);
        this.rounds.Add(aFinalRound);
    }

    private List<Position> getPossibleMoves(Marshal m)
    {
        List<Position> possPos = new List<Position>();
        TrainCar marshalCar = m.getPosition().getTrainCar();

        try
        {
            // Add adjacent position
            possPos.Add(this.myTrain[this.myTrain.IndexOf(marshalCar) - 1].getInside());
        }
        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
        {
        }

        try
        {
            // Add adjacent position
            possPos.Add(this.myTrain[this.myTrain.IndexOf(marshalCar) + 1].getInside());


        }
        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
        {
        }
        

        return possPos;
    }

    private List<Position> getPossibleMoves(Player p)
    {
        List<Position> possPos = new List<Position>();
        TrainCar playerCar = p.getPosition().getTrainCar();

        //if the player is on a horse 
        if (p.isPlayerOnAHorse())
        {
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    TrainCar tmp = this.myTrain[this.myTrain.IndexOf(playerCar) - i];

                    // Add adjacent possible positions at the back of current position
                    possPos.Add(tmp.getInside());
                    //if there is the stage coach et the desired level, stagecoach is counted. 
                    if (myStageCoach.getAdjacentCar().Equals(tmp))
                    {
                        possPos.Add(myStageCoach.getInside());
                    }
                }
                catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                {
                    continue;
                }
            }
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    TrainCar tmp = this.myTrain[this.myTrain.IndexOf(playerCar) + i];
                    // Add adjacent possible positions at the front of current position
                    possPos.Add(tmp.getInside());
                    if (myStageCoach.getAdjacentCar().Equals(tmp))
                    {
                        possPos.Add(myStageCoach.getInside());
                    }
                }
                catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                {
                    continue;
                }
            }


            try
            {
                TrainCar tmp = this.myTrain[this.myTrain.IndexOf(playerCar)];
                // Add adjacent possible positions at the front of current position
                possPos.Add(tmp.getInside());
                if (myStageCoach.getAdjacentCar().Equals(tmp))
                {
                    possPos.Add(myStageCoach.getInside());
                }
            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {
                
            }

        }
        //if the player is inside or on the roof of the stagecoach
        else if (p.getPosition().isInStageCoach(myStageCoach))
        {
            if (p.getPosition().isInside())
            {
                return possPos;
            }
            else //On roof
            {

                if (p.getHostage() != null)
                {
                    if (p.getHostage().getHostageChar() == HostageChar.OldLady)
                    {
                        TrainCar adjacentCar = myStageCoach.getAdjacentCar();

                        //Add the adjacent car's roof
                        possPos.Add(adjacentCar.getRoof());

                    } 
                    else
                    {
                        TrainCar adjacentCar = myStageCoach.getAdjacentCar();

                        //Add the adjacent car's roof
                        possPos.Add(adjacentCar.getRoof());

                        // Add 1-2 distance forward or backwards
                        for (int i = 1; i < 3; i++)
                        {
                            try
                            {
                                // Add adjacent positions
                                possPos.Add(this.myTrain[this.myTrain.IndexOf(adjacentCar) - i].getRoof());
                            }
                            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                            {
                                continue;
                            }
                        }

                        for (int i = 1; i < 3; i++)
                        {
                            try
                            {
                                // Add adjacent positions
                                possPos.Add(this.myTrain[this.myTrain.IndexOf(adjacentCar) + i].getRoof());
                            }
                            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                            {
                                continue;
                            }
                        }
                    }
                } else
                {
                    TrainCar adjacentCar = myStageCoach.getAdjacentCar();

                    //Add the adjacent car's roof
                    possPos.Add(adjacentCar.getRoof());

                    // Add 1-2 distance forward or backwards
                    for (int i = 1; i < 3; i++)
                    {
                        try
                        {
                            // Add adjacent positions
                            possPos.Add(this.myTrain[this.myTrain.IndexOf(adjacentCar) - i].getRoof());
                        }
                        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                        {
                            continue;
                        }
                    }

                    for (int i = 1; i < 3; i++)
                    {
                        try
                        {
                            // Add adjacent positions
                            possPos.Add(this.myTrain[this.myTrain.IndexOf(adjacentCar) + i].getRoof());
                        }
                        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                        {
                            continue;
                        }
                    }
                }

                

            }
        }
        //if the player is on the roof of a Car
        else if (p.getPosition().isInside() == false)
        {
            //if the playerCar is adjacent to the stageCoach, add roof of the stage coach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {
                possPos.Add(myStageCoach.getRoof());
            }

            //if the current Player has Old Lady as Hostage, can only move one car at a time on the roof
            if (currentPlayer.getHostage() != null)
            {
                if (currentPlayer.getHostage().getHostageChar().Equals(HostageChar.OldLady))
                {
                    try
                    {
                        TrainCar wagon = this.myTrain[this.myTrain.IndexOf(playerCar) - 1];
                        // Add adjacent positions
                        possPos.Add(wagon.getRoof());

                    }
                    catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                    {

                    }

                    try
                    {
                        // Add adjacent positions
                        possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getRoof());
                    }
                    catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                    {

                    }
                }
                else
                {
                    // Add 1-3 distance forward or backwards
                    for (int i = 1; i < 4; i++)
                    {
                        try
                        {
                            // Add adjacent positions
                            possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) - i].getRoof());
                        }
                        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                        {
                            continue;
                        }

                        //If the first or second car is adjacent to the stagecoach, it counts as an alternate third position
                        if (i != 3)
                        {
                            //if the car is adjacent to the stageCoach, add roof of the stage coach to possPos
                            if (myStageCoach.getAdjacentCar().Equals(myTrain[myTrain.IndexOf(playerCar) - i]))
                            {
                                possPos.Add(myStageCoach.getRoof());
                            }
                        }

                    }

                    for (int i = 1; i < 4; i++)
                    {
                        try
                        {
                            // Add adjacent positions
                            possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + i].getRoof());


                        }
                        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                        {
                            continue;
                        }
                        //If the first or second car is adjacent to the stagecoach, it counts as an alternate third position
                        if (i != 3)
                        {
                            //if the car is adjacent to the stageCoach, add roof of the stage coach to possPos
                            if (myStageCoach.getAdjacentCar().Equals(myTrain[myTrain.IndexOf(playerCar) + i]))
                            {
                                possPos.Add(myStageCoach.getRoof());
                            }
                        }
                    }
                }
            }
            else
            {
                // Add 1-3 distance forward or backwards
                for (int i = 1; i < 4; i++)
                {
                    try
                    {
                        // Add adjacent positions
                        possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) - i].getRoof());
                    }
                    catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                    {
                        continue;
                    }

                    //If the first or second car is adjacent to the stagecoach, it counts as an alternate third position
                    if (i != 3)
                    {
                        //if the car is adjacent to the stageCoach, add roof of the stage coach to possPos
                        if (myStageCoach.getAdjacentCar().Equals(myTrain[myTrain.IndexOf(playerCar) - i]))
                        {
                            possPos.Add(myStageCoach.getRoof());
                        }
                    }

                }
            
                for (int i = 1; i < 4; i++)
                {
                    try
                    {
                        // Add adjacent positions
                        possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + i].getRoof());


                    }
                    catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                    {
                        continue;
                    }
                    //If the first or second car is adjacent to the stagecoach, it counts as an alternate third position
                    if (i != 3)
                    {
                        //if the car is adjacent to the stageCoach, add roof of the stage coach to possPos
                        if (myStageCoach.getAdjacentCar().Equals(myTrain[myTrain.IndexOf(playerCar) + i]))
                        {
                            possPos.Add(myStageCoach.getRoof());
                        }
                    }
                }
            }
 

        }
        //if the player is inside a Car
        else
        {
            //Add adjacent positions
            try
            {
                TrainCar wagon = this.myTrain[this.myTrain.IndexOf(playerCar) - 1];
                // Add adjacent positions
                possPos.Add(wagon.getInside());

            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }

            try
            {
                // Add adjacent positions
                possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside());
            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }

        }

        return possPos;
    }

    //add a bullet to the player's discard pile that have LadyPool as hostage 
    private void checkLadyPool()
    {
        foreach (Player p in players)
        {
            if (p.getHostage() != null)
            { 
                if (p.getHostage().getHostageChar().Equals(HostageChar.LadyPoodle))
                {
                    p.addToDiscardPile(new BulletCard(null, -1));

                    if (this.currentRound.getIsLastRound() && this.currentRound.getEvent() == EndOfRoundEvent.MortalBullet)
                    {
                        p.addMortalBullet();
                    }
                    break;
                }
            }
        }
    }

    public void getPossiblePunchMoves(Player p)
    {
        List<Position> possPos = new List<Position>();
        TrainCar playerCar = p.getPosition().getTrainCar();

        //Move the target to the floor of an adjacent car

        //If in stagecoach, add appropriate floor of adjacent car
        if (p.getPosition().isInStageCoach(myStageCoach))
        {
            if (p.getPosition().isInside())
            {
                possPos.Add(myStageCoach.getAdjacentCar().getInside());
            }
            else
            {
                possPos.Add(myStageCoach.getAdjacentCar().getRoof());
            }
        } 
        else if (p.getPosition().isInside())
        {
            //if the playerCar is adjacent to the stageCoach, add inside of the stage coach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {
                possPos.Add(myStageCoach.getInside());
            }

            try
            {
                TrainCar wagon = this.myTrain[this.myTrain.IndexOf(playerCar) - 1];
                // Add adjacent positions
                possPos.Add(wagon.getInside());

            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }

            try
            {
                // Add adjacent positions
                possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside());
            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }
        } else if (p.getPosition().isInside() == false)
        {
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {
                possPos.Add(myStageCoach.getRoof());
            }

            try
            {
                TrainCar wagon = this.myTrain[this.myTrain.IndexOf(playerCar) - 1];
                // Add adjacent positions
                possPos.Add(wagon.getRoof());

            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }

            try
            {
                // Add adjacent positions
                possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getRoof());
            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }
        }

        List<int> indices = new List<int>();

        possPos.ForEach(m => indices.Add(getIndexByTrainCar(m.getTrainCar())));

        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePunchPositions", possPos, indices);

        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);
    }

    private void resolveEndOfRoundEvent()
    {

        switch (this.currentRound.getEvent())
        {

            case EndOfRoundEvent.AngryMarshal:
                {
                    //player on top of marshal get a bullet card 
                    foreach (Player p in aMarshal.getPosition().getTrainCar().getRoof().getPlayers())
                    {

                        p.addToDiscardPile(new BulletCard(null, -1));

                    }

                    //Move marshal one traincar to the back 
                    int marshalAt = getIndexByTrainCar(aMarshal.getPosition().getTrainCar());

                    if (marshalAt != myTrain.Count() - 1)
                    {
                        marshalAt++;
                    }

                    Position pos = myTrain[marshalAt].getInside();
                    aMarshal.setPosition(pos);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aMarshal, pos, getIndexByTrainCar(pos.getTrainCar()));
                    break;
                }

            case EndOfRoundEvent.SwivelArm:
                {
                    //player on a roof are sent to the roof of the cabosse 
                    Position pos = myTrain[myTrain.Count() - 1].getRoof();
                    foreach (Player p in players)
                    {
                        if (p.getPosition().isInside() == false && p.getPosition().isInStageCoach(myStageCoach) == false)
                        {
                            p.setPosition(pos);
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", p, pos, getIndexByTrainCar(pos.getTrainCar()));
                        }
                    }
                    break;
                }

            case EndOfRoundEvent.Braking:
                {
                    //player on a roof move on car to the front  
                    foreach (Player p in players)
                    {

                        //if p is on a roof && not on the locomotive && not on the stage coach 
                        if (p.getPosition().isInside() == false && p.getPosition().getTrainCar().Equals(myTrain[0]) == false && p.getPosition().isInStageCoach(myStageCoach) == false)
                        {
                            Position pos = myTrain[myTrain.IndexOf(p.getPosition().getTrainCar()) - 1].getRoof();
                            p.setPosition(pos);
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", p, pos, getIndexByTrainCar(pos.getTrainCar()));
                        }
                    }
                    break;
                }

            case EndOfRoundEvent.TakeItAll:
                {
                    //add one strongbox to the position of the marshal 
                    GameItem aSB = new GameItem(ItemType.Strongbox, 1000);
                    Position pos = aMarshal.getPosition();
                    aSB.setPosition(pos);

                    CommunicationAPI.sendMessageToClient(null, "moveGameItem", aSB, pos, getIndexByTrainCar(pos.getTrainCar()));
                    break;
                }

            case EndOfRoundEvent.PassengersRebellion:
                {
                    //all player inside the train receive a neutral bullet 
                    foreach (Player p in players)
                    {
                        if (p.getPosition().isInside() && p.getPosition().isInStageCoach(myStageCoach) == false)
                        {
                            p.addToDiscardPile(new BulletCard(null, -1));
                        }
                    }
                    break;
                }

            case EndOfRoundEvent.HigherSpeed:
                {
                    //player on a roof move on car to the back   
                    foreach (Player p in players)
                    {
                        if (p.getPosition().isInside() == false && p.getPosition().getTrainCar().Equals(myTrain[myTrain.Count() - 1]) == false && p.getPosition().isInStageCoach(myStageCoach) == false)
                        {
                            Position pos = myTrain[myTrain.IndexOf(p.getPosition().getTrainCar()) + 1].getRoof();
                            p.setPosition(pos);
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", p, pos, getIndexByTrainCar(pos.getTrainCar()));
                        }
                    }

                    //also move horse 
                    foreach (TrainCar t in myTrain)
                    {
                        if (t.Equals(myTrain[myTrain.Count() - 1]) == false && t.hasHorseAtCarLevel())
                        {
                            t.removeAHorse();
                            t.setHasAHorse(t.hasHorseAtCarLevel());

                            int newIndexHorse = getIndexByTrainCar(t);
                            newIndexHorse++;
                            
                            myTrain[newIndexHorse].setHasAHorse(true);
                            myTrain[newIndexHorse].addAHorse();
                            //TODO HEC add new message to specify clients ?

                            
                        }
                    }

                    CommunicationAPI.sendMessageToClient(null, "roundEvent", "HigherSpeed");

                    //also move stage coach 
                    TrainCar adj = myStageCoach.getAdjacentCar();
                    int newIndex = getIndexByTrainCar(adj);

                    if (newIndex != myTrain.Count() - 1)
                    {
                        newIndex++;
                    }

                    myStageCoach.setAdjacentCar(myTrain[newIndex]);

                    CommunicationAPI.sendMessageToClient(null, "moveStageCoach");

                    //and shotgun if he is on the train
                    if (aShotGun.getIsOnStageCoach() == false)
                    {
                        int newIndexShotgun = getIndexByTrainCar(aShotGun.getPosition().getTrainCar());

                        if (newIndexShotgun != myTrain.Count() - 1)
                        {
                            newIndexShotgun++;
                        }

                        aShotGun.setPosition(myTrain[newIndexShotgun].getRoof());
                        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aShotGun, aShotGun.getPosition(), getIndexByTrainCar(aShotGun.getPosition().getTrainCar()));
                    }

                    break;
                }

            case EndOfRoundEvent.ShotgunRage:
                {
                    List<Player> inRange = new List<Player>();
                    inRange.AddRange(myStageCoach.getRoof().getPlayers());
                    inRange.AddRange(myStageCoach.getInside().getPlayers());
                    inRange.AddRange(myStageCoach.getAdjacentCar().getRoof().getPlayers());
                    inRange.AddRange(myStageCoach.getAdjacentCar().getInside().getPlayers());

                    foreach (Player p in inRange)
                    {
                        p.addToDiscardPile(new BulletCard(null, -1));
                    }

                    break;
                }

            case EndOfRoundEvent.PantingHorses:
                {
                    int horsesRemoved = 0;
                    //TODO remove horse at the back 
                    foreach (TrainCar t in myTrain)
                    {
                        if (t.hasHorseAtCarLevel())
                        {
                            if (t.getNumOfHorses() == 1)
                            {
                                t.removeAHorse();
                                horsesRemoved++;

                                t.setHasAHorse(t.hasHorseAtCarLevel());
                                if (horsesRemoved == 2)
                                {

                                    break;
                                }
                            }
                            else if (t.getNumOfHorses() >= 2)
                            {
                                t.removeAHorse();
                                horsesRemoved++;
                                
                                t.setHasAHorse(t.hasHorseAtCarLevel());

                                if (horsesRemoved == 2)
                                {
                                    break;
                                }
                                else
                                {
                                    t.removeAHorse();
                                    horsesRemoved++;
                                    t.setHasAHorse(t.hasHorseAtCarLevel());
                                    if (horsesRemoved == 2)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    CommunicationAPI.sendMessageToClient(null, "roundEvent", "PantingHorses");
                    break;
                }

            case EndOfRoundEvent.WhiskeyForMarshal:
                {
                    //remove whiskey at marshal position
                    Position pos = aMarshal.getPosition();
                    foreach (GameItem g in pos.getItems())
                    {
                        if (g is Whiskey)
                        {

                            Whiskey w = (Whiskey)g;
                            //if normal whiskey, marshal move to adjacent 
                            if (w.getWhiskeyKind().Equals(WhiskeyKind.Normal))
                            {
                                int newIndex = getIndexByTrainCar(pos.getTrainCar());

                                if (newIndex != myTrain.Count() - 1)
                                {
                                    newIndex++;
                                }


                                Position newPos = myTrain[newIndex].getInside();
                                aMarshal.setPosition(newPos);
                                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aMarshal, newPos, getIndexByTrainCar(newPos.getTrainCar()));

                                //Player check
                                foreach (Player aPlayer in newPos.getPlayers())
                                {
                                    BulletCard b = new BulletCard(null, -1);
                                    aPlayer.addToDiscardPile(b);
                                    newPos.getTrainCar().moveRoofCar(aPlayer);
                                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aPlayer, newPos.getTrainCar().getRoof(), getIndexByTrainCar(newPos.getTrainCar()));
                                }

                            }
                            //if old whiskey, move to the cabosse and shoot everyone in the way.
                            else if (w.getWhiskeyKind().Equals(WhiskeyKind.Old))
                            {

                                while (pos.getTrainCar().Equals(myTrain[myTrain.Count() - 1]) == false)
                                {

                                    pos = myTrain[myTrain.IndexOf(pos.getTrainCar()) + 1].getInside();
                                    aMarshal.setPosition(pos);
                                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aMarshal, pos, getIndexByTrainCar(pos.getTrainCar()));

                                    foreach (Player aPlayer in pos.getPlayers())
                                    {
                                        BulletCard b = new BulletCard(null, -1);
                                        aPlayer.addToDiscardPile(b);
                                        pos.getTrainCar().moveRoofCar(aPlayer);
                                        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aPlayer, pos.getTrainCar().getRoof(), getIndexByTrainCar(pos.getTrainCar()));
                                    }
                                }

                            }


                            pos.getItems().Remove(w);
                            CommunicationAPI.sendMessageToClient(null, "updateLootAtLocation", pos, getIndexByTrainCar(pos.getTrainCar()), pos.getItems(), true);

                            break;
                        }
                    }


                    break;
                }


        }

    }

    public Boolean getEndOfGame()
    {
        return endOfGame;
    }

    private List<Player> getPossibleShootTarget(Player p)
    {
        List<Player> possPlayers = new List<Player>();
        TrainCar playerCar = p.getPosition().getTrainCar();

        //If player is on roof of stagecoach, all players on the train's roofs are targets
        if (myStageCoach.getRoof().Equals(p.getPosition()))
        {

            for (int i = 0; i < myTrain.Count(); i++)
            {
                List<Player> playersOnWagon = myTrain[i].getRoof().getPlayers();
                if (playersOnWagon.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon);
                    break;
                }
            }
        }
        //If player is inside of stagecoach, add adjacent car's players as targets
        else if (myStageCoach.getInside().Equals(p.getPosition()))
        {
            Position inSC = myStageCoach.getAdjacentCar().getInside();
            List<Player> playersOnWagon = inSC.getPlayers();
            if (playersOnWagon.Count != 0)
            {
                possPlayers.AddRange(playersOnWagon);
            }
        }
        //On a roof
        else if (p.getPosition().isInside() == false)
        {
            // Look for the players in line of sight forward on roof
            for (int i = this.myTrain.IndexOf(playerCar) + 1; i < myTrain.Count; i++)
            {

                List<Player> playersOnWagon = this.myTrain[i].getRoof().getPlayers();
                if (playersOnWagon.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon);
                    break;
                }

                // If the shotgun is in the line of sight, then he blocks it
                if (aShotGun.getIsOnStageCoach() == false)
                {
                    if (this.myTrain[i].getRoof().hasShotgun(aShotGun))
                    {
                        break;
                    }
                }
            }

            // Look for the players in line of sight backwards on roof
            for (int i = this.myTrain.IndexOf(playerCar) - 1; i >= 0; i--)
            {

                List<Player> playersOnWagon2 = this.myTrain[i].getRoof().getPlayers();
                if (playersOnWagon2.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon2);
                    break;
                }
                // If the shotgun is in the line of sight, then he blocks it
                if (aShotGun.getIsOnStageCoach() == false)
                {
                    if (this.myTrain[i].getRoof().hasShotgun(aShotGun))
                    {
                        break;
                    }
                }
            }

            //Look for player on the roof of the stageCoach, if the playerCar is adjacent to the stageCoach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {
                List<Player> playersOnWagon3 = this.myStageCoach.getRoof().getPlayers();
                if (playersOnWagon3.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon3);
                }
            }

            //If player is tuco, then he can shoot on the interior of the same wagon 
            if (currentPlayer.getBandit().Equals(Character.Tuco) && currentPlayer.getHasSpecialAbility())
            {
                List<Player> playersInside = p.getPosition().getTrainCar().getInside().getPlayers();
                if (playersInside.Count != 0)
                {
                    possPlayers.AddRange(playersInside);
                }
            }
        }
        //Inside
        else
        {
            // Look for the players in the next wagon backwards
            try
            {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) - 1].getInside().getPlayers());
            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }

            // Loof for the players in the next wagon forward
            try
            {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside().getPlayers());
            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }
            //Look for player inside the stageCoach, if the playerCar is adjacent to the stageCoach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {

                List<Player> playersOnWagon3 = this.myStageCoach.getInside().getPlayers();
                if (playersOnWagon3.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon3);
                }
            }

            //If player is tuco, then he can shoot the roof of the same wagon 
            if (currentPlayer.getBandit().Equals(Character.Tuco) && currentPlayer.getHasSpecialAbility())
            {
                List<Player> playersOnTheRoof = p.getPosition().getTrainCar().getRoof().getPlayers();
                if (playersOnTheRoof.Count != 0)
                {
                    possPlayers.AddRange(playersOnTheRoof);
                }
            }
        }

        // If there is more than one possible player, we remove Belle.
        bool flagRemoveBelle = false;
        if (possPlayers.Count > 1)
        {
            foreach (Player pl in possPlayers)
            {
                if (pl.getBandit().Equals(Character.Belle) && pl.getHasSpecialAbility())
                {
                    flagRemoveBelle = true;
                }
            }
        }

        if (flagRemoveBelle)
        {
            possPlayers.Remove(getPlayerByCharacter(Character.Belle));
        }

        return possPlayers;

    }

    public Position getPositionByIndex(int index, Boolean inside)
    {
        if (index == -1)
        {
            if (inside)
            {
                return this.myStageCoach.getInside();
            }
            else
            {
                return this.myStageCoach.getRoof();
            }
        } 
        else
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

    public GameItem getItemfromTypePosition(WhiskeyStatus aStatus, WhiskeyKind aKind)
    {
        List<GameItem> al = this.currentPlayer.getPosition().getItems();
        foreach (GameItem anItem in al)
        {
            if (anItem is Whiskey)
            {
                //If the requested kind was Unknown, then find the first instance of a Full Whiskey
                if (aKind == WhiskeyKind.Unknown && ((Whiskey)anItem).getWhiskeyStatus() == WhiskeyStatus.Full)
                {
                    return (Whiskey)anItem;
                } 
                //Otherwise, there must be a dropped visible Normal or Old whiskey at the location - meaning they should be HALF-FULL
                else if (((Whiskey)anItem).getWhiskeyKind() == aKind && ((Whiskey)anItem).getWhiskeyStatus() == aStatus)
                {
                    return (Whiskey)anItem;
                }
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
        //Console.WriteLine("INDEX AT " + index +" WITH "+ ((ActionCard)al[index]).getKind());
        return (ActionCard)al[index];
    }

    public int getIndexByTrainCar(TrainCar trainCar)
    {
        return myTrain.IndexOf(trainCar);
    }

    //C# does a remainder, not a true modulo
    private int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}

