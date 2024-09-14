using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//弓：瞄准射击
public class Attack_Arrow_Focus : AttackMode
{
    public Attack_Arrow_Focus(Player player, int count) : base(player, count)
    {
    }


    public bool Useable()
    {
        foreach (var e in Field.Instance.Spawn.Enemys) {
            if (e.IsValid) return true;
        }

        return false;
    }

    public override void Execute()
    {
        if (!Useable()) return;

        List<GameObject> enemys = new List<GameObject>();
        Field.Instance.Spawn.Enemys.ForEach(e => {
            if (e.IsValid) enemys.Add(e.gameObject);
        });

        GameObject[] EnemiesToSort = enemys.ToArray();

        //获取自身的位置
        Vector3 o_pos = Belong.transform.localPosition;
        
        //使用 LINQ 根据距离进行排序
        GameObject[] sortedObjects = EnemiesToSort.OrderBy(obj => Vector3.Distance(o_pos, obj.transform.localPosition)).ToArray();

        
        for (int i = 0; i < Count; i++)
        {
            if (i >= sortedObjects.Length) break;

            Enemy e = sortedObjects[i].GetComponent<Enemy>();

            //向目标发射子弹
            var bullet = Field.Instance.CreateBullet(Belong);
            bullet.Shoot(ToolUtility.VectorToAngle(e.transform.localPosition - Belong.transform.localPosition));
        }
    }
}
