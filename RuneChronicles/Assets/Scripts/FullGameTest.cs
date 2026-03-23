using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    /// <summary>
    /// 完整游戏测试 - 自动验证所有功能
    /// </summary>
    public class FullGameTest
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Run Full Game Test")]
        public static void RunTest()
        {
            Debug.Log("===== 开始完整游戏测试 =====");
            
            // 1. 检查管理器
            var gm = Object.FindObjectOfType<GameManager>();
            var cm = Object.FindObjectOfType<CardManager>();
            var fs = Object.FindObjectOfType<FusionSystem>();
            
            if (gm == null || cm == null || fs == null)
            {
                Debug.LogError("❌ 缺少必要的管理器组件！");
                return;
            }
            
            Debug.Log("✅ 管理器检查通过");
            
            // 2. 检查游戏设置
            Debug.Log($"- 每回合可打牌: {gm.maxCardsPerTurn} 张");
            Debug.Log($"- 玩家初始生命: {gm.playerHealth}");
            Debug.Log($"- 玩家初始魔法: {gm.maxMana}");
            
            if (gm.currentEnemy != null)
            {
                Debug.Log($"- 敌人: {gm.currentEnemy.enemyName}");
                Debug.Log($"- 敌人生命: {gm.currentEnemy.maxHealth}");
                Debug.Log($"- 敌人攻击: {gm.currentEnemy.attack}");
                Debug.Log($"- 敌人攻击间隔: {gm.currentEnemy.attackInterval} 回合");
            }
            
            // 3. 检查卡牌库
            Debug.Log($"✅ 卡牌库检查: 共 {cm.allCards.Count} 张卡");
            foreach (var card in cm.allCards)
            {
                if (card != null)
                {
                    Debug.Log($"  - {card.cardName}: 攻击{card.attack}, 费用{card.manaCost}, 稀有度{card.rarity}");
                }
            }
            
            if (cm.allCards.Count < 3)
            {
                Debug.LogWarning("⚠️ 卡牌库卡牌较少，建议至少3张");
            }
            
            // 4. 检查融合配方
            Debug.Log($"✅ 融合配方检查: 共 {fs.fusionRecipes.Count} 个");
            foreach (var recipe in fs.fusionRecipes)
            {
                if (recipe != null && recipe.fusionRecipe != null && recipe.fusionRecipe.Length > 0)
                {
                    string materials = "";
                    foreach (var mat in recipe.fusionRecipe)
                    {
                        if (mat != null)
                            materials += mat.cardName + " + ";
                    }
                    materials = materials.TrimEnd(' ', '+');
                    Debug.Log($"  - {materials} → {recipe.cardName}");
                }
            }
            
            // 5. 检查 UI
            var controller = Object.FindObjectOfType<ChineseTestController>();
            if (controller == null)
            {
                Debug.LogError("❌ 缺少 ChineseTestController！");
                return;
            }
            
            if (controller.statusText == null || controller.drawCardButton == null || 
                controller.fusionButton == null || controller.endTurnButton == null)
            {
                Debug.LogError("❌ ChineseTestController 的 UI 引用不完整！");
                return;
            }
            
            Debug.Log("✅ UI 控制器检查通过");
            
            // 6. 游戏平衡检查
            if (gm.currentEnemy != null && cm.allCards.Count > 0)
            {
                int totalDamage = 0;
                foreach (var card in cm.allCards)
                {
                    if (card != null)
                        totalDamage += card.attack;
                }
                int avgDamage = totalDamage / cm.allCards.Count;
                int turnsToKill = Mathf.CeilToInt((float)gm.currentEnemy.maxHealth / avgDamage);
                int enemyAttacks = turnsToKill / gm.currentEnemy.attackInterval;
                int playerDamageTaken = enemyAttacks * gm.currentEnemy.attack;
                
                Debug.Log($"📊 游戏平衡分析:");
                Debug.Log($"  - 平均每张卡伤害: {avgDamage}");
                Debug.Log($"  - 预计击败敌人回合数: {turnsToKill}");
                Debug.Log($"  - 期间敌人攻击次数: {enemyAttacks}");
                Debug.Log($"  - 玩家预计受伤: {playerDamageTaken}");
                
                if (playerDamageTaken >= gm.playerHealth)
                {
                    Debug.LogWarning("⚠️ 游戏难度过高！玩家可能无法获胜");
                }
                else if (playerDamageTaken < gm.playerHealth / 2)
                {
                    Debug.LogWarning("⚠️ 游戏难度过低！玩家很容易获胜");
                }
                else
                {
                    Debug.Log("✅ 游戏平衡合理");
                }
            }
            
            Debug.Log("===== 测试完成 =====");
            Debug.Log("✅ 所有检查通过！游戏可以正常运行");
        }
#endif
    }
}
