using UnityEngine;
using System;

/// <summary>
/// 敌人基类
/// Week 2: 简单AI系统
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("敌人数据")]
    public string enemyId;
    public string enemyName;
    public int maxHP = 50;
    public int currentHP = 50;
    
    [Header("行为模式")]
    public EnemyBehaviorPattern behaviorPattern = EnemyBehaviorPattern.AttackOnly;
    public int minDamage = 5;
    public int maxDamage = 10;
    
    [Header("当前意图")]
    public EnemyIntent currentIntent = EnemyIntent.Attack;
    public int intentValue = 0;

    [Header("护盾")]
    public int currentBlock = 0;
    public int bonusDamage = 0; // Buff增伤

    // 事件
    public event Action<int> OnTakeDamage;
    public event Action OnDeath;

    private int turnCount = 0;
    
    void Start()
    {
        currentHP = maxHP;
        DetermineNextIntent();
    }
    
    #region 敌人行为
    
    /// <summary>
    /// 执行当前回合的行动
    /// </summary>
    public void ExecuteAction()
    {
        turnCount++;
        currentBlock = 0; // 每回合清除护盾
        
        switch (currentIntent)
        {
            case EnemyIntent.Attack:
                PerformAttack();
                break;
            
            case EnemyIntent.Defend:
                PerformDefend();
                break;
            
            case EnemyIntent.Buff:
                PerformBuff();
                break;
            
            case EnemyIntent.Debuff:
                PerformDebuff();
                break;
        }
        
        // 决定下一回合的意图
        DetermineNextIntent();
    }
    
    /// <summary>
    /// 攻击玩家
    /// </summary>
    private void PerformAttack()
    {
        int damage = UnityEngine.Random.Range(minDamage, maxDamage + 1) + bonusDamage;
        Debug.Log($"[Enemy] {enemyName} 攻击玩家，造成 {damage} 点伤害");

        if (Player.Instance != null)
            Player.Instance.TakeDamage(damage);
    }

    private void PerformDefend()
    {
        int block = intentValue > 0 ? intentValue : 8;
        currentBlock += block;
        Debug.Log($"[Enemy] {enemyName} 防御，获得 {block} 点护盾（当前: {currentBlock}）");
    }

    private void PerformBuff()
    {
        bonusDamage += 3;
        Heal(5);
        Debug.Log($"[Enemy] {enemyName} 使用增益：攻击+3（当前增伤: {bonusDamage}），恢复5HP");
    }

    private void PerformDebuff()
    {
        int debuffDamage = UnityEngine.Random.Range(2, 5);
        Debug.Log($"[Enemy] {enemyName} 对玩家施加减益：造成{debuffDamage}点伤害");
        if (Player.Instance != null)
            Player.Instance.TakeDamage(debuffDamage);
    }
    
    /// <summary>
    /// 决定下一回合的意图（根据行为模式）
    /// </summary>
    private void DetermineNextIntent()
    {
        switch (behaviorPattern)
        {
            case EnemyBehaviorPattern.AttackOnly:
                // 一直攻击
                currentIntent = EnemyIntent.Attack;
                intentValue = UnityEngine.Random.Range(minDamage, maxDamage + 1);
                break;
            
            case EnemyBehaviorPattern.AttackDefend:
                // 攻击 -> 防御 -> 循环
                if (turnCount % 2 == 0)
                {
                    currentIntent = EnemyIntent.Attack;
                    intentValue = UnityEngine.Random.Range(minDamage, maxDamage + 1);
                }
                else
                {
                    currentIntent = EnemyIntent.Defend;
                    intentValue = 8;
                }
                break;
            
            case EnemyBehaviorPattern.Random:
                int random = UnityEngine.Random.Range(0, 4);
                switch (random)
                {
                    case 0: case 1:
                        currentIntent = EnemyIntent.Attack;
                        intentValue = UnityEngine.Random.Range(minDamage, maxDamage + 1);
                        break;
                    case 2:
                        currentIntent = EnemyIntent.Defend;
                        intentValue = 8;
                        break;
                    case 3:
                        currentIntent = EnemyIntent.Buff;
                        intentValue = 0;
                        break;
                }
                break;

            case EnemyBehaviorPattern.Aggressive:
                // 70%攻击 20%Buff 10%防御
                float aggroRoll = UnityEngine.Random.value;
                if (aggroRoll < 0.7f)
                {
                    currentIntent = EnemyIntent.Attack;
                    intentValue = UnityEngine.Random.Range(minDamage, maxDamage + 1);
                }
                else if (aggroRoll < 0.9f)
                {
                    currentIntent = EnemyIntent.Buff;
                    intentValue = 0;
                }
                else
                {
                    currentIntent = EnemyIntent.Defend;
                    intentValue = 5;
                }
                break;

            case EnemyBehaviorPattern.Defensive:
                // 50%防御 30%攻击 20%Buff
                float defRoll = UnityEngine.Random.value;
                if (defRoll < 0.5f)
                {
                    currentIntent = EnemyIntent.Defend;
                    intentValue = 10;
                }
                else if (defRoll < 0.8f)
                {
                    currentIntent = EnemyIntent.Attack;
                    intentValue = UnityEngine.Random.Range(minDamage, maxDamage + 1);
                }
                else
                {
                    currentIntent = EnemyIntent.Buff;
                    intentValue = 0;
                }
                break;
        }
        
        Debug.Log($"[Enemy] {enemyName} 下一回合意图: {currentIntent} (值: {intentValue})");
    }
    
    #endregion
    
    #region 伤害/治疗
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        int actualDamage = damage;

        // 先消耗护盾
        if (currentBlock > 0)
        {
            if (currentBlock >= damage)
            {
                currentBlock -= damage;
                actualDamage = 0;
                Debug.Log($"[Enemy] {enemyName} 护盾吸收 {damage} 点伤害，剩余护盾: {currentBlock}");
            }
            else
            {
                actualDamage = damage - currentBlock;
                currentBlock = 0;
            }
        }

        currentHP -= actualDamage;
        currentHP = Mathf.Max(0, currentHP);

        Debug.Log($"[Enemy] {enemyName} 受到 {actualDamage} 点伤害，剩余HP: {currentHP}/{maxHP}");

        OnTakeDamage?.Invoke(actualDamage);

        if (currentHP <= 0)
            Die();
    }
    
    /// <summary>
    /// 治疗
    /// </summary>
    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
        
        Debug.Log($"[Enemy] {enemyName} 恢复 {amount} 点生命，当前HP: {currentHP}/{maxHP}");
    }
    
    /// <summary>
    /// 死亡
    /// </summary>
    private void Die()
    {
        Debug.Log($"[Enemy] {enemyName} 已死亡");
        OnDeath?.Invoke();
        
        // 销毁GameObject
        Destroy(gameObject, 0.5f); // 延迟0.5秒销毁，让事件先触发
    }
    
    #endregion
}

/// <summary>
/// 敌人行为模式
/// </summary>
public enum EnemyBehaviorPattern
{
    AttackOnly,      // 一直攻击
    AttackDefend,    // 攻击-防御循环
    Random,          // 随机行为
    Aggressive,      // 激进（多次攻击）
    Defensive        // 防守（多次防御）
}

/// <summary>
/// 敌人意图
/// </summary>
public enum EnemyIntent
{
    Attack,          // 攻击
    Defend,          // 防御
    Buff,            // 增益
    Debuff,          // 减益
    Unknown          // 未知
}
