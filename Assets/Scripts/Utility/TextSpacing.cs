using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextSpacing : MonoBehaviour
{
    public float spacing = 1.0f; // 字符间距

    private Text textComponent;
    private string originalText;

    void Start()
    {
        textComponent = GetComponent<Text>();
    }

    void UpdateText()
    {
        if (textComponent == null || originalText == null) return;

        string spacedText = "";
        foreach (char c in originalText)
        {
            spacedText += c;
            spacedText += "<size=1>" + new string(' ', Mathf.FloorToInt(spacing)) + "</size>"; // 添加间距
        }

        textComponent.text = spacedText;
    }

    public void SetText(string newText)
    {
        originalText = newText;
        UpdateText();
    }
}
