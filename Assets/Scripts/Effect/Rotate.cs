using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float Time = 0.8f;


    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalRotate(new Vector3(0, 0, -360), Time, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
