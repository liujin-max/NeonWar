using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelBroken : MonoBehaviour
{
    [SerializeField] ParticleSystem m_ParticleSystem;

    public void Init(ParticleSystem.MinMaxGradient color)
    {
        ParticleSystem.MainModule main = m_ParticleSystem.main;
        
        // 将颜色设置为两者之间的随机颜色
        main.startColor = color;
    }

}
