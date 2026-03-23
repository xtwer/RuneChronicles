using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    public class CanvasPositionFixer
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Reset Canvas Position")]
        public static void ResetCanvasPosition()
        {
            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                var rect = canvas.GetComponent<RectTransform>();
                
                // 重置位置
                rect.anchoredPosition = Vector2.zero;
                rect.localPosition = Vector3.zero;
                
                // 确保锚点和 Pivot 正确
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                
                // 重置偏移
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                
                Debug.Log($"✅ Canvas position reset!");
                Debug.Log($"Position: {rect.anchoredPosition}");
                Debug.Log($"LocalPosition: {rect.localPosition}");
                
                EditorUtility.SetDirty(canvas.gameObject);
            }
            else
            {
                Debug.LogError("Canvas not found!");
            }
        }
#endif
    }
}
