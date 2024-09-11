using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//荆棘区域
//玩家路过此区域时会减速
public class Area_Thorns : Area
{
    //进入区域
    //进入区域后要做的逻辑写这里面
    public override void Enter(Unit unit)
    {
        unit.ATT.SPEED.PutMUL(this, 0.3f);
    }

    //离开区域时
    //重写此方法，把离开区域后要做的逻辑写这里面
    public override void Exit(Unit unit)
    {
        unit.ATT.SPEED.Pop(this);
    }

}
