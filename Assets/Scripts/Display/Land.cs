using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


//负责游戏场景的表现逻辑
public class Land : MonoBehaviour
{
    public Transform ENTITY;
    public Transform ENTITY_ROOT;
    public Transform ELEMENT_ROOT;

    public CameraUtility SCENE_CAMERA;
    
    
    public Land()
    {
        ENTITY      = GameObject.Find("Field").transform;
        ENTITY_ROOT = GameObject.Find("Field/Entitys").transform;
        ELEMENT_ROOT= GameObject.Find("Field/Elements").transform;


        SCENE_CAMERA= GameObject.FindWithTag("SceneCamera").GetComponent<CameraUtility>();
    }

    public void Dispose()
    {
        SCENE_CAMERA.Reset();
    }



    public void DoShake()
    {
        SCENE_CAMERA.DoShake();
    }

    public void DoSmallShake()
    {
        SCENE_CAMERA.DoSmallShake();
    }
}
