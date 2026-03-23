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
        
        // 恢复能量
        SetEnergy(maxEnergy);
        
        // 抽牌
        DrawCards(cardsPerTurn);
        
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
    /// 执行卡牌效果
    /// </summary>
    private void ExecuteCardEffect(CardData card, Enemy target)
    {
        switch (card.cardType)
        {
            case CardType.Attack:
                if (target != null)
                {
                    target.TakeDamage(card.value);
                }
                break;
            
            case CardType.Skill:
                // 技能卡效果：根据描述判断效果类型
                if (card.description.Contains("护盾") || card.description.Contains("防御"))
                {
                    // 获得护盾
                    if (Player.Instance != null)
                    {
                        Player.Instance.GainBlock(card.value);
                    }
                }
                else if (card.description.Contains("治疗") || card.description.Contains("恢复"))
                {
                    // 恢复生命
                    if (Player.Instance != null)
                    {
                        Player.Instance.Heal(card.value);
                    }
                }
                Debug.Log($"[BattleManager] 执行技能卡: {card.cardName}");
                break;
            
            case CardType.Power:
                // 能力卡效果：增益效果
                Debug.Log($"[BattleManager] 执行能力卡: {card.cardName}（增益效果暂未实现）");
                break;
        }
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
