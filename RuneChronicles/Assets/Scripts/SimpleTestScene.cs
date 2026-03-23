using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;
#endif

namespace RuneChronicles
{
    /// <summary>
    /// 超级简单的测试场景 - 保证能看到 UI
    /// </summary>
    public class SimpleTestScene
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Create Super Simple Scene")]
        public static void CreateSuperSimpleScene()
        {
            // 创建新场景
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
            
            // 创建背景面板
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasObj.transform, false);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // 创建大标题
            var titleObj = new GameObject("Title");
            titleObj.transform.SetParent(canvasObj.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -20);
            titleRect.sizeDelta = new Vector2(800, 80);
            var titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Rune Chronicles - Test Scene";
            titleText.fontSize = 48;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            
            // 创建状态文本
            var statusObj = new GameObject("Status");
            statusObj.transform.SetParent(canvasObj.transform, false);
            var statusRect = statusObj.AddComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0.5f, 1f);
            statusRect.anchorMax = new Vector2(0.5f, 1f);
            statusRect.pivot = new Vector2(0.5f, 1f);
            statusRect.anchoredPosition = new Vector2(0, -120);
            statusRect.sizeDelta = new Vector2(600, 150);
            var statusText = statusObj.AddComponent<TextMeshProUGUI>();
            statusText.text = "HP: 20 (生命)\nMana: 10/10 (魔法)\nEnemy HP: 20 (敌人)\nHand: 0 cards (手牌)";
            statusText.fontSize = 30;
            statusText.alignment = TextAlignmentOptions.Center;
            statusText.color = Color.green;
            
            // 创建按钮容器
            var buttonPanel = new GameObject("ButtonPanel");
            buttonPanel.transform.SetParent(canvasObj.transform, false);
            var buttonPanelRect = buttonPanel.AddComponent<RectTransform>();
            buttonPanelRect.anchorMin = new Vector2(0.5f, 0f);
            buttonPanelRect.anchorMax = new Vector2(0.5f, 0f);
            buttonPanelRect.pivot = new Vector2(0.5f, 0f);
            buttonPanelRect.anchoredPosition = new Vector2(0, 20);
            buttonPanelRect.sizeDelta = new Vector2(900, 100);
            
            // 创建 3 个按钮
            CreateBigButton(buttonPanel.transform, "DrawButton", "Draw Card (抽卡)", new Vector2(-300, 0));
            CreateBigButton(buttonPanel.transform, "FusionButton", "Fuse Cards (融合)", new Vector2(0, 0));
            CreateBigButton(buttonPanel.transform, "EndTurnButton", "End Turn (结束回合)", new Vector2(300, 0));
            
            // 创建手牌显示区域
            var handAreaObj = new GameObject("HandArea");
            handAreaObj.transform.SetParent(canvasObj.transform, false);
            var handRect = handAreaObj.AddComponent<RectTransform>();
            handRect.anchorMin = new Vector2(0.5f, 0.5f);
            handRect.anchorMax = new Vector2(0.5f, 0.5f);
            handRect.pivot = new Vector2(0.5f, 0.5f);
            handRect.anchoredPosition = new Vector2(0, 0);
            handRect.sizeDelta = new Vector2(1400, 280);
            
            // 添加网格布局
            var gridLayout = handAreaObj.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(180, 240);
            gridLayout.spacing = new Vector2(20, 0);
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            
            // 添加实时手牌显示
            var liveDisplay = handAreaObj.AddComponent<LiveHandDisplay>();
            liveDisplay.handContainer = handAreaObj.transform;
            
            // 创建管理器
            var gmObj = new GameObject("GameManager");
            var gm = gmObj.AddComponent<GameManager>();
            // 注意：新版GameManager不再有currentEnemy属性
            // gm.currentEnemy = Resources.Load<EnemyData>("Enemies/Slime");
            
            var cmObj = new GameObject("CardManager");
            var cm = cmObj.AddComponent<CardManager>();
            // 注意：新版CardManager从JSON自动加载，不需要手动添加
            // cm.allCards.Add(Resources.Load<CardData>("Cards/Fire"));
            // cm.allCards.Add(Resources.Load<CardData>("Cards/Ice"));
            
            var fsObj = new GameObject("FusionSystem");
            var fs = fsObj.AddComponent<FusionSystem>();
            fs.fusionRecipes.Add(Resources.Load<CardData>("Cards/Steam"));
            
            // 创建控制器
            var ctrlObj = new GameObject("TestController");
            var ctrl = ctrlObj.AddComponent<TestSceneController>();
            ctrl.playerHealthText = statusText;
            ctrl.enemyHealthText = statusText;
            ctrl.manaText = statusText;
            ctrl.drawCardButton = GameObject.Find("DrawButton").GetComponent<Button>();
            ctrl.fusionButton = GameObject.Find("FusionButton").GetComponent<Button>();
            ctrl.endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
            
            // 保存场景
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/SimpleTest.unity");
            
            Debug.Log("✅ Super simple scene created! Press Play now!");
        }
        
        private static void CreateBigButton(Transform parent, string name, string text, Vector2 pos)
        {
            var btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            
            var btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.sizeDelta = new Vector2(250, 80);
            btnRect.anchoredPosition = pos;
            
            var btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.3f, 0.6f, 0.9f, 1f);
            
            var button = btnObj.AddComponent<Button>();
            
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 28;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }
#endif
    }
}
