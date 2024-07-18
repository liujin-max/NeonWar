using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Floating : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 计算从startTime开始时的进度
        float progress = RandomUtility.Random(0, 100) / 100.0f;
        // 创建浮动动画
        transform.DOMoveY(transform.position.y + 0.5f, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).Goto(progress, true);

 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
