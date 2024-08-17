using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//关卡
public class Level
{
    public int ID;
    public LevelJSON LevelJSON;

    public Level(int id)
    {
        ID = id;
    }

    public void Init()
    {
        LevelJSON = (LevelJSON)GameFacade.Instance.DataCenter.Levels.LoadLevelJSON(ID);
    }
}
