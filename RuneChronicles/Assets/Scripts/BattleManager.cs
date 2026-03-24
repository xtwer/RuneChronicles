using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 战斗管理器 - 核心战斗逻辑
/// Week 2: 回合制战斗系统
/// </summary>
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("战斗配置")]
    public int maxEnergy = 3;
    public int cardsPerTurn = 5;
    public int maxHandSize = 10;

    [Header("战斗状态")]
    public BattleState currentState = BattleState.None;
    public int currentEnergy = 0;
    public int currentTurn = 1;

    // 卡牌相关
    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> hand = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();

    // 敌人相关
    private List<Enemy> enemies = new List<Enemy>();

    // Power卡持续效果（战斗期间累加）
    [Header("Power卡效果")]
    public int playerBonusDamage = 0;   // 每次攻击额外伤害
    public int turnStartBlock = 0;       // 每回合开始获得的护盾
    public int turnStartHeal = 0;        // 每回合开始恢复的生命
    public int turnStartEnergy = 0;      // 每回合额外能量
    public int turnStartDraw = 0;        // 每回合额外抽牌
    public float lifeStealPercent = 0f;  // 吸血百分比（0~1）

    // 事件
    public event Action OnBattleStart;
    public event Action OnPlayerTurnStart;
    public event Action OnPlayerTurnEnd;
    public event Action OnEnemyTurnStart;
    public event Action OnEnemyTurnEnd;
    public event Action<int> OnEnergyChanged;
    public event Action<CardData> OnCardDrawn;
    public event Action<CardData> OnCardPlayed;
    public event Action OnBattleEnd;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 战斗流程控制

    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartBattle(List<CardData> playerDeck, List<Enemy> enemyList)
    {
        Debug.Log("[BattleManager] 战斗开始");
        
        // 初始化
        drawPile = new List<CardData>(playerDeck);
        hand.Clear();
        discardPile.Clear();
        enemies = new List<Enemy>(enemyList);
        
        currentTurn = 1;
        currentState = BattleState.BattleStart;

        // 重置Power卡效果
        playerBonusDamage = 0;
        turnStartBlock = 0;
        turnStartHeal = 0;
        turnStartEnergy = 0;
        turnStartDraw = 0;
        lifeStealPercent = 0f;

        // 洗牌
        ShuffleDeck();
        
        // 触发事件
        OnBattleStart?.Invoke();
        
        // 开始玩家回合
        StartPlayerTurn();
    }

    /// <summary>
    /// 开始玩家回合
    /// </summary>
    public void StartPlayerTurn()
    {
        Debug.Log($"[BattleManager] 玩家回合 {currentTurn} 开始");
        
        currentState = BattleState.PlayerTurn;

        // 清除护盾（杀戮尖塔规则：每回合开始清护盾）
        if (Player.Instance != null)
            Player.Instance.ClearBlock();

        // 恢复能量
        SetEnergy(maxEnergy);

        // 抽牌
        DrawCards(cardsPerTurn);

        // Power卡回合开始效果
        if (turnStartBlock > 0)
            Player.Instance?.GainBlock(turnStartBlock);
        if (turnStartHeal > 0)
            Player.Instance?.Heal(turnStartHeal);
        if (turnStartEnergy > 0)
            GainEnergy(turnStartEnergy);
        if (turnStartDraw > 0)
            DrawCards(turnStartDraw);

        // 触发事件
        OnPlayerTurnStart?.Invoke();
    }

    /// <summary>
    /// 结束玩家回合
    /// </summary>
    public void EndPlayerTurn()
    {
        Debug.Log("[BattleManager] 玩家回合结束");
        
        currentState = BattleState.PlayerTurnEnd;
        
        // 弃掉所有手牌
        DiscardHand();
        
        // 触发事件
        OnPlayerTurnEnd?.Invoke();
        
        // 开始敌人回合
        StartEnemyTurn();
    }

    /// <summary>
    /// 开始敌人回合
    /// </summary>
    public void StartEnemyTurn()
    {
        Debug.Log("[BattleManager] 敌人回合开始");
        
        currentState = BattleState.EnemyTurn;
        
        // 触发事件
        OnEnemyTurnStart?.Invoke();
        
        // 执行所有敌人的行动
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.currentHP > 0)
            {
                enemy.ExecuteAction();
            }
        }
        
        // 敌人回合结束
        EndEnemyTurn();
    }

    /// <summary>
    /// 结束敌人回合
    /// </summary>
    public void EndEnemyTurn()
    {
        Debug.Log("[BattleManager] 敌人回合结束");
        
        currentState = BattleState.EnemyTurnEnd;
        
        // 触发事件
        OnEnemyTurnEnd?.Invoke();
        
        // 检查战斗是否结束
        if (CheckBattleEnd())
        {
            EndBattle();
        }
        else
        {
            // 下一回合
            currentTurn++;
            StartPlayerTurn();
        }
    }

    /// <summary>
    /// 结束战斗
    /// </summary>
    public void EndBattle()
    {
        if (currentState == BattleState.BattleEnd) return; // 防止重复触发

        Debug.Log("[BattleManager] 战斗结束");

        currentState = BattleState.BattleEnd;

        // 触发事件
        OnBattleEnd?.Invoke();
    }

    /// <summary>
    /// 检查战斗是否结束
    /// </summary>
    private bool CheckBattleEnd()
    {
        // 所有敌人死亡 -> 玩家胜利
        bool allEnemiesDead = true;
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.currentHP > 0)
            {
                allEnemiesDead = false;
                break;
            }
        }
        
        if (allEnemiesDead)
        {
            Debug.Log("[BattleManager] 玩家胜利！");
            return true;
        }
        
        // 玩家死亡 -> 战斗失败
        if (Player.Instance != null && Player.Instance.isDead)
        {
            Debug.Log("[BattleManager] 玩家死亡，战斗失败！");
            return true;
        }

        return false;
    }

    #endregion

    #region 能量系统

    /// <summary>
    /// 设置能量
    /// </summary>
    public void SetEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(amount, 0, maxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy);
        Debug.Log($"[BattleManager] 当前能量: {currentEnergy}/{maxEnergy}");
    }

    /// <summary>
    /// 消耗能量
    /// </summary>
    public bool SpendEnergy(int amount)
    {
        if (currentEnergy >= amount)
        {
            SetEnergy(currentEnergy - amount);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 增加能量
    /// </summary>
    public void GainEnergy(int amount)
    {
        SetEnergy(currentEnergy + amount);
    }

    #endregion

    #region 卡牌系统

    /// <summary>
    /// 抽牌
    /// </summary>
    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            DrawCard();
        }
    }

    /// <summary>
    /// 抽一张牌
    /// </summary>
    public CardData DrawCard()
    {
        // 手牌已满
        if (hand.Count >= maxHandSize)
        {
            Debug.LogWarning("[BattleManager] 手牌已满，无法抽牌");
            return null;
        }
        
        // 抽牌堆空了，洗弃牌堆
        if (drawPile.Count == 0)
        {
            ReshuffleDiscardPile();
        }
        
        // 还是没牌，返回null
        if (drawPile.Count == 0)
        {
            Debug.LogWarning("[BattleManager] 没有可抽的牌了");
            return null;
        }
        
        // 抽牌
        CardData card = drawPile[0];
        drawPile.RemoveAt(0);
        hand.Add(card);
        
        Debug.Log($"[BattleManager] 抽到卡牌: {card.cardName}");
        OnCardDrawn?.Invoke(card);
        
        return card;
    }

    /// <summary>
    /// 打出卡牌
    /// </summary>
    public bool PlayCard(CardData card, Enemy target = null)
    {
        // 检查能量
        if (currentEnergy < card.cost)
        {
            Debug.LogWarning($"[BattleManager] 能量不足，无法打出 {card.cardName}");
            return false;
        }
        
        // 检查手牌
        if (!hand.Contains(card))
        {
            Debug.LogWarning($"[BattleManager] 手牌中没有 {card.cardName}");
            return false;
        }
        
        // 消耗能量
        SpendEnergy(card.cost);

        // 从手牌移除
        hand.Remove(card);

        // 执行卡牌效果
        ExecuteCardEffect(card, target);

        // 放入弃牌堆
        discardPile.Add(card);

        Debug.Log($"[BattleManager] 打出卡牌: {card.cardName}");
        OnCardPlayed?.Invoke(card); // UI先刷新，让玩家看到最后的血量变化

        // 打出攻击牌后立即检查战斗是否结束（敌人全灭）
        if (CheckBattleEnd())
        {
            EndBattle();
        }

        return true;
    }

    /// <summary>
    /// 执行卡牌效果（基于描述关键字解析多重效果）
    /// </summary>
    private void ExecuteCardEffect(CardData card, Enemy target)
    {
        string desc = card.description;

        Debug.Log($"[BattleManager] 执行效果: {card.cardName} 类型={card.cardType} 目标={(target != null ? target.enemyName : "null")}");

        // === 伤害效果 ===
        if (card.cardType == CardType.Attack)
        {
            int totalDamage = card.value + playerBonusDamage;

            // 血性：生命低于50%时额外加伤（临时检查）
            if (Player.Instance != null && Player.Instance.currentHP < Player.Instance.maxHP * 0.5f)
            {
                var condMatch = System.Text.RegularExpressions.Regex.Match(desc, @"低于.*?额外造成(\d+)");
                if (condMatch.Success)
                    totalDamage += int.Parse(condMatch.Groups[1].Value);
            }

            if (desc.Contains("所有敌人"))
            {
                foreach (var e in GetAliveEnemies())
                    e.TakeDamage(totalDamage);
            }
            else
            {
                // target 为 null 时自动找第一个存活敌人
                if (target == null || target.currentHP <= 0)
                {
                    var alive = GetAliveEnemies();
                    target = alive.Count > 0 ? alive[0] : null;
                }
                if (target != null)
                {
                    target.TakeDamage(totalDamage);
                    // 吸血
                    if (lifeStealPercent > 0)
                        Player.Instance?.Heal(Mathf.Max(1, Mathf.RoundToInt(totalDamage * lifeStealPercent)));
                }
                else
                {
                    Debug.LogWarning($"[BattleManager] 攻击卡 {card.cardName} 无有效目标！");
                }
            }
        }

        // === 护盾效果 ===
        if (desc.Contains("护盾") || desc.Contains("防御"))
        {
            // 解析护盾值：优先从描述中提取，否则使用card.value
            int blockValue = card.value;
            var blockMatch = System.Text.RegularExpressions.Regex.Match(desc, @"(\d+)点护盾");
            if (blockMatch.Success) blockValue = int.Parse(blockMatch.Groups[1].Value);
            else if (card.cardType == CardType.Attack)
            {
                // 攻击牌附带护盾，取描述中的数值
                blockMatch = System.Text.RegularExpressions.Regex.Match(desc, @"获得(\d+)");
                if (blockMatch.Success) blockValue = int.Parse(blockMatch.Groups[1].Value);
            }
            Player.Instance?.GainBlock(blockValue);
        }

        // === 治疗效果 ===
        if (desc.Contains("治疗") || desc.Contains("恢复"))
        {
            int healValue = card.value;
            var healMatch = System.Text.RegularExpressions.Regex.Match(desc, @"恢复(\d+)点生命");
            if (healMatch.Success) healValue = int.Parse(healMatch.Groups[1].Value);
            Player.Instance?.Heal(healValue);
        }

        // === 抽牌效果 ===
        if (desc.Contains("抽"))
        {
            var drawMatch = System.Text.RegularExpressions.Regex.Match(desc, @"抽(\d+)张");
            if (drawMatch.Success)
            {
                int drawCount = int.Parse(drawMatch.Groups[1].Value);
                DrawCards(drawCount);
            }
        }

        // === 能量效果 ===
        if (desc.Contains("能量"))
        {
            var energyMatch = System.Text.RegularExpressions.Regex.Match(desc, @"(\d+)点能量");
            if (energyMatch.Success)
            {
                int energyGain = int.Parse(energyMatch.Groups[1].Value);
                GainEnergy(energyGain);
            }
        }

        // === Power卡持续增益效果（注册到战斗期间生效）===
        if (card.cardType == CardType.Power)
        {
            bool registered = false;

            // 每次攻击额外造成N点伤害
            var bonusAtkMatch = System.Text.RegularExpressions.Regex.Match(desc, @"每次攻击额外造成(\d+)点伤害");
            if (bonusAtkMatch.Success)
            {
                int bonus = int.Parse(bonusAtkMatch.Groups[1].Value);
                playerBonusDamage += bonus;
                Debug.Log($"[BattleManager] {card.cardName}: 攻击加伤 +{bonus}（当前:{playerBonusDamage}）");
                registered = true;
            }

            // 每回合获得N点护盾（支持多效果描述，如"每回合开始获得3点护盾，恢复3点生命"）
            var blockMatch2 = System.Text.RegularExpressions.Regex.Match(desc, @"每回合[^。]*?获得(\d+)点护盾");
            if (blockMatch2.Success)
            {
                int b = int.Parse(blockMatch2.Groups[1].Value);
                turnStartBlock += b;
                Debug.Log($"[BattleManager] {card.cardName}: 每回合护盾 +{b}（当前:{turnStartBlock}）");
                registered = true;
            }

            // 每回合恢复N点生命
            var healMatch2 = System.Text.RegularExpressions.Regex.Match(desc, @"每回合[^。]*?恢复(\d+)点生命");
            if (healMatch2.Success)
            {
                int h = int.Parse(healMatch2.Groups[1].Value);
                turnStartHeal += h;
                Debug.Log($"[BattleManager] {card.cardName}: 每回合治疗 +{h}（当前:{turnStartHeal}）");
                registered = true;
            }

            // 每回合额外获得N点能量
            var energyMatch2 = System.Text.RegularExpressions.Regex.Match(desc, @"每回合额外获得(\d+)点能量");
            if (energyMatch2.Success)
            {
                int e = int.Parse(energyMatch2.Groups[1].Value);
                turnStartEnergy += e;
                Debug.Log($"[BattleManager] {card.cardName}: 每回合能量 +{e}（当前:{turnStartEnergy}）");
                registered = true;
            }

            // 每回合开始抽N张额外卡牌
            var drawMatch2 = System.Text.RegularExpressions.Regex.Match(desc, @"每回合[^，。]*抽(\d+)张");
            if (drawMatch2.Success)
            {
                int d = int.Parse(drawMatch2.Groups[1].Value);
                turnStartDraw += d;
                Debug.Log($"[BattleManager] {card.cardName}: 每回合抽牌 +{d}（当前:{turnStartDraw}）");
                registered = true;
            }

            // 吸血：攻击恢复N%伤害值的生命
            var lifeStealMatch = System.Text.RegularExpressions.Regex.Match(desc, @"攻击恢复(\d+)%");
            if (lifeStealMatch.Success)
            {
                float ls = int.Parse(lifeStealMatch.Groups[1].Value) / 100f;
                lifeStealPercent += ls;
                Debug.Log($"[BattleManager] {card.cardName}: 吸血 +{ls * 100}%（当前:{lifeStealPercent * 100}%）");
                registered = true;
            }

            // 未能解析的Power卡：即时给予护盾作为保底
            if (!registered)
            {
                Player.Instance?.GainBlock(card.value);
                Debug.Log($"[BattleManager] {card.cardName}: 通用效果 获得{card.value}点护盾");
            }
        }

        Debug.Log($"[BattleManager] 执行卡牌: {card.cardName} ({card.cardType})");
    }

    /// <summary>
    /// 弃掉所有手牌
    /// </summary>
    private void DiscardHand()
    {
        discardPile.AddRange(hand);
        hand.Clear();
        Debug.Log("[BattleManager] 弃掉所有手牌");
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    private void ShuffleDeck()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, drawPile.Count);
            var temp = drawPile[i];
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
        Debug.Log("[BattleManager] 牌堆已洗牌");
    }

    /// <summary>
    /// 将弃牌堆重新洗入抽牌堆
    /// </summary>
    private void ReshuffleDiscardPile()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
        Debug.Log("[BattleManager] 弃牌堆重新洗入抽牌堆");
    }

    #endregion

    #region 敌人管理

    /// <summary>
    /// 添加敌人
    /// </summary>
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    /// <summary>
    /// 移除敌人
    /// </summary>
    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    /// <summary>
    /// 获取所有存活的敌人
    /// </summary>
    public List<Enemy> GetAliveEnemies()
    {
        List<Enemy> aliveEnemies = new List<Enemy>();
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.currentHP > 0)
            {
                aliveEnemies.Add(enemy);
            }
        }
        return aliveEnemies;
    }

    #endregion

    #region 调试接口（与DebugManager配合）

    /// <summary>
    /// DEBUG: 立即胜利
    /// </summary>
    public void DEBUG_Win()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.currentHP > 0)
            {
                enemy.TakeDamage(enemy.currentHP);
            }
        }
        EndBattle();
    }

    /// <summary>
    /// 获取当前手牌（供UI读取）
    /// </summary>
    public List<CardData> GetHand()
    {
        return new List<CardData>(hand);
    }

    /// <summary>
    /// DEBUG: 添加卡牌到手牌
    /// </summary>
    public void DEBUG_AddCard(CardData card)
    {
        if (hand.Count < maxHandSize)
        {
            hand.Add(card);
            OnCardDrawn?.Invoke(card);
        }
    }

    #endregion
}

/// <summary>
/// 战斗状态枚举
/// </summary>
public enum BattleState
{
    None,
    BattleStart,
    PlayerTurn,
    PlayerTurnEnd,
    EnemyTurn,
    EnemyTurnEnd,
    BattleEnd
}
