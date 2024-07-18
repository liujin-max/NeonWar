using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoxColliderUtility : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D box = transform.GetComponent<BoxCollider2D>();
        if (box == null) {
            box = transform.AddComponent<BoxCollider2D>();
        }

        box.size = GetComponent<RectTransform>().rect.size;
    }
}
