using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    public class UILayoutFixer
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Fix UI Layout")]
        public static void FixUILayout()
        {
            // 修复 GamePanel - 铺满整个屏幕
            var gamePanel = GameObject.Find("GamePanel");
            if (gamePanel != null)
            {
                var rect = gamePanel.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                Debug.Log("✅ Fixed GamePanel");
            }
            
            // 修复 TopBar - 顶部横条
            var topBar = GameObject.Find("TopBar");
            if (topBar != null)
            {
                var rect = topBar.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0.5f, 1);
                rect.anchoredPosition = new Vector2(0, 0);
                rect.sizeDelta = new Vector2(0, 100);
                Debug.Log("✅ Fixed TopBar");
            }
            
            // 修复 ButtonBar - 底部横条
            var buttonBar = GameObject.Find("ButtonBar");
            if (buttonBar != null)
            {
                var rect = buttonBar.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0.5f, 0);
                rect.anchoredPosition = new Vector2(0, 0);
                rect.sizeDelta = new Vector2(0, 80);
                Debug.Log("✅ Fixed ButtonBar");
            }
            
            // 修复 HandArea - 中间区域
            var handArea = GameObject.Find("HandArea");
            if (handArea != null)
            {
                var rect = handArea.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.1f, 0.2f);
                rect.anchorMax = new Vector2(0.9f, 0.8f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                
                // 添加 GridLayoutGroup 自动排列卡牌
                var layout = handArea.GetComponent<GridLayoutGroup>();
                if (layout == null)
                {
                    layout = handArea.AddComponent<GridLayoutGroup>();
                }
                layout.cellSize = new Vector2(150, 200);
                layout.spacing = new Vector2(20, 20);
                layout.startAxis = GridLayoutGroup.Axis.Horizontal;
                layout.childAlignment = TextAnchor.MiddleCenter;
                
                Debug.Log("✅ Fixed HandArea");
            }
            
            EditorUtility.SetDirty(gamePanel);
            Debug.Log("✅ UI Layout fixed! Press Play to test.");
        }
#endif
    }
}
