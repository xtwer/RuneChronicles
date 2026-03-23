using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    /// <summary>
    /// 创建更多测试卡牌
    /// </summary>
    public class CreateMoreCards
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Create More Cards")]
        public static void CreateCards()
        {
            // 创建防御卡
            var shieldCard = CreateCard("Shield", "获得 5 点护盾", 0, 2, CardRarity.Common);
            
            // 创建治疗卡
            var healCard = CreateCard("Heal", "恢复 3 点生命", 0, 3, CardRarity.Common);
            
            // 创建强力攻击卡
            var lightningCard = CreateCard("Lightning", "造成 8 点伤害", 8, 4, CardRarity.Rare);
            
            // 创建 AOE 卡（融合产物）
            var meteorCard = CreateCard("Meteor", "造成 15 点伤害", 15, 7, CardRarity.Epic);
            
            // 更新 CardManager 的卡牌库（需要手动在 Inspector 中添加）
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("✅ 新卡牌创建完成！");
            Debug.Log("卡牌：Shield（护盾）, Heal（治疗）, Lightning（闪电）, Meteor（流星）");
            Debug.Log("⚠️ 需要手动在 GameManager 和 CardManager 中添加这些卡牌！");
        }
        
        private static CardData CreateCard(string name, string desc, int attack, int mana, CardRarity rarity)
        {
            var card = ScriptableObject.CreateInstance<CardData>();
            card.cardName = name;
            card.description = desc;
            card.attack = attack;
            card.manaCost = mana;
            card.rarity = rarity;
            
            string path = $"Assets/Resources/Cards/{name}.asset";
            AssetDatabase.CreateAsset(card, path);
            
            return card;
        }
#endif
    }
}
