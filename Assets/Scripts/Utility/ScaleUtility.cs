using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleUtility : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one , 0.3f).SetEase(Ease.OutElastic);
    }
}
