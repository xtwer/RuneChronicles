using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI字体修复器 - 自动为所有Text组件设置中文字体
/// </summary>
public class UIFontFixer : MonoBehaviour
{
    void Start()
    {
        FixAllTextFonts();
    }
    
    void FixAllTextFonts()
    {
        Font chineseFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        if (chineseFont == null)
        {
            Debug.LogError("[UIFontFixer] 无法加载中文字体！");
            return;
        }
        
        // 查找所有Text组件
        Text[] allTexts = Resources.FindObjectsOfTypeAll<Text>();
        int fixedCount = 0;
        
        foreach (Text text in allTexts)
        {
            if (text != null && text.gameObject.scene.isLoaded)
            {
                text.font = chineseFont;
                fixedCount++;
            }
        }
        
        Debug.Log($"[UIFontFixer] 已修复 {fixedCount} 个Text组件的字体");
    }
}
