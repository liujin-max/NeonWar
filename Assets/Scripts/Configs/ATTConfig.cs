using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ATT", menuName = "ScriptableObject/单位属性")]
public class ATTConfig : ScriptableObject
{
    public int HPMAX   = 3;
    [HideInInspector] public int HP = 3;
    [HideInInspector] public AttributeValue ATK  = new AttributeValue(1);
    [Header("攻速(毫秒)")] public AttributeValue ASP = new AttributeValue(0);    //攻速 
    [Header("暴击率(千分制)")] public AttributeValue CP = new AttributeValue(0);
    [Header("暴击伤害(千分制)")] public AttributeValue CT = new AttributeValue(0);
    [Header("闪避率(千分制)")] public AttributeValue DODGE = new AttributeValue(0);
    [Header("移动速度")] public AttributeValue SPEED = new AttributeValue(0);
    
    //易伤倍率
    [HideInInspector] public AttributeValue VUN_INC     = new AttributeValue(1f, false);
    //对头目伤害加成
    [HideInInspector] public AttributeValue BOSS_INC    = new AttributeValue(1f, false);
}
