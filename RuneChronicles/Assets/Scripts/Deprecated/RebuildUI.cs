using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace RuneChronicles
{
    /// <summary>
    /// 强制重建整个 UI 布局（删除旧的，创建新的）
    /// </summary>
    public class RebuildUI
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Rebuild UI Completely")]
        public static void Rebuild()
        {
            // 1. 删除旧的 UI 对象
            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                foreach (Transform child in canvas.transform)
                {
                    if (child.name == "Background" || child.name == "Title" || 
                        child.name == "Status" || child.name == "HandArea" ||
                        child.name.Contains("Button"))
                    {
                        Object.DestroyImmediate(child.gameObject);
                    }
                }
            }
            
            // 2. 创建背景
            var bg = new GameObject("Background");
            bg.transform.SetParent(canvas.transform, false);
            var bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bg.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f);
            
            // 3. 创建标题（大字体、黄色、往下移留边距）
            var title = CreateText("Title", "符文编年史 - 测试场景", canvas.transform);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -80);  // 标题再往下移
            titleRect.sizeDelta = new Vector2(1200, 80);
            var titleText = title.GetComponent<Text>();
            titleText.fontSize = 48;
            titleText.color = Color.yellow;
            titleText.fontStyle = FontStyle.Bold;
            titleText.horizontalOverflow = HorizontalWrapMode.Overflow;
            
            // 4. 创建状态文本（大字体、白色、标题下方，往下移）
            var status = CreateText("Status", "回合: 0\n生命: 20\n魔法: 10/10\n本回合可打牌: 1/1\n敌人生命: 30\n手牌: 0 张", canvas.transform);
            var statusRect = status.GetComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0.5f, 1f);
            statusRect.anchorMax = new Vector2(0.5f, 1f);
            statusRect.pivot = new Vector2(0.5f, 1f);
            statusRect.anchoredPosition = new Vector2(0, -200);  // 状态文本再往下移
            statusRect.sizeDelta = new Vector2(900, 200);
            var statusText = status.GetComponent<Text>();
            statusText.fontSize = 28;
            statusText.color = Color.white;
            statusText.alignment = TextAnchor.UpperCenter;
            
            // 5. 创建手牌区域（中间）
            var handArea = new GameObject("HandArea");
            handArea.transform.SetParent(canvas.transform, false);
            var handRect = handArea.AddComponent<RectTransform>();
            handRect.anchorMin = new Vector2(0.5f, 0.5f);
            handRect.anchorMax = new Vector2(0.5f, 0.5f);
            handRect.pivot = new Vector2(0.5f, 0.5f);
            handRect.anchoredPosition = new Vector2(0, -50);
            handRect.sizeDelta = new Vector2(1600, 300);
            
            var grid = handArea.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(200, 280);
            grid.spacing = new Vector2(30, 0);
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.MiddleCenter;
            
            var liveDisplay = handArea.AddComponent<ChineseHandDisplay>();
            liveDisplay.handContainer = handArea.transform;
            
            // 6. 创建按钮（往上移，离底部留更多边距）
            CreateButton("DrawButton", "抽卡", new Vector2(-350, 180), canvas.transform);  // 按钮再往上移
            CreateButton("FusionButton", "融合", new Vector2(0, 180), canvas.transform);
            CreateButton("EndTurnButton", "结束回合", new Vector2(350, 180), canvas.transform);
            
            // 7. 重新连接 Controller
            var controller = Object.FindObjectOfType<ChineseTestController>();
            if (controller != null)
            {
                controller.statusText = statusText;
                controller.drawCardButton = GameObject.Find("DrawButton").GetComponent<Button>();
                controller.fusionButton = GameObject.Find("FusionButton").GetComponent<Button>();
                controller.endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
                EditorUtility.SetDirty(controller);
            }
            
            // 保存
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            
            Debug.Log("✅ UI 已完全重建！请播放测试");
        }
        
        private static GameObject CreateText(string name, string content, Transform parent)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var text = obj.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.alignment = TextAnchor.MiddleCenter;
            return obj;
        }
        
        private static void CreateButton(string name, string label, Vector2 pos, Transform parent)
        {
            var btn = new GameObject(name);
            btn.transform.SetParent(parent, false);
            var btnRect = btn.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0f);
            btnRect.anchorMax = new Vector2(0.5f, 0f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = pos;
            btnRect.sizeDelta = new Vector2(300, 100);
            
            btn.AddComponent<Image>().color = new Color(0.2f, 0.5f, 0.9f);
            btn.AddComponent<Button>();
            
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(btn.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;
            
            var text = textObj.AddComponent<Text>();
            text.text = label;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 32;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
        }
#endif
    }
}
