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
        m_Text.text = "<#9C9C9C>" + "· " + CONST.COLOR_BROWN + "+" + property.GetDescription() + " " + property.Name;
    }
}
