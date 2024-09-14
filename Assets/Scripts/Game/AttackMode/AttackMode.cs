using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackMode
{
    public Player Belong;
    public int Count { get; set;}


    public AttackMode(Player player, int count)
    {
        Belong  = player;
        Count   = count;
    }

    public abstract void Execute();
}
