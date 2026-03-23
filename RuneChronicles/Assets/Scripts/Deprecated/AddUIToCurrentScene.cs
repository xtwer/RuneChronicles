using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace RuneChronicles
{
    public class AddUIToCurrentScene
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Add UI to Current Scene")]
        public static void AddUI()
        {
            // 确保有 Canvas
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                var canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                canvasObj.AddComponent<GraphicRaycaster>();
                Debug.Log("✅ Created Canvas");
            }
            
            // 确保有 EventSystem
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                var eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                Debug.Log("✅ Created EventSystem");
            }
            
            // 确保有摄像机
            if (Camera.main == null)
            {
                var cameraObj = new GameObject("Main Camera");
                var camera = cameraObj.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
                camera.orthographic = true;
                cameraObj.tag = "MainCamera";
                Debug.Log("✅ Created Camera");
            }
            
            // 清理旧的测试 UI（如果存在）
            var oldBg = GameObject.Find("Background");
            if (oldBg != null) Object.DestroyImmediate(oldBg);
            
            var oldTitle = GameObject.Find("Title");
            if (oldTitle != null) Object.DestroyImmediate(oldTitle);
            
            var oldStatus = GameObject.Find("Status");
            if (oldStatus != null) Object.DestroyImmediate(oldStatus);
            
            var oldButtons = GameObject.Find("ButtonPanel");
            if (oldButtons != null) Object.DestroyImmediate(oldButtons);
            
            var oldHand = GameObject.Find("HandArea");
            if (oldHand != null) Object.DestroyImmediate(oldHand);
            
            // 创建背景
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvas.transform, false);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            // 创建标题
            var titleObj = CreateText(canvas.transform, "Title", "符文编年史 - 测试场景",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -30),
                new Vector2(800, 60), 40, Color.white);
            
            // 创建状态
            var statusObj = CreateText(canvas.transform, "Status", "生命: 20\n魔法: 10/10\n敌人生命: 20\n手牌: 0 张",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -140),
                new Vector2(500, 120), 26, Color.green);
            
            // 创建按钮
            var drawBtn = CreateButton(canvas.transform, "DrawButton", "抽卡",
                new Vector2(0.5f, 0f), new Vector2(-300, 50), new Vector2(200, 70));
            var fuseBtn = CreateButton(canvas.transform, "FusionButton", "融合",
                new Vector2(0.5f, 0f), new Vector2(0, 50), new Vector2(200, 70));
            var endBtn = CreateButton(canvas.transform, "EndTurnButton", "结束回合",
                new Vector2(0.5f, 0f), new Vector2(300, 50), new Vector2(200, 70));
            
            // 创建手牌区域
            var handAreaObj = new GameObject("HandArea");
            handAreaObj.transform.SetParent(canvas.transform, false);
            var handRect = handAreaObj.AddComponent<RectTransform>();
            handRect.anchorMin = new Vector2(0.5f, 0.5f);
            handRect.anchorMax = new Vector2(0.5f, 0.5f);
            handRect.pivot = new Vector2(0.5f, 0.5f);
            handRect.anchoredPosition = new Vector2(0, 50);
            handRect.sizeDelta = new Vector2(1400, 300);
            
            var gridLayout = handAreaObj.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(180, 260);
            gridLayout.spacing = new Vector2(20, 0);
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            
            var liveDisplay = handAreaObj.AddComponent<ChineseHandDisplay>();
            liveDisplay.handContainer = handAreaObj.transform;
            
            // 确保有管理器
            if (Object.FindObjectOfType<GameManager>() == null)
            {
                var gmObj = new GameObject("GameManager");
                var gm = gmObj.AddComponent<GameManager>();
                gm.currentEnemy = Resources.Load<EnemyData>("Enemies/Slime");
                Debug.Log("✅ Created GameManager");
            }
            
            if (Object.FindObjectOfType<CardManager>() == null)
            {
                var cmObj = new GameObject("CardManager");
                var cm = cmObj.AddComponent<CardManager>();
                cm.allCards.Add(Resources.Load<CardData>("Cards/Fire"));
                cm.allCards.Add(Resources.Load<CardData>("Cards/Ice"));
                Debug.Log("✅ Created CardManager");
            }
            
            if (Object.FindObjectOfType<FusionSystem>() == null)
            {
                var fsObj = new GameObject("FusionSystem");
                var fs = fsObj.AddComponent<FusionSystem>();
                fs.fusionRecipes.Add(Resources.Load<CardData>("Cards/Steam"));
                Debug.Log("✅ Created FusionSystem");
            }
            
            // 创建控制器
            var oldCtrl = Object.FindObjectOfType<ChineseTestController>();
            if (oldCtrl != null) Object.DestroyImmediate(oldCtrl.gameObject);
            
            var ctrlObj = new GameObject("ChineseTestController");
            var ctrl = ctrlObj.AddComponent<ChineseTestController>();
            ctrl.statusText = statusObj.GetComponent<Text>();
            ctrl.drawCardButton = drawBtn;
            ctrl.fusionButton = fuseBtn;
            ctrl.endTurnButton = endBtn;
            
            // 保存场景
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            
            Debug.Log("✅ UI 已添加到当前场景！现在可以播放测试了！");
        }
        
        private static Text CreateText(Transform parent, string name, string text,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size,
            int fontSize, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            
            var textComp = obj.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = fontSize;
            textComp.color = color;
            textComp.alignment = TextAnchor.MiddleCenter;
            
            return textComp;
        }
        
        private static Button CreateButton(Transform parent, string name, string text,
            Vector2 anchor, Vector2 pos, Vector2 size)
        {
            var btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            
            var btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = anchor;
            btnRect.anchorMax = anchor;
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = pos;
            btnRect.sizeDelta = size;
            
            var btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.5f, 0.9f, 1f);
            
            var button = btnObj.AddComponent<Button>();
            
            CreateText(btnObj.transform, "Text", text,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                28, Color.white);
            
            return button;
        }
#endif
    }
}
