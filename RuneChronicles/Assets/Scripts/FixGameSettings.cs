using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    public class FixGameSettings
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Fix Game Settings")]
        public static void FixSettings()
        {
            var gm = Object.FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.maxCardsPerTurn = 1;
                gm.cardsPlayedThisTurn = 0;
                
                // 增加敌人难度
                if (gm.currentEnemy != null)
                {
                    gm.currentEnemy.maxHealth = 30;
                    gm.currentEnemy.currentHealth = 30;
                    gm.currentEnemy.attack = 5;
                }
                
                EditorUtility.SetDirty(gm);
                Debug.Log("✅ 游戏设置已修复！");
                Debug.Log("- 每回合最多打 1 张卡");
                Debug.Log("- 敌人血量提升到 30");
                Debug.Log("- 敌人攻击力提升到 5");
            }
            
            // 添加更多卡牌到 CardManager
            var cm = Object.FindObjectOfType<CardManager>();
            if (cm != null)
            {
                // 清空旧卡牌
                cm.allCards.Clear();
                
                // 重新添加卡牌（包括新卡）
                var fire = Resources.Load<CardData>("Cards/Fire");
                var ice = Resources.Load<CardData>("Cards/Ice");
                
                if (fire != null) cm.allCards.Add(fire);
                if (ice != null) cm.allCards.Add(ice);
                
                // 尝试加载新卡（如果存在）
                var shield = Resources.Load<CardData>("Cards/Shield");
                var heal = Resources.Load<CardData>("Cards/Heal");
                var lightning = Resources.Load<CardData>("Cards/Lightning");
                
                if (shield != null) cm.allCards.Add(shield);
                if (heal != null) cm.allCards.Add(heal);
                if (lightning != null) cm.allCards.Add(lightning);
                
                EditorUtility.SetDirty(cm);
                Debug.Log($"✅ CardManager 卡牌库已更新！共 {cm.allCards.Count} 张卡");
            }
            
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
#endif
    }
}
