using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalatroFloating : MonoBehaviour
{
    private int Random = RandomUtility.Random(0, 100);
    [SerializeField] private float Strenth = 4f;

    void FixedUpdate()
    {
        float sine = Mathf.Sin(Time.time + Random) * Strenth;
        float cosine = Mathf.Cos(Time.time + Random) * Strenth;
        transform.localEulerAngles = new Vector3(sine, cosine, 0);
    }
}
