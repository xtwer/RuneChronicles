using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    public class UIColorFixer
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Fix UI Colors")]
        public static void FixColors()
        {
            // 修改所有 Panel 的颜色为浅灰色（更明显）
            var panels = Object.FindObjectsOfType<Image>();
            foreach (var panel in panels)
            {
                if (panel.gameObject.name.Contains("Panel") || 
                    panel.gameObject.name.Contains("Bar") ||
                    panel.gameObject.name.Contains("Area"))
                {
                    panel.color = new Color(0.3f, 0.3f, 0.3f, 1f); // 不透明浅灰色
                    Debug.Log($"Fixed color: {panel.gameObject.name}");
                }
                
                // 修改按钮颜色
                if (panel.gameObject.name.Contains("Button"))
                {
                    panel.color = new Color(0.2f, 0.5f, 0.8f, 1f); // 蓝色
                    Debug.Log($"Fixed button color: {panel.gameObject.name}");
                }
            }
            
            // 确保 Canvas 的 Sort Order 正确
            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 100; // 确保在最上层
                Debug.Log("Set Canvas sorting order to 100");
            }
            
            Debug.Log("✅ UI colors fixed! Press Play again.");
        }
#endif
    }
}
