




using System;
using UnityEngine;

/// <summary>
/// 事件系统 所有事件的集合
/// 代替原先的EVENT和EventManager
/// </summary>


#region 全屏遮罩
public class Event_PopupMask : BaseEvent<Event_PopupMask>
{
    public bool IsShow; 

}
#endregion



#region 通用提示
public class Event_PopupTip : BaseEvent<Event_PopupTip>
{
    public string Text;
    public bool IsSilence;
}
#endregion



#region 技能升级
public class Event_SkillUpgrade : BaseEvent<Event_SkillUpgrade>
{


}
#endregion



#region 装备变化
public class Event_PearChange : BaseEvent<Event_PearChange>
{
    public bool IsEquip;
    public Pear Pear;
    public PearSlotMsg Slot;
}
#endregion



#region 装备合成
public class Event_PearCompose : BaseEvent<Event_PearCompose>
{
    public Pear Pear;
    
}
#endregion



#region 装备合成操作
public class Event_PearComposeChange : BaseEvent<Event_PearComposeChange>
{
    public bool IsPush;
    public Pear Pear;
}
#endregion



#region 点击装备
public class Event_SelectPear : BaseEvent<Event_SelectPear>
{
    public Pear Pear;
}
#endregion



#region 开关背包
public class Event_BackpackOpen : BaseEvent<Event_BackpackOpen>
{
    public bool IsOpen;
}
#endregion



#region 刷新怪物进度
public class Event_EnemyProgress : BaseEvent<Event_EnemyProgress>
{
    public int Count;
}
#endregion



#region 显示战斗按钮
public class Event_FightShow : BaseEvent<Event_FightShow>
{
    public bool Flag;
}
#endregion



#region 刷新碎片
public class Event_UpdateGlass : BaseEvent<Event_UpdateGlass>
{

}
#endregion



#region 触摸方向
public class Event_JoystickPress : BaseEvent<Event_JoystickPress>
{
    public int Direction;
}
#endregion






#region 战斗开始
public class Event_BattleStart : BaseEvent<Event_BattleStart>
{

}
#endregion



#region 战斗结束
public class Event_BattleEnd : BaseEvent<Event_BattleEnd>
{
    
}
#endregion



#region Hit
public class Event_Hit : BaseEvent<Event_Hit>
{
    public Hit Hit;
    public Unit Unit;
}
#endregion



#region Crash
public class Event_Crash : BaseEvent<Event_Crash>
{
    public Enemy Caster;
    public Player Target;
}
#endregion



#region 玩家血量变化
public class Event_UpdateHP : BaseEvent<Event_UpdateHP>
{

}
#endregion



#region 创建Bullet
public class Event_BulletCreate : BaseEvent<Event_BulletCreate>
{
    public Bullet Bullet;
}
#endregion



#region 发射Bullet
public class Event_BulletShoot : BaseEvent<Event_BulletShoot>
{
    public Bullet Bullet;
}
#endregion



#region Bullet击中目标
public class Event_BulletHit : BaseEvent<Event_BulletHit>
{
    public Bullet Bullet;
    public Unit Target;
}
#endregion



#region 添加Buff
public class Event_BuffADD : BaseEvent<Event_BuffADD>
{
    public Buff Buff;
}
#endregion



#region 移除Buff
public class Event_BuffRemove : BaseEvent<Event_BuffRemove>
{
    public Buff Buff;
}
#endregion



#region 闪避
public class Event_Dodge : BaseEvent<Event_Dodge>
{
    public Unit Unit;
}
#endregion



#region 击杀Enemy
public class Event_KillEnemy : BaseEvent<Event_KillEnemy>
{
    public Enemy Enemy;
    public Hit Hit;
}
#endregion



#region 添加Area
public class Event_AreaPush : BaseEvent<Event_AreaPush>
{
    public Area Area;
}
#endregion



#region 移除Area
public class Event_AreaRemove : BaseEvent<Event_AreaRemove>
{
    public Area Area;
}
#endregion



#region 投掷陷阱
public class Event_PlayTrap : BaseEvent<Event_PlayTrap>
{
    public Skill Skill;
}
#endregion







//事件基类
public class BaseEvent<T> where T : BaseEvent<T>
{
    public static event Action<T> OnEvent = null;

    public void Notify()
    {
        OnEvent?.Invoke((T)this);
    }
}
