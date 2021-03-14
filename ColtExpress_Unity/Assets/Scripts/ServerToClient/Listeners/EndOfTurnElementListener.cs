using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfTurnElementListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        //Data should contain
        //currentTurn, currentPlayer, GameStatus enum, currentPlayer hasAnotherAction bool, waitingForInput bool
        //JSON String parse somewhere here instead



        //If currentPlayer has another action, let UI let them draw cards or play a card

        //If waitingForInput is false lock UI for currentPlayer



        //Update currentPlayer if in same turn
        
        //Update currentTurn and currentPlayer if next turn

        //If lastTurn, game is moved to Stealin
        //(if moved to Stealin, use readyForNextMove())


    }
}
