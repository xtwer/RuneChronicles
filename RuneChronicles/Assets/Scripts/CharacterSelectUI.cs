using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 角色选择UI
/// </summary>
public class CharacterSelectUI : MonoBehaviour
{
    void Start()
    {
        CreateCharacterSelect();
    }
    
    void CreateCharacterSelect()
    {
        // 创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("CharacterSelectCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
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
        bgImage.color = new Color(0.1f, 0.1f, 0.15f);
        
        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(canvas.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.85f);
        titleRect.anchorMax = new Vector2(0.5f, 0.95f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(800, 100);
        
        var titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "选择你的角色";
        titleText.fontSize = 56;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        // 法师卡片
        CreateCharacterCard(canvas.transform, CharacterClass.Mage, 
            "符文法师", 
            "生命: 80\n能量: 3\n风格: 魔法输出",
            new Vector2(-300, 0));
        
        // 战士卡片
        CreateCharacterCard(canvas.transform, CharacterClass.Warrior,
            "符文战士",
            "生命: 100\n能量: 3\n风格: 反击防御",
            new Vector2(300, 0));
        
        // 返回按钮
        CreateButton(canvas.transform, "返回", new Vector2(0, -400), OnBack);
        
        Debug.Log("[CharacterSelectUI] 角色选择界面已创建");
    }
    
    void CreateCharacterCard(Transform parent, CharacterClass charClass, string name, string desc, Vector2 pos)
    {
        var cardObj = new GameObject($"CharacterCard_{charClass}");
        cardObj.transform.SetParent(parent, false);
        
        var cardRect = cardObj.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = pos;
        cardRect.sizeDelta = new Vector2(400, 500);
        
        var cardImage = cardObj.AddComponent<Image>();
        cardImage.color = new Color(0.2f, 0.2f, 0.3f, 0.9f);
        
        var button = cardObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnSelectCharacter(charClass));
        
        // 角色名称
        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform, false);
        var nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.8f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        var nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = name;
        nameText.fontSize = 42;
        nameText.fontStyle = FontStyles.Bold;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = new Color(1f, 0.8f, 0.3f);
        
        // 描述
        var descObj = new GameObject("Description");
        descObj.transform.SetParent(cardObj.transform, false);
        var descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.1f, 0.1f);
        descRect.anchorMax = new Vector2(0.9f, 0.7f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;
        
        var descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = desc;
        descText.fontSize = 28;
        descText.alignment = TextAlignmentOptions.Center;
        descText.color = Color.white;
    }
    
    void CreateButton(Transform parent, string text, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        var btnObj = new GameObject($"Button_{text}");
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = pos;
        btnRect.sizeDelta = new Vector2(300, 70);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.5f, 0.5f, 0.5f);
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(action);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }
    
    void OnSelectCharacter(CharacterClass charClass)
    {
        Debug.Log($"[CharacterSelectUI] 选择角色: {charClass}");
        
        // 开始新游戏
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewRun(charClass);
        }
        
        // 销毁角色选择UI
        Destroy(gameObject);
        
        // 创建战斗UI
        var battleUIObj = new GameObject("BattleUI");
        battleUIObj.AddComponent<BattleUI>();
        
        // 开始第一场战斗（测试）
        StartFirstBattle();
    }
    
    void StartFirstBattle()
    {
        // 创建测试敌人
        var enemy1 = new GameObject("Enemy1").AddComponent<Enemy>();
        enemy1.enemyId = "ENM_001";
        enemy1.enemyName = "符文傀儡";
        enemy1.maxHP = 40;
        enemy1.currentHP = 40;
        enemy1.minDamage = 5;
        enemy1.maxDamage = 7;
        enemy1.behaviorPattern = EnemyBehaviorPattern.AttackDefend;
        
        var enemies = new System.Collections.Generic.List<Enemy> { enemy1 };
        var playerDeck = CardManager.Instance.playerDeck;
        
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.StartBattle(playerDeck, enemies);
        }
    }
    
    void OnBack()
    {
        Debug.Log("[CharacterSelectUI] 返回主菜单");
        
        // 销毁角色选择UI
        Destroy(gameObject);
        
        // 重新创建主菜单
        var menuObj = new GameObject("MainMenuUI");
        menuObj.AddComponent<MainMenuUI>();
    }
}
