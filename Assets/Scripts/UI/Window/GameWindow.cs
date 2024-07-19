using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWindow : MonoBehaviour
{
    [SerializeField] Joystick m_Joystick;

    void Awake()
    {
       ShowJoyStick(false); 
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowJoyStick(bool flag)
    {
        m_Joystick.gameObject.SetActive(flag);
    }
}
