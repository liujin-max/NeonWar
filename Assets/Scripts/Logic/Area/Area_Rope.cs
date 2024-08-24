using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//引力陷阱
public class Area_Rope : Area
{
    //自定义的Update函数
    public override void CustomUpdate(float deltaTime)
    {
        base.CustomUpdate(deltaTime);


        m_Units.ForEach(u => {
            Enemy e = u.GetComponent<Enemy>();
            if (e != null) {
                e.Repel((transform.localPosition - e.transform.localPosition) * 0.15f);
            }
        });
    }
}
