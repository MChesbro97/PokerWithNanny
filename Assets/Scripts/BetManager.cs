using System.Collections.Generic;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    private List<Player> players;
    private int dealerIndex;
    private int smallBlindIndex;
    private int bigBlindIndex;
    private int currentPlayerIndex;
    private int totalPot;
    private int currentBet;
    private int smallBlindAmount;
    private int bigBlindAmount;

    public BetManager(List<Player> players, int smallBlindAmount, int bigBlindAmount)
    {
        this.players = players;
        this.smallBlindAmount = smallBlindAmount;
        this.bigBlindAmount = bigBlindAmount;
        dealerIndex = 0;
        smallBlindIndex = 1;
        bigBlindIndex = 2;
        currentPlayerIndex = (bigBlindIndex + 1) % players.Count;
        totalPot = 0;
        currentBet = bigBlindAmount;
    }

    public void StartNewRound()
    {
        totalPot = 0;
        currentBet = bigBlindAmount;
        currentPlayerIndex = (bigBlindIndex + 1) % players.Count;
        foreach (var player in players)
        {
            player.ResetBet();
            player.IsInRound = true;
        }

        PlaceBlind(smallBlindIndex, smallBlindAmount);
        PlaceBlind(bigBlindIndex, bigBlindAmount);
    }

    private void PlaceBlind(int playerIndex, int blindAmount)
    {
        if (players[playerIndex].Bet(blindAmount))
        {
            totalPot += blindAmount;
        }
        else
        {
            // Handle situation where player doesn't have enough chips for blind
            totalPot += players[playerIndex].TotalChips;
            players[playerIndex].Bet(players[playerIndex].TotalChips);
        }
    }

    public void PlayerFold()
    {
        players[currentPlayerIndex].Fold();
        MoveToNextPlayer();
    }

    public void PlayerCall()
    {
        int amountToCall = currentBet - players[currentPlayerIndex].CurrentBet;
        if (players[currentPlayerIndex].Bet(amountToCall))
        {
            totalPot += amountToCall;
        }
        else
        {
            // Player goes all-in
            totalPot += players[currentPlayerIndex].TotalChips;
            players[currentPlayerIndex].Bet(players[currentPlayerIndex].TotalChips);
        }
        MoveToNextPlayer();
    }

    public void PlayerRaise(int raiseAmount)
    {
        int amountToCall = currentBet - players[currentPlayerIndex].CurrentBet;
        int totalBet = amountToCall + raiseAmount;

        if (players[currentPlayerIndex].Bet(totalBet))
        {
            totalPot += totalBet;
            currentBet += raiseAmount;
        }
        else
        {
            // Player goes all-in with less than the raise amount
            totalPot += players[currentPlayerIndex].TotalChips;
            players[currentPlayerIndex].Bet(players[currentPlayerIndex].TotalChips);
            currentBet = players[currentPlayerIndex].CurrentBet;
        }
        MoveToNextPlayer();
    }

    private void MoveToNextPlayer()
    {
        do
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        } while (!players[currentPlayerIndex].IsInRound);
    }

    public void NextDealer()
    {
        dealerIndex = (dealerIndex + 1) % players.Count;
        smallBlindIndex = (dealerIndex + 1) % players.Count;
        bigBlindIndex = (dealerIndex + 2) % players.Count;
        currentPlayerIndex = (bigBlindIndex + 1) % players.Count;
    }

    public int GetTotalPot()
    {
        return totalPot;
    }

    public int GetCurrentBet()
    {
        return currentBet;
    }
}
