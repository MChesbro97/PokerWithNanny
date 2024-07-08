using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int TotalChips { get; private set; }
    public int CurrentBet { get; private set; }
    public bool IsInRound { get; set; }

    public Player(int initialChips)
    {
        TotalChips = initialChips;
        CurrentBet = 0;
        IsInRound = true;
    }

    public bool Bet(int amount)
    {
        if (amount > TotalChips)
        {
            return false; // Not enough chips to bet the specified amount
        }

        TotalChips -= amount;
        CurrentBet += amount;
        return true;
    }

    public void Fold()
    {
        IsInRound = false;
        CurrentBet = 0;
    }

    public void ResetBet()
    {
        CurrentBet = 0;
    }

    public void AddWinnings(int amount)
    {
        TotalChips += amount;
    }
}

