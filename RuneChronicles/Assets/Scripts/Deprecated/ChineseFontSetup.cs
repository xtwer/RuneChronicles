using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    /// <summary>
    /// 配置 TextMesh Pro 支持中文
    /// </summary>
    public class ChineseFontSetup
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Setup Chinese Font")]
        public static void SetupChineseFont()
        {
            // 1. 导入 TextMesh Pro Essential Resources（如果还没导入）
            Debug.Log("Step 1: Make sure TMP Essential Resources are imported.");
            Debug.Log("Go to: Window → TextMeshPro → Import TMP Essential Resources");
            
            // 2. 查找或创建中文字体资源
            // Unity 自带的 Arial 字体支持部分中文
            var arialFont = Resources.Load<Font>("Arial");
            
            if (arialFont == null)
            {
                Debug.LogWarning("Arial font not found. Using system font instead.");
                arialFont = Font.CreateDynamicFontFromOSFont("Arial Unicode MS", 14);
            }
            
            // 3. 提示用户手动设置
            Debug.Log("===== 中文字体配置步骤 =====");
            Debug.Log("1. 在 Project 窗口中，创建 TextMesh Pro Font Asset:");
            Debug.Log("   右键 → Create → TextMeshPro → Font Asset");
            Debug.Log("2. 选择系统中支持中文的字体（例如：");
            Debug.Log("   - macOS: PingFang SC, Heiti SC, STHeiti");
            Debug.Log("   - Windows: Microsoft YaHei, SimSun");
            Debug.Log("3. 在 Font Asset 设置中:");
            Debug.Log("   - Atlas Population Mode: Dynamic");
            Debug.Log("   - 或者添加常用汉字到 Character Set");
            Debug.Log("4. 将生成的 Font Asset 设置为 TMP 默认字体:");
            Debug.Log("   Edit → Project Settings → TextMesh Pro → Settings");
            Debug.Log("   → Default Font Asset");
            Debug.Log("==============================");
            
            EditorUtility.DisplayDialog(
                "中文字体配置", 
                "请按照 Console 中的步骤手动配置中文字体。\n\n" +
                "或者使用动态字体（推荐）：\n" +
                "将所有 TextMeshProUGUI 组件的 Font Asset 改为 'LiberationSans SDF'，\n" +
                "并勾选 'Enable Dynamic Font Features'。",
                "OK"
            );
        }
#endif
    }
}
