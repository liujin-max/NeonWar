using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEvent : MonoBehaviour
{
    public Unit Unit;
    public void DO()
    {
        Unit.DoAttack();
    }
}
