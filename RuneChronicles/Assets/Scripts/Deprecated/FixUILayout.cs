using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    public class FixUILayout
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Fix UI Layout Now")]
        public static void FixLayout()
        {
            // 修复标题
            var title = GameObject.Find("Title");
            if (title != null)
            {
                var titleText = title.GetComponent<Text>();
                if (titleText != null)
                {
                    titleText.fontSize = 36;
                    titleText.color = Color.yellow;
                }
                
                var titleRect = title.GetComponent<RectTransform>();
                if (titleRect != null)
                {
                    titleRect.anchorMin = new Vector2(0.5f, 1f);
                    titleRect.anchorMax = new Vector2(0.5f, 1f);
                    titleRect.pivot = new Vector2(0.5f, 1f);
                    titleRect.anchoredPosition = new Vector2(0, -10);
                    titleRect.sizeDelta = new Vector2(900, 50);
                }
                Debug.Log("✅ 修复了标题");
            }
            
            // 修复状态文本
            var status = GameObject.Find("Status");
            if (status != null)
            {
                var statusText = status.GetComponent<Text>();
                if (statusText != null)
                {
                    statusText.fontSize = 22;
                    statusText.color = Color.white;
                    statusText.alignment = TextAnchor.UpperCenter;
                }
                
                var statusRect = status.GetComponent<RectTransform>();
                if (statusRect != null)
                {
                    statusRect.anchorMin = new Vector2(0.5f, 1f);
                    statusRect.anchorMax = new Vector2(0.5f, 1f);
                    statusRect.pivot = new Vector2(0.5f, 1f);
                    statusRect.anchoredPosition = new Vector2(0, -70);
                    statusRect.sizeDelta = new Vector2(800, 180);
                }
                Debug.Log("✅ 修复了状态文本");
            }
            
            // 修复手牌区域
            var handArea = GameObject.Find("HandArea");
            if (handArea != null)
            {
                var handRect = handArea.GetComponent<RectTransform>();
                if (handRect != null)
                {
                    handRect.anchorMin = new Vector2(0.5f, 0.5f);
                    handRect.anchorMax = new Vector2(0.5f, 0.5f);
                    handRect.pivot = new Vector2(0.5f, 0.5f);
                    handRect.anchoredPosition = new Vector2(0, -50);
                    handRect.sizeDelta = new Vector2(1400, 280);
                }
                Debug.Log("✅ 修复了手牌区域");
            }
            
            // 修复按钮
            string[] buttonNames = { "DrawButton", "FusionButton", "EndTurnButton" };
            float[] buttonX = { -300, 0, 300 };
            
            for (int i = 0; i < buttonNames.Length; i++)
            {
                var button = GameObject.Find(buttonNames[i]);
                if (button != null)
                {
                    var btnRect = button.GetComponent<RectTransform>();
                    if (btnRect != null)
                    {
                        btnRect.anchorMin = new Vector2(0.5f, 0f);
                        btnRect.anchorMax = new Vector2(0.5f, 0f);
                        btnRect.pivot = new Vector2(0.5f, 0f);
                        btnRect.anchoredPosition = new Vector2(buttonX[i], 30);
                        btnRect.sizeDelta = new Vector2(220, 70);
                    }
                    
                    var btnText = button.GetComponentInChildren<Text>();
                    if (btnText != null)
                    {
                        btnText.fontSize = 26;
                    }
                }
            }
            Debug.Log("✅ 修复了按钮位置");
            
            // 保存场景
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            
            Debug.Log("✅ UI 布局已修复！请保存场景并重新播放测试");
        }
#endif
    }
}
