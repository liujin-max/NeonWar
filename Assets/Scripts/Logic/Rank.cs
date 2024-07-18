using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



//排行榜数据
[System.Serializable]
public class RankData
{
    public string OpenID;
    public string Name;
    public string Head;
    public int Value;
    public int Order;

}

[System.Serializable]
public class RankDataInfo
{
    public RankData[] data;
}


public class Rank
{
    private static Rank m_instance = null;
    public static Rank Instance
    {
        get {
            if (m_instance == null){
                m_instance = new Rank();
            }
            return m_instance; 
        }
    }



    //将上次拉取到的排行榜记录在本地
    private List<RankData> m_RankList = new List<RankData>();


    private RankData m_GoalRank = null;
    public RankData GoalRank {get { return m_GoalRank;}}
    private bool m_SyncFlag = false;
    

    //刷新本地排行榜
    public void SyncRank(_C.RANK rank_type, RankData[] rankDatas)
    {
        if (rank_type == _C.RANK.ENDLESS)
        {
            m_SyncFlag = true;
            m_RankList.Clear();
            for (int i = 0; i < rankDatas.Length; i++) {
                var rankData    = rankDatas[i];
                rankData.Order  = i + 1;
                m_RankList.Add(rankData);
            }
        }
        else if (rank_type == _C.RANK.LEVEL)
        {
            for (int i = 0; i < rankDatas.Length; i++) {
                var rankData    = rankDatas[i];
                rankData.Order  = i + 1;
            }
        }
    }

    public bool IsSyncFinished()
    {
        return m_SyncFlag;
    }

    public void GenerateGoalRank()
    {
        //重置
        m_GoalRank  = null;

        if (m_RankList.Count == 0) return;

        int my_order    = this.GetMyRankOrder();

        //计算目标排名
        if (my_order == _C.DEFAULT_RANK)
        {
            m_GoalRank  = m_RankList.Last();
        }
        else if (my_order == 1)
        {
            m_GoalRank  = m_RankList.First();
        }
        else
        {
            m_GoalRank  = m_RankList[my_order - 2];
        }
    }

    public bool NextGoalRank()
    {
        //已经是第一名了，不停刷新分数
        // if (m_GoalRank.Order == 1) {
        //     m_GoalRank.Head = GameFacade.Instance.DataCenter.User.HeadURL;
        //     m_GoalRank.Value = Field.Instance.Stage.GetScore();
        //     return false;
        // }

        // m_GoalRank  = m_RankList[m_GoalRank.Order - 2];

        return true;
    }

    //获取我的排名
    public int GetMyRankOrder()
    {
        if (m_RankList.Count == 0 && GameFacade.Instance.DataCenter.User.Score > 0) return 1;

        for (int i = 0; i < m_RankList.Count; i++) {
            var rankData = m_RankList[i];
            if (GameFacade.Instance.DataCenter.User.Score >= rankData.Value) {
                return rankData.Order;
            }
        }

        return _C.DEFAULT_RANK;
    }


}

