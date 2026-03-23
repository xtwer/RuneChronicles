using UnityEngine;
using System;

/// <summary>
/// 玩家类
/// Week 2: 生命值、护盾、状态管理
/// </summary>
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    [Header("生命值")]
    public int maxHP = 80;
    public int currentHP = 80;
    
    [Header("护盾")]
    public int currentBlock = 0;
    
    [Header("状态")]
    public bool isDead = false;
    
    // 事件
    public event Action<int> OnTakeDamage;
    public event Action<int> OnGainBlock;
    public event Action<int> OnHeal;
    public event Action OnDeath;
    public event Action<int, int> OnHPChanged; // (currentHP, maxHP)
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        currentHP = maxHP;
        currentBlock = 0;
        isDead = false;
    }
    
    #region 伤害/治疗
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        int actualDamage = damage;
        
        // 先消耗护盾
        if (currentBlock > 0)
        {
            if (currentBlock >= damage)
            {
                // 护盾完全吸收
                currentBlock -= damage;
                actualDamage = 0;
                Debug.Log($"[Player] 护盾吸收 {damage} 点伤害，剩余护盾: {currentBlock}");
            }
            else
            {
                // 护盾部分吸收
                actualDamage = damage - currentBlock;
                Debug.Log($"[Player] 护盾吸收 {currentBlock} 点伤害，剩余伤害: {actualDamage}");
                currentBlock = 0;
            }
        }
        
        // 扣除生命值
        if (actualDamage > 0)
        {
            currentHP -= actualDamage;
            currentHP = Mathf.Max(0, currentHP);
            
            Debug.Log($"[Player] 受到 {actualDamage} 点伤害，剩余HP: {currentHP}/{maxHP}");
            
            OnTakeDamage?.Invoke(actualDamage);
            OnHPChanged?.Invoke(currentHP, maxHP);
            
            // 检查死亡
            if (currentHP <= 0)
            {
                Die();
            }
        }
    }
    
    /// <summary>
    /// 治疗
    /// </summary>
    public void Heal(int amount)
    {
        if (isDead) return;
        
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
        
        Debug.Log($"[Player] 恢复 {amount} 点生命，当前HP: {currentHP}/{maxHP}");
        
        OnHeal?.Invoke(amount);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }
    
    #endregion
    
    #region 护盾系统
    
    /// <summary>
    /// 获得护盾
    /// </summary>
    public void GainBlock(int amount)
    {
        if (isDead) return;
        
        currentBlock += amount;
        
        Debug.Log($"[Player] 获得 {amount} 点护盾，当前护盾: {currentBlock}");
        
        OnGainBlock?.Invoke(amount);
    }
    
    /// <summary>
    /// 清除护盾（回合结束时）
    /// </summary>
    public void ClearBlock()
    {
        if (currentBlock > 0)
        {
            Debug.Log($"[Player] 清除 {currentBlock} 点护盾");
            currentBlock = 0;
        }
    }
    
    #endregion
    
    #region 死亡
    
    /// <summary>
    /// 死亡
    /// </summary>
    private void Die()
    {
        isDead = true;
        Debug.Log("[Player] 玩家死亡");
        
        OnDeath?.Invoke();
        
        // TODO: 触发战斗失败
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.EndBattle();
        }
    }
    
    /// <summary>
    /// 复活（用于调试或特殊机制）
    /// </summary>
    public void Revive()
    {
        currentHP = maxHP;
        currentBlock = 0;
        isDead = false;
        
        Debug.Log("[Player] 玩家复活");
        OnHPChanged?.Invoke(currentHP, maxHP);
    }
    
    #endregion
    
    #region DEBUG接口
    
    /// <summary>
    /// DEBUG: 设置生命值
    /// </summary>
    public void DEBUG_SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        
        if (currentHP <= 0)
        {
            Die();
        }
        else if (isDead && currentHP > 0)
        {
            Revive();
        }
    }
    
    /// <summary>
    /// DEBUG: 设置护盾
    /// </summary>
    public void DEBUG_SetBlock(int block)
    {
        currentBlock = Mathf.Max(0, block);
    }
    
    #endregion
}
