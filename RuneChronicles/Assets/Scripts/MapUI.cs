using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 地图UI - 显示15层地图，可选择路径
/// </summary>
public class MapUI : MonoBehaviour
{
    private Canvas canvas;
    private List<Button> nodeButtons = new List<Button>();
    
    void Start()
    {
        CreateMapUI();
        UpdateMap();
    }
    
    void CreateMapUI()
    {
        // 创建Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("MapCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // 背景
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvas.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.2f);
        
        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(canvas.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        var titleText = titleObj.AddComponent<Text>();
        titleText.text = "选择你的路径";
        titleText.fontSize = 48;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        // 融合按钮（右上角）
        CreateFusionButton(canvas.transform);
        
        Debug.Log("[MapUI] 地图UI已创建");
    }
    
    void CreateFusionButton(Transform parent)
    {
        var btnObj = new GameObject("FusionButton");
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.85f, 0.85f);
        btnRect.anchorMax = new Vector2(0.95f, 0.9f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.6f, 0.2f, 0.8f);
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(OnOpenFusion);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var text = textObj.AddComponent<Text>();
        text.text = "融合";
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
    }
    
    void OnOpenFusion()
    {
        Debug.Log("[MapUI] 打开融合界面");
        
        // 销毁地图UI
        Destroy(gameObject);
        
        // 创建融合UI
        var fusionUIObj = new GameObject("FusionUI");
        fusionUIObj.AddComponent<FusionUI>();
    }
    
    void UpdateMap()
    {
        if (MapManager.Instance == null) return;
        
        // 清除旧节点
        foreach (var btn in nodeButtons)
        {
            if (btn != null) Destroy(btn.gameObject);
        }
        nodeButtons.Clear();
        
        // 获取当前层的节点
        var currentFloor = MapManager.Instance.currentFloor;
        var nodes = MapManager.Instance.GetCurrentFloorNodes();
        
        // 显示当前层节点
        for (int i = 0; i < nodes.Count; i++)
        {
            CreateNodeButton(nodes[i], i, nodes.Count);
        }
    }
    
    void CreateNodeButton(MapNode node, int index, int total)
    {
        var btnObj = new GameObject($"Node_{node.nodeType}");
        btnObj.transform.SetParent(canvas.transform, false);
        
        // 水平排列
        float spacing = 300f;
        float startX = -(total - 1) * spacing / 2f;
        float x = startX + index * spacing;
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = new Vector2(x, 0);
        btnRect.sizeDelta = new Vector2(200, 200);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = GetNodeColor(node.nodeType);
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnNodeClick(node));
        
        // 节点类型文本
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var text = textObj.AddComponent<Text>();
        text.text = GetNodeText(node.nodeType);
        text.fontSize = 32;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        
        nodeButtons.Add(button);
    }
    
    Color GetNodeColor(MapNodeType nodeType)
    {
        switch (nodeType)
        {
            case MapNodeType.Battle: return new Color(0.3f, 0.5f, 0.3f);
            case MapNodeType.Elite: return new Color(0.7f, 0.3f, 0.3f);
            case MapNodeType.Boss: return new Color(0.9f, 0.1f, 0.1f);
            case MapNodeType.Shop: return new Color(0.3f, 0.6f, 0.8f);
            case MapNodeType.Treasure: return new Color(0.8f, 0.6f, 0.2f);
            default: return Color.gray;
        }
    }
    
    string GetNodeText(MapNodeType nodeType)
    {
        switch (nodeType)
        {
            case MapNodeType.Battle: return "战斗";
            case MapNodeType.Elite: return "精英";
            case MapNodeType.Boss: return "BOSS";
            case MapNodeType.Shop: return "商店";
            case MapNodeType.Treasure: return "宝箱";
            default: return "???";
        }
    }
    
    void OnNodeClick(MapNode node)
    {
        Debug.Log($"[MapUI] 选择节点: {node.nodeType}");
        
        // 进入节点
        if (MapManager.Instance != null)
        {
            MapManager.Instance.EnterNode(node);
        }
        
        // 根据节点类型跳转
        switch (node.nodeType)
        {
            case MapNodeType.Battle:
            case MapNodeType.Elite:
                StartBattle(node.nodeType == MapNodeType.Elite);
                break;
            
            case MapNodeType.Boss:
                StartBossBattle();
                break;
            
            case MapNodeType.Shop:
                OpenShop();
                break;
            
            case MapNodeType.Treasure:
                OpenTreasure();
                break;
        }
        
        // 销毁地图UI
        Destroy(gameObject);
    }
    
    void StartBattle(bool isElite)
    {
        Debug.Log($"[MapUI] 开始{(isElite ? "精英" : "普通")}战斗");
        
        // 创建敌人
        var enemy = new GameObject("Enemy").AddComponent<Enemy>();
        enemy.enemyName = isElite ? "精英怪物" : "普通怪物";
        enemy.maxHP = isElite ? 60 : 40;
        enemy.currentHP = enemy.maxHP;
        enemy.minDamage = isElite ? 8 : 5;
        enemy.maxDamage = isElite ? 12 : 7;
        enemy.behaviorPattern = EnemyBehaviorPattern.Random;
        
        var enemies = new List<Enemy> { enemy };
        
        // 开始战斗
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.StartBattle(CardManager.Instance.playerDeck, enemies);
        }
        
        // 创建战斗UI
        var battleUIObj = new GameObject("BattleUI");
        battleUIObj.AddComponent<BattleUI>();
    }
    
    void StartBossBattle()
    {
        Debug.Log("[MapUI] 开始BOSS战");
        
        // 创建BOSS
        var boss = new GameObject("Boss").AddComponent<Enemy>();
        boss.enemyName = "BOSS";
        boss.maxHP = 200;
        boss.currentHP = 200;
        boss.minDamage = 15;
        boss.maxDamage = 25;
        boss.behaviorPattern = EnemyBehaviorPattern.Random;
        
        var enemies = new List<Enemy> { boss };
        
        // 开始战斗
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.StartBattle(CardManager.Instance.playerDeck, enemies);
        }
        
        // 创建战斗UI
        var battleUIObj = new GameObject("BattleUI");
        battleUIObj.AddComponent<BattleUI>();
    }
    
    void OpenShop()
    {
        Debug.Log("[MapUI] 打开商店");
        
        // 创建商店UI
        var shopUIObj = new GameObject("ShopUI");
        shopUIObj.AddComponent<ShopUI>();
    }
    
    void OpenTreasure()
    {
        Debug.Log("[MapUI] 打开宝箱");
        
        // 获得奖励
        if (GameManager.Instance != null)
        {
            int gold = Random.Range(40, 80);
            GameManager.Instance.AddGold(gold);
            Debug.Log($"获得 {gold} 金币！");
        }
        
        // 继续前进
        MapManager.Instance.MoveToNextFloor(0);
        
        // 重新显示地图
        var mapUIObj = new GameObject("MapUI");
        mapUIObj.AddComponent<MapUI>();
    }
}
