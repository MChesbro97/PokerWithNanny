using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokerGameMode
{
    FiveCardDraw, //the draw part, will do with buttons
    DayBaseball,
    NightBaseball,
    FollowTheQueen,
    Woolworth,
    DeucesAndJacks,
    HighChicago,//Chicago games need to be have their special win case where the pot is split, will do when I've added betting
    LowChicago,
    KingsAndLittleOnes
}
