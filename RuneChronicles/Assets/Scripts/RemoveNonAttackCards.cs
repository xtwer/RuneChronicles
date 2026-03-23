using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    public class RemoveNonAttackCards
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Remove Non-Attack Cards")]
        public static void RemoveCards()
        {
            // 从 CardManager 中移除非攻击卡
            var cm = Object.FindObjectOfType<CardManager>();
            if (cm != null)
            {
                cm.allCards.Clear();
                
                // 只保留攻击卡
                var fire = Resources.Load<CardData>("Cards/Fire");
                var ice = Resources.Load<CardData>("Cards/Ice");
                var lightning = Resources.Load<CardData>("Cards/Lightning");
                
                if (fire != null) cm.allCards.Add(fire);
                if (ice != null) cm.allCards.Add(ice);
                if (lightning != null) cm.allCards.Add(lightning);
                
                EditorUtility.SetDirty(cm);
                
                Debug.Log($"✅ 已移除非攻击卡！当前卡牌库: {cm.allCards.Count} 张");
                Debug.Log("- Fire (5攻击, 2费用)");
                Debug.Log("- Ice (3攻击, 2费用)");
                Debug.Log("- Lightning (8攻击, 4费用)");
            }
            
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            
            Debug.Log("✅ 场景已标记为修改，请保存场景（Cmd+S）");
        }
#endif
    }
}
