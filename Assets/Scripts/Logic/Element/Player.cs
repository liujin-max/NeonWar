using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float m_Angle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, m_Angle + 90);
    }

    public void Move(float angle)
    {
        m_Angle = angle;

        Vector2 point = ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, angle);
        this.SetPosition(point);
    }

    public void SetPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }
}
