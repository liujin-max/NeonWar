using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//基础数据
[System.Serializable]
public struct BaseData
{
    public string openId;
}

//道具数据
[System.Serializable]
public struct PropMsg
{
    public int ID;
    public int Count;
}

//任务记录数据
[System.Serializable]
public class TaskMsg
{
    public int ID;
    public int Day; //上次领取的日期
}

//宝珠槽数据
[System.Serializable]
public class PearSlotMsg
{
    public int UUID;

    [NonSerialized] public Pear Pear;

    public void UpdateUUID(int uuid)
    {
        UUID = uuid;
        Pear = DataCenter.Instance.Backpack.GetPearByUUID(uuid);
    }
}

//技能槽数据
[System.Serializable]
public class SkillSlotMsg
{
    public int ID = -1;
    public int Level;


    //解锁条件及技能池
    [NonSerialized] public int ATK;
    [NonSerialized] public int ASP;
    [NonSerialized] public int WOR;
    [NonSerialized] public string POS;  //对应SkillTreeItem的SkillPivot下的支点名称
    [NonSerialized] public int[] SkillPool;

    
    public bool IsUnlock()
    {
        PlayerMsg playerMsg = DataCenter.Instance.User.CurrentPlayer;
        return playerMsg.ATK >= ATK && playerMsg.ASP >= ASP && playerMsg.WORTH >= WOR;
    }

    public bool IsLevelMax()
    {
        var skill_data = DataCenter.Instance.League.GetSkillData(ID);
        if (skill_data != null) {
            return Level >= skill_data.LevelMax;
        }
        return false;
    }
}

//武器数据
[System.Serializable]
public class PlayerMsg
{
    public int ID;
    public bool UnlockFlag = false;
    public bool InUse;  //使用中
    public int ATK = 1;
    public int ASP = 1;
    public int WORTH = 1;


    [NonSerialized] public string UI;
    //技能池
    [NonSerialized] public int[] SkillPools;

    //5个技能槽
    public List<SkillSlotMsg> SkillSlots;

    //N个宝珠槽(每种武器拥有的宝珠槽数量不同)
    public List<PearSlotMsg> PearSlots;

    //养成过
    public bool IsDevelop()
    {
        return ATK > 1 || ASP > 1 || WORTH > 1;
    }
}

//宝珠数据
[System.Serializable]
public class PearMsg
{
    public int ID;
    public int UUID;
    public int Level;
    public string[] Properties;
}


//账号数据
[System.Serializable]
public class GameUserData
{
    public int Level;       //通关记录
    public int Coin;        //金币
    public int Glass;       //碎片

    //当前只开放了1个角色
    public List<PlayerMsg> Players = new List<PlayerMsg>()
    {
        //弓
        new PlayerMsg() 
        {
            ID          = (int)PLAYER.BOW, 
            UI          = "Bow",
            UnlockFlag  = true, 
            InUse       = true, 
            SkillSlots  = new List<SkillSlotMsg>()
            {
                //攻击分支
                new SkillSlotMsg() {ATK = 3, POS = "1", SkillPool = new int[] {10010, 10020, 10030}},
                new SkillSlotMsg() {ATK =15, POS = "2", SkillPool = new int[] {10060, 10070}},

                //攻速分支
                new SkillSlotMsg() {ASP = 5, POS = "11", SkillPool = new int[] {10110, 10120, 10130}},
                new SkillSlotMsg() {ASP =20, POS = "12", SkillPool = new int[] {10160, 10170}},

                //价值分支
                new SkillSlotMsg() {WOR = 8, POS = "21", SkillPool = new int[] {10210, 10220, 10230}},
                new SkillSlotMsg() {WOR =25, POS = "22", SkillPool = new int[] {10260, 10270, 10280}},
            },

            PearSlots   = new List<PearSlotMsg>()   //2个宝珠槽
            {
                new PearSlotMsg() {UUID = -1},
                new PearSlotMsg() {UUID = -1},
            },
        }
    };

    //道具数据
    public List<PearMsg> Pears = new List<PearMsg>();

    public int GlobalUUID;

    public long RecoveryTimestamp;    //上次体力的恢复时间
    public int RegisterDay;     //注册时间(一年中的第几天)
    public int LastLoginDay;    //上次登录的天数
    public int LoginDay;        //登录天数
    public int SignInStatus;  //签到状态
}




//负责管理账号的各种数据
public class User
{
    //用户数据
    private BaseData m_Base = new BaseData();
    public BaseData Base { 
        get { return m_Base; } 
        set { m_Base = value; }
    }

    private GameUserData m_Data = new GameUserData();
    public GameUserData Data {
        get { return m_Data; }
        set { m_Data = value; }
    }

    public string OpenID { get{ return m_Base.openId;}}
    public int Level { get{ return m_Data.Level;}}
    public int Coin { get{ return m_Data.Coin;}}
    public int Glass { get{ return m_Data.Glass;}}

    //获取当前正在使用的武器
    public PlayerMsg CurrentPlayer {
        get { 
            foreach (var player_msg in m_Data.Players) {
                if (player_msg.InUse == true) 
                    return player_msg;
            }

            m_Data.Players[0].InUse = true;
            return m_Data.Players[0];
        }
    }

    private bool m_userUpdate = false;  //账号数据变动标记
    public bool IsDirty {get { return m_userUpdate;} set { m_userUpdate = value;}}

    

    public bool IsSyncFinished = false;
    
    //从存档里同步数据
    public void Sync()
    {
        Platform.Instance.SYNC();
    }

    public void SyncFinish()
    {
        IsSyncFinished = true;
    }

    public void SyncRecords()
    {
        int day_of_year = DateTime.Now.DayOfYear;

        //同步宝珠数据
        DataCenter.Instance.Backpack.SyncPears(m_Data.Pears);

        //刷新道具槽数据
        foreach (var pearSlotMsg in CurrentPlayer.PearSlots) pearSlotMsg.UpdateUUID(pearSlotMsg.UUID);
        UpdateSpecialPropertyValid();


        //同步任务信息
        // m_Data.TaskRecords.ForEach(msg => {
        //     if (msg.Day == day_of_year)
        //     {
        //         var t = DataCenter.Instance.Daily.GetTask(msg.ID);
        //         if (t != null) {
        //             t.Receive();
        //         }
        //     }
        // });


        //任务：每日登录
        // DataCenter.Instance.Daily.FinishTask((int)CONST.TASK.LOGIN);


        //未注册|新的一年重新开始
        // if (m_Data.RegisterDay == 0 || day_of_year < m_Data.RegisterDay)
        // {
        //     m_Data.RegisterDay = day_of_year;
        //     m_Data.LoginDay    = 0;
 
        //     m_userUpdate = true;
        // }

        // //记录登录天数
        // if (day_of_year != m_Data.LastLoginDay) 
        // {
        //     m_Data.LastLoginDay = day_of_year;
        //     m_Data.LoginDay++;

        //     m_userUpdate = true;
        // }

        

    }


    public void Upload()
    {
        if (!m_userUpdate) return;

        m_Data.Pears = DataCenter.Instance.Backpack.ExportPears();

        Platform.Instance.UPLOAD(m_Data);

        //重置标记
        m_userUpdate    = false;
    }

    public int CreateUUID()
    {
        m_Data.GlobalUUID++;

        m_userUpdate = true;

        return m_Data.GlobalUUID;
    }


    public void SetLevel(int value)
    {
        if (value <= m_Data.Level ) return;

        m_Data.Level = value;

        m_userUpdate = true;
    }

    public void SetCoin(int value)
    {
        m_Data.Coin = value;

        m_userUpdate = true;
    }

    public void UpdateCoin(int value)
    {
        m_Data.Coin  = Mathf.Max(0, m_Data.Coin + value);

        m_userUpdate = true;
    }

    public void UpdateGlass(int value)
    {
        m_Data.Glass = Mathf.Max(0, m_Data.Glass + value);

        m_userUpdate = true;
    }

    public void UpdateProperty(ATT property, int value)
    {
        switch (property)
        {
            case ATT.ATK: CurrentPlayer.ATK += value; break;
            case ATT.ASP: CurrentPlayer.ASP += value; break;
            case ATT.WORTH: CurrentPlayer.WORTH += value; break;
        }

        m_userUpdate = true;
    }

    public int GetPropertyCost(ATT property)
    {
        switch (property)
        {
            case ATT.ATK: return NumericalManager.FML_ATKCost(CurrentPlayer.ATK);
            case ATT.ASP: return NumericalManager.FML_ASPCost(CurrentPlayer.ASP);
            case ATT.WORTH: return NumericalManager.FML_WORTHCost(CurrentPlayer.WORTH);
        }

        return 1;
    }

    public int GetPropertyLevel(ATT property)
    {
        switch (property)
        {
            case ATT.ATK: return CurrentPlayer.ATK;
            case ATT.ASP: return CurrentPlayer.ASP;
            case ATT.WORTH: return CurrentPlayer.WORTH;
        }

        return 1;
    }

    //升级技能
    public void UpgradeSkill(SkillSlotMsg slotMsg, int skill_id)
    {
        slotMsg.ID      = skill_id;
        slotMsg.Level   += 1;

        m_userUpdate = true;
    }

    //重置技能
    public void ResetSkill(SkillSlotMsg skill_slot)
    {
        skill_slot.ID       = -1;
        skill_slot.Level    = 0;

        m_userUpdate = true;
    }

    public SkillSlotMsg GetSkillSlotMsg(int order)
    {
        return CurrentPlayer.SkillSlots[order];
    }

    //宝珠槽是不是满了
    public bool IsPearSeatsFull()
    {
        for (int i = 0; i < CurrentPlayer.PearSlots.Count; i++)
        {
            var slot = CurrentPlayer.PearSlots[i];
            if (slot.UUID == -1) {
                return false;
            }
        }

        return true;
    }

    //穿脱宝珠
    public PearSlotMsg EquipPear(Pear pear)
    {
        //先判断有没有相同id的宝珠，如果有 则替换
        for (int i = 0; i < CurrentPlayer.PearSlots.Count; i++)
        {
            var slot = CurrentPlayer.PearSlots[i];
            if (slot.Pear != null && slot.Pear.ID == pear.ID) {
                m_userUpdate = true;
                slot.UpdateUUID(pear.UUID);
                UpdateSpecialPropertyValid();
                return slot;
            }
        } 


        for (int i = 0; i < CurrentPlayer.PearSlots.Count; i++)
        {
            var slot = CurrentPlayer.PearSlots[i];
            if (slot.UUID == -1) {
                m_userUpdate = true;
                slot.UpdateUUID(pear.UUID);
                UpdateSpecialPropertyValid();
                return slot;
            }
        }

        return null;
    }

    public PearSlotMsg UnloadPear(Pear pear)
    {
        if (pear.SpecialProperty != null) pear.SpecialProperty.IsValid = true;

        for (int i = 0; i < CurrentPlayer.PearSlots.Count; i++)
        {
            var slot = CurrentPlayer.PearSlots[i];
            if (slot.Pear == pear) {
                m_userUpdate = true;
                slot.UpdateUUID(-1);
                UpdateSpecialPropertyValid();
                
                return slot;
            }
        }

        return null;
    }

    //穿脱道具时，对道具的词条做[去重]判断
    //携带相同的特殊词条时，只有属性更好的那个会生效
    void UpdateSpecialPropertyValid()
    {
        Dictionary<int, List<Property>> sp_records = new Dictionary<int, List<Property>>();

        foreach (var pear_slot_msg in CurrentPlayer.PearSlots)
        {
            if (pear_slot_msg.Pear == null) continue;
            if (pear_slot_msg.Pear.SpecialProperty == null) continue;

            var sp = pear_slot_msg.Pear.SpecialProperty;
            if (!sp_records.ContainsKey(sp.ID))
                sp_records.Add(sp.ID, new List<Property>());

            sp_records[sp.ID].Add(sp);
        }

        //参数从大到小排序
        foreach (var config in sp_records)
        {
            config.Value.Sort((a1, b1)=>{
                return b1.Value.CompareTo(a1.Value);
            });

            for (int i = 0; i < config.Value.Count; i++)
            {
                if (i == 0) config.Value[i].IsValid = true;
                else config.Value[i].IsValid = false;
            }
        }
    }

    public bool IsPearEquiped(Pear pear)
    {
        foreach (var slot_msg in CurrentPlayer.PearSlots) {
            if (slot_msg.UUID == pear.UUID) return true;
        }
        return false;
    }

    public bool HasSamePear(Pear pear)
    {
        foreach (var slot_msg in CurrentPlayer.PearSlots) {
            if (slot_msg.Pear != null && slot_msg.Pear.ID == pear.ID) return true;
        }
        return false;
    }



    public void SetRecoveryTimestamp(long value)
    {
        m_Data.RecoveryTimestamp = value;

        m_userUpdate = true;
    }


    //七日签到
    public void RecordsSignIn(int day)
    {
        m_Data.SignInStatus |= (1 << (day - 1));

        m_userUpdate = true;
    }


    public void Update(float dt)
    {   

        if (m_userUpdate) {
            Upload();
        }
    }
}


