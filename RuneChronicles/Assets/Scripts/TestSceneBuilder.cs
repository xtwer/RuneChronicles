using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.EventSystems;
#endif

namespace RuneChronicles
{
    /// <summary>
    /// 自动创建测试场景
    /// </summary>
    public class TestSceneBuilder
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Build Test Scene")]
        public static void BuildTestScene()
        {
            // 创建 Canvas
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 创建 EventSystem
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                var eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
            
            // 创建管理器
            CreateManagers();
            
            // 创建 UI 面板
            var panelObj = CreatePanel(canvasObj.transform, "GamePanel", new Vector2(0, 0), new Vector2(1, 1));
            
            // 创建顶部状态栏
            var topBarObj = CreatePanel(panelObj.transform, "TopBar", new Vector2(0.5f, 1f), new Vector2(800, 100));
            topBarObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
            
            var playerHealthText = CreateText(topBarObj.transform, "PlayerHealth", "HP: 20", new Vector2(0.2f, 0.5f));
            var manaText = CreateText(topBarObj.transform, "Mana", "Mana: 10/10", new Vector2(0.5f, 0.5f));
            var enemyHealthText = CreateText(topBarObj.transform, "EnemyHealth", "Enemy HP: 20", new Vector2(0.8f, 0.5f));
            
            // 创建按钮栏
            var buttonBarObj = CreatePanel(panelObj.transform, "ButtonBar", new Vector2(0.5f, 0f), new Vector2(600, 80));
            buttonBarObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 40);
            
            CreateButton(buttonBarObj.transform, "DrawButton", "Draw Card", new Vector2(-200, 0));
            CreateButton(buttonBarObj.transform, "FusionButton", "Fuse Cards", new Vector2(0, 0));
            CreateButton(buttonBarObj.transform, "EndTurnButton", "End Turn", new Vector2(200, 0));
            
            // 创建手牌区域
            var handAreaObj = CreatePanel(panelObj.transform, "HandArea", new Vector2(0.5f, 0.3f), new Vector2(900, 300));
            var handDisplay = handAreaObj.AddComponent<SimpleHandDisplay>();
            
            // 创建卡牌预制体
            var cardPrefab = CreateCardPrefab();
            handDisplay.cardPrefab = cardPrefab;
            handDisplay.handContainer = handAreaObj.transform;
            
            // 创建场景控制器
            var controllerObj = new GameObject("TestSceneController");
            var controller = controllerObj.AddComponent<TestSceneController>();
            controller.playerHealthText = playerHealthText;
            controller.enemyHealthText = enemyHealthText;
            controller.manaText = manaText;
            controller.drawCardButton = GameObject.Find("DrawButton").GetComponent<Button>();
            controller.fusionButton = GameObject.Find("FusionButton").GetComponent<Button>();
            controller.endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
            
            Debug.Log("✅ Test scene built successfully!");
        }
        
        private static void CreateManagers()
        {
            if (Object.FindObjectOfType<GameManager>() == null)
            {
                var obj = new GameObject("GameManager");
                var gm = obj.AddComponent<GameManager>();
                gm.currentEnemy = Resources.Load<EnemyData>("Enemies/Slime");
            }
            
            if (Object.FindObjectOfType<CardManager>() == null)
            {
                var obj = new GameObject("CardManager");
                var cm = obj.AddComponent<CardManager>();
                cm.allCards.Add(Resources.Load<CardData>("Cards/Fire"));
                cm.allCards.Add(Resources.Load<CardData>("Cards/Ice"));
            }
            
            if (Object.FindObjectOfType<FusionSystem>() == null)
            {
                var obj = new GameObject("FusionSystem");
                var fs = obj.AddComponent<FusionSystem>();
                fs.fusionRecipes.Add(Resources.Load<CardData>("Cards/Steam"));
            }
        }
        
        private static GameObject CreatePanel(Transform parent, string name, Vector2 anchor, Vector2 size)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            
            var image = obj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            return obj;
        }
        
        private static TextMeshProUGUI CreateText(Transform parent, string name, string text, Vector2 anchor)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(200, 50);
            rect.anchoredPosition = Vector2.zero;
            
            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            
            return tmp;
        }
        
        private static Button CreateButton(Transform parent, string name, string text, Vector2 position)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(150, 50);
            rect.anchoredPosition = position;
            
            var image = obj.AddComponent<Image>();
            image.color = new Color(0.4f, 0.6f, 0.8f);
            
            var button = obj.AddComponent<Button>();
            
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 20;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            
            return button;
        }
        
        private static GameObject CreateCardPrefab()
        {
            var obj = new GameObject("CardPrefab");
            
            var rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 200);
            
            var image = obj.AddComponent<Image>();
            image.color = Color.white;
            
            var button = obj.AddComponent<Button>();
            
            var cardUI = obj.AddComponent<SimpleCardUI>();
            cardUI.cardButton = button;
            cardUI.background = image;
            
            // 创建文本
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(obj.transform);
            var nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.8f);
            nameRect.anchorMax = new Vector2(1, 1f);
            nameRect.sizeDelta = Vector2.zero;
            var nameTmp = nameObj.AddComponent<TextMeshProUGUI>();
            nameTmp.fontSize = 18;
            nameTmp.alignment = TextAlignmentOptions.Center;
            nameTmp.color = Color.black;
            cardUI.nameText = nameTmp;
            
            var atkObj = new GameObject("Attack");
            atkObj.transform.SetParent(obj.transform);
            var atkRect = atkObj.AddComponent<RectTransform>();
            atkRect.anchorMin = new Vector2(0, 0.4f);
            atkRect.anchorMax = new Vector2(1, 0.6f);
            atkRect.sizeDelta = Vector2.zero;
            var atkTmp = atkObj.AddComponent<TextMeshProUGUI>();
            atkTmp.fontSize = 16;
            atkTmp.alignment = TextAlignmentOptions.Center;
            atkTmp.color = Color.black;
            cardUI.attackText = atkTmp;
            
            var manaObj = new GameObject("Mana");
            manaObj.transform.SetParent(obj.transform);
            var manaRect = manaObj.AddComponent<RectTransform>();
            manaRect.anchorMin = new Vector2(0, 0.2f);
            manaRect.anchorMax = new Vector2(1, 0.4f);
            manaRect.sizeDelta = Vector2.zero;
            var manaTmp = manaObj.AddComponent<TextMeshProUGUI>();
            manaTmp.fontSize = 14;
            manaTmp.alignment = TextAlignmentOptions.Center;
            manaTmp.color = Color.black;
            cardUI.manaText = manaTmp;
            
            // 保存为预制体
            string path = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            
            PrefabUtility.SaveAsPrefabAsset(obj, $"{path}/CardPrefab.prefab");
            Object.DestroyImmediate(obj);
            
            return AssetDatabase.LoadAssetAtPath<GameObject>($"{path}/CardPrefab.prefab");
        }
#endif
    }
}
