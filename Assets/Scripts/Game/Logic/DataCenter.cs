using System;
using System.Collections.Generic;
using UnityEngine;







//全局数据类
public class DataCenter
{
    //账号信息
    public User User;
    public League League;
    public Levels Levels;
    public Backpack Backpack;


    private static DataCenter m_Instance;
    public static DataCenter Instance {
        get {
            if (m_Instance == null) m_Instance = new DataCenter();

            return m_Instance;
        }
    }


    public void Init()
    {
        //账号数据
        User    = new User();

        League  = new League();
        League.Init();

        Levels  = new Levels();

        Backpack= new Backpack();
        Backpack.Init();
    }

    public void Update(float dt)
    {
        User.Update(dt);
    }

    public bool IsPearUnlock()
    {
        return User.Level >= CONST.PEAR_UNLOCK_LEVEL;
    }

    public string GetLevelString()
    {
        int level   = User.Level + 1;

        return string.Format("关卡 {0}", level);
    }

    // 合成宝珠
    // 宝珠合成系统，任意相同品质三颗宝珠可以合成更高品质的随机宝珠
    // 三颗相同品质宝珠可以合成下一品质宝珠
    // 如果只花2颗宝珠合成，则成功率只有50%
    public Pear Synthesis(List<Pear> pears)
    {
        if (pears.Count <= 1) return null;

        int success_rate = (pears.Count - 1) * 50;

        // //判断合成手续费是否足够
        // int cost_coin   = 50;

        // if (DataCenter.Instance.User.Coin < cost_coin) {
        //     EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "<sprite=0>不足"));
        //     return null;
        // }

        //扣除消耗
        // DataCenter.Instance.User.UpdateCoin(-cost_coin);

        User.IsDirty = true;

        //消耗宝珠
        pears.ForEach(pear => {
            Backpack.RemovePear(pear.ID, 1);
        });


        if (!RandomUtility.IsHit(success_rate)) {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "合成失败"));
            return null;
        }

        bool is_same    = true;
        int last_id     = pears[0].ID;

        for (int i = 0; i < pears.Count; i++)
        {
            if (pears[i].ID != last_id) {
                is_same = false;
                break;
            }
        }


        int next_id;
        if (is_same == true)
        {
            next_id = pears[0].ID + 1;
        }
        else
        {
            List<PearData> pear_datas = Backpack.GetPearDatasByLevel(pears[0].Level + 1);
            int rand = RandomUtility.Random(0, pear_datas.Count);
            
            next_id = pear_datas[rand].ID;
        }

        Pear pear = Backpack.PushPear(next_id);

        return pear;
    }
}

