using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roar : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_Sprite;


    public void Play(int id)
    {
        m_Sprite.sprite = GameFacade.Instance.AssetManager.LoadSprite("Battle", id.ToString());
    }

    public void SetRotation(Vector3 vector3)
    {
        m_Sprite.transform.localEulerAngles = vector3;
    }
}
