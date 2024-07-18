using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_ParticleSystem;

    public void Init(int jelly_id)
    {
        // ParticleSystem.ShapeModule shape = m_ParticleSystem.shape;
        // shape.texture = Resources.Load<Texture2D>("UI/Element/jelly_" + jelly_id);
    }
}
