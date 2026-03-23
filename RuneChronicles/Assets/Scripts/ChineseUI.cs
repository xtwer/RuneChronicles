using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 中文UI辅助类 - 统一处理中文显示
/// </summary>
public static class ChineseUI
{
    private static Font chineseFont;
    
    /// <summary>
    /// 获取支持中文的字体
    /// </summary>
    public static Font GetChineseFont()
    {
        if (chineseFont == null)
        {
            chineseFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        return chineseFont;
    }
    
    /// <summary>
    /// 创建支持中文的Text组件
    /// </summary>
    public static Text CreateText(GameObject obj, string text, int fontSize, TextAnchor alignment, Color color)
    {
        var textComponent = obj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = color;
        textComponent.font = GetChineseFont();
        textComponent.resizeTextForBestFit = false;
        return textComponent;
    }
    
    /// <summary>
    /// 创建标题文字
    /// </summary>
    public static Text CreateTitle(GameObject obj, string text)
    {
        var textComponent = CreateText(obj, text, 56, TextAnchor.MiddleCenter, Color.white);
        textComponent.fontStyle = FontStyle.Bold;
        return textComponent;
    }
    
    /// <summary>
    /// 创建按钮文字
    /// </summary>
    public static Text CreateButtonText(GameObject obj, string text)
    {
        return CreateText(obj, text, 32, TextAnchor.MiddleCenter, Color.white);
    }
    
    /// <summary>
    /// 创建描述文字
    /// </summary>
    public static Text CreateDescription(GameObject obj, string text)
    {
        return CreateText(obj, text, 20, TextAnchor.UpperLeft, Color.white);
    }
    
    /// <summary>
    /// 创建小标题
    /// </summary>
    public static Text CreateSubtitle(GameObject obj, string text)
    {
        return CreateText(obj, text, 28, TextAnchor.MiddleCenter, Color.white);
    }
}
