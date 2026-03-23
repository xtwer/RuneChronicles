using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    /// <summary>
    /// 创建测试数据的工具类
    /// </summary>
    public class TestDataCreator
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Create Test Data")]
        public static void CreateTestData()
        {
            // 创建文件夹
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Cards"))
                AssetDatabase.CreateFolder("Assets/Resources", "Cards");
            
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Enemies"))
                AssetDatabase.CreateFolder("Assets/Resources", "Enemies");
            
            // 创建基础卡牌
            var fireCard = CreateCard("Fire", "Deal 5 damage", 5, 2, CardRarity.Common);
            var iceCard = CreateCard("Ice", "Deal 3 damage and slow", 3, 2, CardRarity.Common);
            
            // 创建融合卡牌
            var steamCard = CreateCard("Steam", "Deal 10 damage to all enemies", 10, 5, CardRarity.Rare);
            steamCard.fusionRecipe = new CardData[] { fireCard, iceCard };
            EditorUtility.SetDirty(steamCard);
            
            // 创建敌人
            var slime = CreateEnemy("Slime", 20, 3);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("✅ Test data created successfully!");
            Debug.Log($"Cards: {fireCard.name}, {iceCard.name}, {steamCard.name}");
            Debug.Log($"Enemy: {slime.name}");
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
        
        private static EnemyData CreateEnemy(string name, int hp, int atk)
        {
            var enemy = ScriptableObject.CreateInstance<EnemyData>();
            enemy.enemyName = name;
            enemy.maxHealth = hp;
            enemy.currentHealth = hp;
            enemy.attack = atk;
            enemy.attackInterval = 3;
            
            string path = $"Assets/Resources/Enemies/{name}.asset";
            AssetDatabase.CreateAsset(enemy, path);
            
            return enemy;
        }
#endif
    }
}
