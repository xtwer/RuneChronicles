using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// 完整的战斗UI系统
/// </summary>
public class BattleUI : MonoBehaviour
{
    [Header("UI面板")]
    public GameObject handPanel;
    public GameObject enemyPanel;
    public GameObject playerPanel;
    public GameObject buttonPanel;
    
    [Header("玩家信息")]
    public Text playerHPText;
    public Text playerBlockText;
    public Slider playerHPBar;
    
    [Header("能量信息")]
    public Text energyText;
    
    [Header("回合信息")]
    public Text turnText;
    public Button endTurnButton;
    
    [Header("卡牌预制体")]
    public GameObject cardPrefab;
    
    [Header("敌人预制体")]
    public GameObject enemyPrefab;
    
    private List<GameObject> handCardObjects = new List<GameObject>();
    private List<GameObject> enemyObjects = new List<GameObject>();
    private List<Text> enemyHPTexts = new List<Text>();
    private List<Slider> enemyHPBars = new List<Slider>();
    public Enemy targetEnemy = null;
    private Text combatLogText;
    
    void Start()
    {
        CreateBattleUI();
        
        // 订阅事件
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPlayerTurnStart += OnPlayerTurnStart;
            BattleManager.Instance.OnEnergyChanged += UpdateEnergy;
            BattleManager.Instance.OnCardDrawn += OnCardDrawn;
            BattleManager.Instance.OnCardPlayed += OnCardPlayed;
            BattleManager.Instance.OnBattleEnd += OnBattleEnd;
            BattleManager.Instance.OnEnemyTurnEnd += OnEnemyTurnEnd;
        }

        if (Player.Instance != null)
        {
            Player.Instance.OnHPChanged += UpdatePlayerHP;
            Player.Instance.OnGainBlock += UpdatePlayerBlock;
            Player.Instance.OnTakeDamage += OnPlayerTakeDamage;
        }

        // 事件可能在 Start() 之前已触发（第一回合），手动同步一次
        RefreshHand();
        UpdateUI();
    }

    void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPlayerTurnStart -= OnPlayerTurnStart;
            BattleManager.Instance.OnEnergyChanged -= UpdateEnergy;
            BattleManager.Instance.OnCardDrawn -= OnCardDrawn;
            BattleManager.Instance.OnCardPlayed -= OnCardPlayed;
            BattleManager.Instance.OnBattleEnd -= OnBattleEnd;
            BattleManager.Instance.OnEnemyTurnEnd -= OnEnemyTurnEnd;
        }

        if (Player.Instance != null)
        {
            Player.Instance.OnHPChanged -= UpdatePlayerHP;
            Player.Instance.OnGainBlock -= UpdatePlayerBlock;
            Player.Instance.OnTakeDamage -= OnPlayerTakeDamage;
        }
    }

    void CreateBattleUI()
    {
        Debug.Log("[BattleUI] 创建战斗UI...");
        
        // 创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("BattleCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 清除旧UI内容
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(canvas.transform.GetChild(i).gameObject);

        // 战斗背景
        var bgObj = new GameObject("BattleBG");
        bgObj.transform.SetParent(canvas.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgObj.AddComponent<Image>().color = new Color(0.12f, 0.15f, 0.2f);

        // 玩家面板（底部）
        CreatePlayerPanel(canvas.transform);

        // 敌人面板（顶部）
        CreateEnemyPanel(canvas.transform);

        // 填充敌人UI
        PopulateEnemyPanel();

        // 手牌面板（底部中间）
        CreateHandPanel(canvas.transform);

        // 按钮面板（右下）
        CreateButtonPanel(canvas.transform);

        // 初始化UI数值
        UpdateUI();

        // 播放战斗BGM
        if (AudioManager.Instance != null)
        {
            var bgmClip = Resources.Load<AudioClip>("Audio/BGM/battle");
            if (bgmClip != null) AudioManager.Instance.PlayBGM(bgmClip);
        }

        Debug.Log("[BattleUI] 战斗UI创建完成");
    }
    
    void CreatePlayerPanel(Transform parent)
    {
        // 玩家面板：左侧中部，不与手牌区重叠
        playerPanel = new GameObject("PlayerPanel");
        playerPanel.transform.SetParent(parent, false);

        var rect = playerPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.01f, 0.43f);
        rect.anchorMax = new Vector2(0.22f, 0.72f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        playerPanel.AddComponent<Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.92f);

        // 生命值
        CreatePlayerStat(playerPanel.transform, "HP", new Vector2(0, 0.55f), out playerHPText, out playerHPBar);

        // 护盾
        CreatePlayerStat(playerPanel.transform, "Block", new Vector2(0, 0.25f), out playerBlockText, out _);

        // 战斗日志：玩家面板下方
        var logObj = new GameObject("CombatLog");
        logObj.transform.SetParent(parent, false);
        var logRect = logObj.AddComponent<RectTransform>();
        logRect.anchorMin = new Vector2(0.01f, 0.30f);
        logRect.anchorMax = new Vector2(0.22f, 0.42f);
        logRect.offsetMin = Vector2.zero;
        logRect.offsetMax = Vector2.zero;
        logObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.45f);

        var logTextObj = new GameObject("LogText");
        logTextObj.transform.SetParent(logObj.transform, false);
        var logTextRect = logTextObj.AddComponent<RectTransform>();
        logTextRect.anchorMin = Vector2.zero;
        logTextRect.anchorMax = Vector2.one;
        logTextRect.offsetMin = new Vector2(6, 4);
        logTextRect.offsetMax = new Vector2(-6, -4);
        combatLogText = logTextObj.AddComponent<Text>();
        combatLogText.text = "";
        combatLogText.fontSize = 18;
        combatLogText.alignment = TextAnchor.MiddleCenter;
        combatLogText.color = new Color(1f, 0.85f, 0.6f);
        combatLogText.font = ChineseUI.GetChineseFont();
        combatLogText.horizontalOverflow = HorizontalWrapMode.Wrap;
    }
    
    void CreatePlayerStat(Transform parent, string label, Vector2 yPos, out Text text, out Slider slider)
    {
        var statObj = new GameObject(label);
        statObj.transform.SetParent(parent, false);
        
        var rect = statObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, yPos.y);
        rect.anchorMax = new Vector2(0.9f, yPos.y + 0.15f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        text = statObj.AddComponent<Text>();
        text.text = $"{label}: 0";
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.white;
        text.font = ChineseUI.GetChineseFont();
        
        // 血条（只给HP）
        if (label == "HP")
        {
            var sliderObj = new GameObject("HPBar");
            sliderObj.transform.SetParent(parent, false);
            
            var sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.1f, 0.45f);
            sliderRect.anchorMax = new Vector2(0.9f, 0.5f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            
            slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 1;
            
            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = Color.red;
            
            // Fill
            var fillAreaObj = new GameObject("Fill Area");
            fillAreaObj.transform.SetParent(sliderObj.transform, false);
            var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;
            
            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = Color.green;
            
            slider.fillRect = fillRect;
        }
        else
        {
            slider = null;
        }
    }
    
    void CreateEnemyPanel(Transform parent)
    {
        // 敌人面板：顶部居中，留出足够高度
        enemyPanel = new GameObject("EnemyPanel");
        enemyPanel.transform.SetParent(parent, false);

        var rect = enemyPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.25f, 0.55f);
        rect.anchorMax = new Vector2(0.75f, 0.98f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var layout = enemyPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 30;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
    }
    
    void PopulateEnemyPanel()
    {
        if (BattleManager.Instance == null || enemyPanel == null) return;

        enemyObjects.Clear();
        enemyHPTexts.Clear();
        enemyHPBars.Clear();

        var enemies = BattleManager.Instance.GetAliveEnemies();
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            var cardObj = new GameObject($"Enemy_{enemy.enemyName}");
            cardObj.transform.SetParent(enemyPanel.transform, false);

            var rect = cardObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 250);

            var bg = cardObj.AddComponent<Image>();
            bg.color = new Color(0.4f, 0.15f, 0.15f, 0.9f);

            // 敌人名称
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(cardObj.transform, false);
            var nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.8f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;
            var nameText = nameObj.AddComponent<Text>();
            nameText.text = enemy.enemyName;
            nameText.fontSize = 22;
            nameText.fontStyle = FontStyle.Bold;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = new Color(1f, 0.7f, 0.7f);
            nameText.font = ChineseUI.GetChineseFont();

            // HP文本
            var hpObj = new GameObject("HP");
            hpObj.transform.SetParent(cardObj.transform, false);
            var hpRect = hpObj.AddComponent<RectTransform>();
            hpRect.anchorMin = new Vector2(0, 0.6f);
            hpRect.anchorMax = new Vector2(1, 0.78f);
            hpRect.offsetMin = Vector2.zero;
            hpRect.offsetMax = Vector2.zero;
            var hpText = hpObj.AddComponent<Text>();
            hpText.text = $"HP: {enemy.currentHP}/{enemy.maxHP}";
            hpText.fontSize = 20;
            hpText.alignment = TextAnchor.MiddleCenter;
            hpText.color = Color.white;
            hpText.font = ChineseUI.GetChineseFont();
            enemyHPTexts.Add(hpText);

            // HP条背景
            var barBgObj = new GameObject("HPBarBG");
            barBgObj.transform.SetParent(cardObj.transform, false);
            var barBgRect = barBgObj.AddComponent<RectTransform>();
            barBgRect.anchorMin = new Vector2(0.05f, 0.5f);
            barBgRect.anchorMax = new Vector2(0.95f, 0.58f);
            barBgRect.offsetMin = Vector2.zero;
            barBgRect.offsetMax = Vector2.zero;
            barBgObj.AddComponent<Image>().color = Color.red;

            // HP条填充区
            var fillAreaObj = new GameObject("FillArea");
            fillAreaObj.transform.SetParent(barBgObj.transform, false);
            var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;

            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            fillObj.AddComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f);

            var slider = barBgObj.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.minValue = 0;
            slider.maxValue = enemy.maxHP;
            slider.value = enemy.currentHP;
            enemyHPBars.Add(slider);

            // 攻击意图
            var intentObj = new GameObject("Intent");
            intentObj.transform.SetParent(cardObj.transform, false);
            var intentRect = intentObj.AddComponent<RectTransform>();
            intentRect.anchorMin = new Vector2(0, 0.25f);
            intentRect.anchorMax = new Vector2(1, 0.45f);
            intentRect.offsetMin = Vector2.zero;
            intentRect.offsetMax = Vector2.zero;
            var intentText = intentObj.AddComponent<Text>();
            intentText.text = $"攻击: {enemy.minDamage}-{enemy.maxDamage}";
            intentText.fontSize = 18;
            intentText.alignment = TextAnchor.MiddleCenter;
            intentText.color = new Color(1f, 0.5f, 0.5f);
            intentText.font = ChineseUI.GetChineseFont();

            enemyObjects.Add(cardObj);

            // 点击敌人时设为目标
            var button = cardObj.AddComponent<Button>();
            var capturedEnemy = enemy;
            button.onClick.AddListener(() => { targetEnemy = capturedEnemy; });
        }
    }

    void RefreshEnemyPanel()
    {
        if (BattleManager.Instance == null) return;

        var enemies = BattleManager.Instance.GetAliveEnemies();
        for (int i = 0; i < enemyHPTexts.Count && i < enemies.Count; i++)
        {
            if (enemyHPTexts[i] != null)
                enemyHPTexts[i].text = $"HP: {enemies[i].currentHP}/{enemies[i].maxHP}";
            if (enemyHPBars[i] != null)
                enemyHPBars[i].value = enemies[i].currentHP;
        }
    }

    void CreateHandPanel(Transform parent)
    {
        // 手牌区：底部全宽，高度28%，不与左右面板重叠
        handPanel = new GameObject("HandPanel");
        handPanel.transform.SetParent(parent, false);

        var rect = handPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.01f, 0.01f);
        rect.anchorMax = new Vector2(0.99f, 0.29f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        handPanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.25f);

        var layout = handPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.padding = new RectOffset(10, 10, 6, 6);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
    }
    
    void CreateButtonPanel(Transform parent)
    {
        // 按钮面板：右侧中部，与玩家面板对齐
        buttonPanel = new GameObject("ButtonPanel");
        buttonPanel.transform.SetParent(parent, false);

        var rect = buttonPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.78f, 0.30f);
        rect.anchorMax = new Vector2(0.99f, 0.72f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        buttonPanel.AddComponent<Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.92f);
        
        // 能量显示
        var energyObj = new GameObject("EnergyText");
        energyObj.transform.SetParent(buttonPanel.transform, false);
        var energyRect = energyObj.AddComponent<RectTransform>();
        energyRect.anchorMin = new Vector2(0, 0.7f);
        energyRect.anchorMax = new Vector2(1, 1);
        energyRect.offsetMin = Vector2.zero;
        energyRect.offsetMax = Vector2.zero;
        
        energyText = energyObj.AddComponent<Text>();
        energyText.text = "能量: 3/3";
        energyText.fontSize = 30;
        energyText.alignment = TextAnchor.MiddleCenter;
        energyText.color = new Color(0.4f, 0.75f, 1f);
        energyText.font = ChineseUI.GetChineseFont();

        // 回合显示
        var turnObj = new GameObject("TurnText");
        turnObj.transform.SetParent(buttonPanel.transform, false);
        var turnRect = turnObj.AddComponent<RectTransform>();
        turnRect.anchorMin = new Vector2(0, 0.60f);
        turnRect.anchorMax = new Vector2(1, 0.72f);
        turnRect.offsetMin = Vector2.zero;
        turnRect.offsetMax = Vector2.zero;

        turnText = turnObj.AddComponent<Text>();
        turnText.text = "回合: 1";
        turnText.fontSize = 22;
        turnText.alignment = TextAnchor.MiddleCenter;
        turnText.color = new Color(0.85f, 0.85f, 0.85f);
        turnText.font = ChineseUI.GetChineseFont();

        // 结束回合按钮
        var btnObj = new GameObject("EndTurnButton");
        btnObj.transform.SetParent(buttonPanel.transform, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.05f, 0.03f);
        btnRect.anchorMax = new Vector2(0.95f, 0.38f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.75f, 0.18f, 0.18f);

        endTurnButton = btnObj.AddComponent<Button>();
        endTurnButton.onClick.AddListener(OnEndTurn);

        var btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        var btnTextRect = btnTextObj.AddComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;

        var btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "结束回合";
        btnText.fontSize = 26;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;
        btnText.font = ChineseUI.GetChineseFont();

        // 投降按钮
        var surrenderObj = new GameObject("SurrenderButton");
        surrenderObj.transform.SetParent(buttonPanel.transform, false);
        var surrenderRect = surrenderObj.AddComponent<RectTransform>();
        surrenderRect.anchorMin = new Vector2(0.05f, 0.40f);
        surrenderRect.anchorMax = new Vector2(0.95f, 0.58f);
        surrenderRect.offsetMin = Vector2.zero;
        surrenderRect.offsetMax = Vector2.zero;
        surrenderObj.AddComponent<Image>().color = new Color(0.35f, 0.35f, 0.38f);
        surrenderObj.AddComponent<Button>().onClick.AddListener(OnSurrender);

        var surTextObj = new GameObject("Text");
        surTextObj.transform.SetParent(surrenderObj.transform, false);
        var surTextRect = surTextObj.AddComponent<RectTransform>();
        surTextRect.anchorMin = Vector2.zero;
        surTextRect.anchorMax = Vector2.one;
        surTextRect.offsetMin = Vector2.zero;
        surTextRect.offsetMax = Vector2.zero;
        var surText = surTextObj.AddComponent<Text>();
        surText.text = "投降";
        surText.fontSize = 22;
        surText.alignment = TextAnchor.MiddleCenter;
        surText.color = Color.white;
        surText.font = ChineseUI.GetChineseFont();
    }
    
    // 事件处理
    void OnPlayerTurnStart()
    {
        RefreshHand();
        UpdateUI();
        // 新回合开始时清除战斗日志
        if (combatLogText != null) combatLogText.text = "";
    }

    void OnEnemyTurnEnd()
    {
        // 强制刷新玩家HP和护盾显示
        if (Player.Instance != null)
            UpdatePlayerHP(Player.Instance.currentHP, Player.Instance.maxHP);
        UpdatePlayerBlock(0);
    }

    void OnPlayerTakeDamage(int damage)
    {
        if (damage <= 0)
        {
            AudioManager.Instance?.PlayShieldSFX();
            if (combatLogText != null && Player.Instance != null)
                combatLogText.text = $"护盾吸收了攻击！\n护盾剩余: {Player.Instance.currentBlock}";
        }
        else
        {
            AudioManager.Instance?.PlayAttackSFX();
            if (combatLogText != null && Player.Instance != null)
                combatLogText.text = $"受到 {damage} 点伤害！\nHP: {Player.Instance.currentHP}/{Player.Instance.maxHP}";
        }
    }
    
    void UpdateEnergy(int energy)
    {
        if (energyText != null && BattleManager.Instance != null)
        {
            energyText.text = $"能量: {energy}/{BattleManager.Instance.maxEnergy}";
        }
    }
    
    void UpdatePlayerHP(int currentHP, int maxHP)
    {
        if (playerHPText != null)
            playerHPText.text = $"生命: {currentHP}/{maxHP}";

        if (playerHPBar != null)
            playerHPBar.value = maxHP > 0 ? (float)currentHP / maxHP : 0;

        // 同步刷新护盾（OnHPChanged 在护盾变化时也会触发）
        UpdatePlayerBlock(0);
    }

    void UpdatePlayerBlock(int block)
    {
        if (playerBlockText != null && Player.Instance != null)
            playerBlockText.text = $"护盾: {Player.Instance.currentBlock}";
    }
    
    void OnCardDrawn(CardData card)
    {
        CreateCardUI(card);
        AudioManager.Instance?.PlayCardDrawSFX();
    }

    void OnCardPlayed(CardData card)
    {
        RefreshHand();
        RefreshEnemyPanel();
        AudioManager.Instance?.PlayCardPlaySFX();
    }
    
    void CreateCardUI(CardData card)
    {
        var cardObj = new GameObject($"Card_{card.cardName}");
        cardObj.transform.SetParent(handPanel.transform, false);

        var rect = cardObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(170, 260);

        cardObj.AddComponent<Image>().color = GetCardColor(card.rarity);

        // 卡名（顶部条）
        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform, false);
        var nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.82f);
        nameRect.anchorMax = Vector2.one;
        nameRect.offsetMin = new Vector2(4, 0);
        nameRect.offsetMax = new Vector2(-4, -2);
        var nameText = nameObj.AddComponent<Text>();
        nameText.text = card.cardName;
        nameText.fontSize = 22;
        nameText.fontStyle = FontStyle.Bold;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = new Color(0.05f, 0.05f, 0.05f);
        nameText.font = ChineseUI.GetChineseFont();
        nameText.horizontalOverflow = HorizontalWrapMode.Wrap;
        nameText.verticalOverflow = VerticalWrapMode.Truncate;

        // 描述（靠下居中区域，字体大）
        var descObj = new GameObject("Desc");
        descObj.transform.SetParent(cardObj.transform, false);
        var descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.04f, 0.20f);
        descRect.anchorMax = new Vector2(0.96f, 0.80f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;
        var descText = descObj.AddComponent<Text>();
        descText.text = card.description;
        descText.fontSize = 18;
        descText.alignment = TextAnchor.LowerCenter;
        descText.color = new Color(0.1f, 0.1f, 0.2f);
        descText.font = ChineseUI.GetChineseFont();
        descText.horizontalOverflow = HorizontalWrapMode.Wrap;
        descText.verticalOverflow = VerticalWrapMode.Overflow;

        // 费用（左下）— Image + 子Text
        var costBg = new GameObject("CostBg");
        costBg.transform.SetParent(cardObj.transform, false);
        var costBgRect = costBg.AddComponent<RectTransform>();
        costBgRect.anchorMin = new Vector2(0, 0);
        costBgRect.anchorMax = new Vector2(0.35f, 0.22f);
        costBgRect.offsetMin = Vector2.zero;
        costBgRect.offsetMax = Vector2.zero;
        costBg.AddComponent<Image>().color = new Color(0.2f, 0.4f, 0.9f);

        var costTextObj = new GameObject("CostText");
        costTextObj.transform.SetParent(costBg.transform, false);
        var costTextRect = costTextObj.AddComponent<RectTransform>();
        costTextRect.anchorMin = Vector2.zero;
        costTextRect.anchorMax = Vector2.one;
        costTextRect.offsetMin = Vector2.zero;
        costTextRect.offsetMax = Vector2.zero;
        var costText = costTextObj.AddComponent<Text>();
        costText.text = card.cost.ToString();
        costText.fontSize = 30;
        costText.fontStyle = FontStyle.Bold;
        costText.alignment = TextAnchor.MiddleCenter;
        costText.color = Color.white;
        costText.font = ChineseUI.GetChineseFont();

        // 数值（右下）— Image + 子Text
        var valueBg = new GameObject("ValueBg");
        valueBg.transform.SetParent(cardObj.transform, false);
        var valueBgRect = valueBg.AddComponent<RectTransform>();
        valueBgRect.anchorMin = new Vector2(0.65f, 0);
        valueBgRect.anchorMax = new Vector2(1, 0.22f);
        valueBgRect.offsetMin = Vector2.zero;
        valueBgRect.offsetMax = Vector2.zero;
        valueBg.AddComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f);

        var valueTextObj = new GameObject("ValueText");
        valueTextObj.transform.SetParent(valueBg.transform, false);
        var valueTextRect = valueTextObj.AddComponent<RectTransform>();
        valueTextRect.anchorMin = Vector2.zero;
        valueTextRect.anchorMax = Vector2.one;
        valueTextRect.offsetMin = Vector2.zero;
        valueTextRect.offsetMax = Vector2.zero;
        var valueText = valueTextObj.AddComponent<Text>();
        valueText.text = card.value.ToString();
        valueText.fontSize = 30;
        valueText.fontStyle = FontStyle.Bold;
        valueText.alignment = TextAnchor.MiddleCenter;
        valueText.color = Color.white;
        valueText.font = ChineseUI.GetChineseFont();

        var dragger = cardObj.AddComponent<CardDragger>();
        dragger.card = card;
        dragger.battleUI = this;

        handCardObjects.Add(cardObj);
    }
    
    void RefreshHand()
    {
        // 清除所有手牌UI
        foreach (var cardObj in handCardObjects)
        {
            if (cardObj != null) Destroy(cardObj);
        }
        handCardObjects.Clear();

        // 从 BattleManager 重建手牌
        if (BattleManager.Instance != null)
        {
            foreach (var card in BattleManager.Instance.GetHand())
            {
                CreateCardUI(card);
            }
        }
    }
    
    Color GetCardColor(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common: return new Color(0.9f, 0.9f, 0.9f);
            case CardRarity.Rare: return new Color(0.7f, 0.85f, 1f);
            case CardRarity.Epic: return new Color(0.8f, 0.6f, 1f);
            case CardRarity.Legendary: return new Color(1f, 0.8f, 0.3f);
            default: return Color.white;
        }
    }
    
    void UpdateUI()
    {
        if (BattleManager.Instance != null)
        {
            UpdateEnergy(BattleManager.Instance.currentEnergy);

            if (turnText != null)
                turnText.text = $"回合: {BattleManager.Instance.currentTurn}";

            // 更新敌人HP显示
            RefreshEnemyPanel();

            // 默认目标为第一个存活敌人
            if (targetEnemy == null || targetEnemy.currentHP <= 0)
            {
                var alive = BattleManager.Instance.GetAliveEnemies();
                targetEnemy = alive.Count > 0 ? alive[0] : null;
            }
        }

        if (Player.Instance != null)
        {
            UpdatePlayerHP(Player.Instance.currentHP, Player.Instance.maxHP);
            UpdatePlayerBlock(0);
        }
    }
    
    void OnBattleEnd()
    {
        Debug.Log("[BattleUI] 战斗结束，显示结算UI");

        bool isVictory = BattleManager.Instance == null ||
                         BattleManager.Instance.GetAliveEnemies().Count == 0;

        // 销毁战斗UI
        Destroy(gameObject);

        // 显示战斗结算UI
        var resultObj = new GameObject("BattleResultUI");
        var resultUI = resultObj.AddComponent<BattleResultUI>();
        resultUI.isVictory = isVictory;
        resultUI.Init();
    }

    void OnEndTurn()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.EndPlayerTurn();
        }
    }

    void OnSurrender()
    {
        Debug.Log("[BattleUI] 玩家投降");
        
        // 只销毁自己，保留Canvas
        Destroy(gameObject);

        // 返回主菜单
        var menuObj = new GameObject("MainMenuUI");
        menuObj.AddComponent<MainMenuUI_Chinese>().Init();
    }

    public void PlayCard(CardData card, Enemy target)
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.PlayCard(card, target);
        }
    }
}

/// <summary>
/// 卡牌拖拽组件
/// </summary>
public class CardDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardData card;
    public BattleUI battleUI;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // 打出到当前选中目标（默认第一个存活敌人）
        if (battleUI != null)
        {
            var target = battleUI.targetEnemy;
            if (target == null || target.currentHP <= 0)
            {
                var enemies = BattleManager.Instance?.GetAliveEnemies();
                target = (enemies != null && enemies.Count > 0) ? enemies[0] : null;
            }
            battleUI.PlayCard(card, target);
        }
        
        // 回到原位
        rectTransform.anchoredPosition = originalPosition;
    }
}
