using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIFitter : MonoBehaviour
{
    public float Offset = 0;

    void Start()
    {
        Platform.Instance.ADAPTATION(GetComponent<RectTransform>(), Offset);
    }
}
