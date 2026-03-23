using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;
#endif

namespace RuneChronicles
{
    /// <summary>
    /// 中文测试场景（使用标准 UI Text，支持中文）
    /// </summary>
    public class ChineseTestScene
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Create Chinese Test Scene")]
        public static void CreateChineseScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // 创建摄像机
            var cameraObj = new GameObject("Main Camera");
            var camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            camera.orthographic = true;
            cameraObj.tag = "MainCamera";
            
            // 创建 EventSystem
            var eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            
            // 创建 Canvas
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 创建背景
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasObj.transform, false);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            // 创建标题
            CreateText(canvasObj.transform, "Title", "符文编年史 - 测试场景", 
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -30), 
                new Vector2(800, 60), 40, Color.white, TextAnchor.MiddleCenter);
            
            // 创建状态显示
            var statusText = CreateText(canvasObj.transform, "Status", 
                "生命: 20\n魔法: 10/10\n敌人生命: 20\n手牌: 0 张", 
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -140), 
                new Vector2(500, 120), 26, Color.green, TextAnchor.MiddleCenter);
            
            // 创建按钮
            var buttonY = 50f;
            var drawBtn = CreateButton(canvasObj.transform, "DrawButton", "抽卡", 
                new Vector2(0.5f, 0f), new Vector2(-300, buttonY), new Vector2(200, 70));
            var fuseBtn = CreateButton(canvasObj.transform, "FusionButton", "融合", 
                new Vector2(0.5f, 0f), new Vector2(0, buttonY), new Vector2(200, 70));
            var endBtn = CreateButton(canvasObj.transform, "EndTurnButton", "结束回合", 
                new Vector2(0.5f, 0f), new Vector2(300, buttonY), new Vector2(200, 70));
            
            // 创建手牌区域
            var handAreaObj = new GameObject("HandArea");
            handAreaObj.transform.SetParent(canvasObj.transform, false);
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
            
            // 创建管理器
            var gmObj = new GameObject("GameManager");
            var gm = gmObj.AddComponent<GameManager>();
            gm.currentEnemy = Resources.Load<EnemyData>("Enemies/Slime");
            
            var cmObj = new GameObject("CardManager");
            var cm = cmObj.AddComponent<CardManager>();
            cm.allCards.Add(Resources.Load<CardData>("Cards/Fire"));
            cm.allCards.Add(Resources.Load<CardData>("Cards/Ice"));
            
            var fsObj = new GameObject("FusionSystem");
            var fs = fsObj.AddComponent<FusionSystem>();
            fs.fusionRecipes.Add(Resources.Load<CardData>("Cards/Steam"));
            
            // 创建控制器
            var ctrlObj = new GameObject("ChineseTestController");
            var ctrl = ctrlObj.AddComponent<ChineseTestController>();
            ctrl.statusText = statusText;
            ctrl.drawCardButton = drawBtn;
            ctrl.fusionButton = fuseBtn;
            ctrl.endTurnButton = endBtn;
            
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/ChineseTest.unity");
            Debug.Log("✅ 中文测试场景创建成功！按播放键测试。");
        }
        
        private static Text CreateText(Transform parent, string name, string text, 
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, 
            int fontSize, Color color, TextAnchor alignment)
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
            textComp.alignment = alignment;
            textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComp.verticalOverflow = VerticalWrapMode.Overflow;
            
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
                28, Color.white, TextAnchor.MiddleCenter);
            
            return button;
        }
#endif
    }
}
