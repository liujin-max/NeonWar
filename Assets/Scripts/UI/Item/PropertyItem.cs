using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PropertyItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;


    public void Init(Property property)
    {
        m_Text.text = CONST.COLOR_BROWN + property.Name + ":" + property.GetDescription();
    }
}
