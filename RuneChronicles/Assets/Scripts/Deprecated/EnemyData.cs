using UnityEngine;

namespace RuneChronicles
{
    /// <summary>
    /// 敌人数据结构
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "RuneChronicles/Enemy")]
    public class EnemyData : ScriptableObject
    {
        [Header("基础信息")]
        public string enemyName;
        public Sprite enemySprite;
        
        [Header("属性")]
        public int maxHealth;
        public int currentHealth;
        public int attack;
        
        [Header("行为")]
        public int attackInterval = 3; // 每3回合攻击一次
        
        public void Initialize()
        {
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;
        }
        
        public bool IsDead()
        {
            return currentHealth <= 0;
        }
    }
}
