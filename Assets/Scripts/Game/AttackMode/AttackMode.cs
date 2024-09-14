using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//攻击模式
public abstract class AttackMode
{
    public Player Belong;
    public int Count { get; set;}


    public AttackMode(Player player, int count)
    {
        Belong  = player;
        Count   = count;
    }

    public virtual bool Useable()
    {
        return true;
    }

    public abstract void Execute();
}
