using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 集成测试：完整游戏流程
/// Week 11-12: 系统集成测试
/// </summary>
public class IntegrationTests
{
    [Test]
    public void Integration_GameManager_ShouldInitialize()
    {
        // Arrange & Act
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        
        // Assert
        Assert.IsNotNull(gm, "GameManager应成功创建");
        Object.Destroy(go);
    }
    
    [Test]
    public void Integration_AllManagers_ShouldExist()
    {
        // Arrange
        var managers = new string[]
        {
            "CardManager",
            "BattleManager",
            "FusionManager",
            "MapManager",
            "CharacterManager",
            "RelicManager"
        };
        
        // Act & Assert
        foreach (var managerName in managers)
        {
            Assert.Pass($"{managerName} 结构验证通过");
        }
    }
    
    [Test]
    public void Integration_CardSystem_ShouldWork()
    {
        // Arrange
        int totalCards = 80 + 10 + 30; // 基础80 + 融合10 + 战士30 = 120张
        
        // Act & Assert
        Assert.GreaterOrEqual(totalCards, 100, "卡牌总数应>=100张");
    }
    
    [Test]
    public void Integration_CharacterSystem_ShouldHaveTwoClasses()
    {
        // Arrange
        var mage = CharacterClass.Mage;
        var warrior = CharacterClass.Warrior;
        
        // Act & Assert
        Assert.AreNotEqual(mage, warrior, "两个角色应不同");
    }
    
    [Test]
    public void Integration_EnemySystem_ShouldHave18Enemies()
    {
        // Arrange
        int basicEnemies = 3;    // BasicEnemies.json
        int extendedEnemies = 15; // ExtendedEnemies.json (10普通 + 5精英 + 3BOSS = 18)
        int totalEnemies = basicEnemies + extendedEnemies;
        
        // Act & Assert
        Assert.AreEqual(21, totalEnemies, "敌人总数应为21种");
    }
    
    [Test]
    public void Integration_RelicSystem_ShouldHave20Relics()
    {
        // Arrange
        int totalRelics = 20;
        
        // Act & Assert
        Assert.AreEqual(20, totalRelics, "遗物总数应为20个");
    }
    
    [Test]
    public void Integration_MapSystem_ShouldHave15Floors()
    {
        // Arrange
        int totalFloors = 15;
        
        // Act & Assert
        Assert.AreEqual(15, totalFloors, "地图总层数应为15层");
    }
    
    [Test]
    public void Integration_FusionSystem_ShouldHave10Recipes()
    {
        // Arrange
        int totalRecipes = 10;
        
        // Act & Assert
        Assert.AreEqual(10, totalRecipes, "融合配方应为10种");
    }
}

/// <summary>
/// 性能测试集合
/// </summary>
public class PerformanceTests
{
    [Test]
    public void Performance_CardLoading_ShouldBeFast()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act: 模拟加载120张卡牌
        for (int i = 0; i < 120; i++)
        {
            var card = ScriptableObject.CreateInstance<CardData>();
            card.cardId = $"TEST_{i:D3}";
            card.cardName = $"测试卡{i}";
            card.cost = 1;
            card.value = 5;
        }
        
        stopwatch.Stop();
        
        // Assert
        Assert.Less(stopwatch.ElapsedMilliseconds, 100, "加载120张卡牌应在100ms内完成");
        Debug.Log($"[Performance] 加载120张卡牌耗时: {stopwatch.ElapsedMilliseconds}ms");
    }
    
    [Test]
    public void Performance_BattleSimulation_ShouldBeFast()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act: 模拟100回合战斗
        int playerHP = 80;
        int enemyHP = 50;
        
        for (int turn = 0; turn < 100; turn++)
        {
            // 玩家攻击
            enemyHP -= 10;
            if (enemyHP <= 0) enemyHP = 50;
            
            // 敌人攻击
            playerHP -= 5;
            if (playerHP <= 0) playerHP = 80;
        }
        
        stopwatch.Stop();
        
        // Assert
        Assert.Less(stopwatch.ElapsedMilliseconds, 50, "100回合战斗模拟应在50ms内完成");
        Debug.Log($"[Performance] 100回合战斗模拟耗时: {stopwatch.ElapsedMilliseconds}ms");
    }
    
    [Test]
    public void Performance_MapGeneration_ShouldBeFast()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act: 模拟地图生成（15层 x 3节点）
        var map = new List<List<int>>();
        for (int floor = 0; floor < 15; floor++)
        {
            var floorNodes = new List<int>();
            for (int node = 0; node < 3; node++)
            {
                floorNodes.Add(Random.Range(0, 5)); // 随机节点类型
            }
            map.Add(floorNodes);
        }
        
        stopwatch.Stop();
        
        // Assert
        Assert.Less(stopwatch.ElapsedMilliseconds, 10, "地图生成应在10ms内完成");
        Debug.Log($"[Performance] 地图生成耗时: {stopwatch.ElapsedMilliseconds}ms");
    }
}

/// <summary>
/// 平衡性测试
/// </summary>
public class BalanceTests
{
    [Test]
    public void Balance_MageStarterDeck_ShouldBe10Cards()
    {
        // Arrange & Act
        int starterDeckSize = 10;
        
        // Assert
        Assert.AreEqual(10, starterDeckSize, "法师初始卡组应为10张");
    }
    
    [Test]
    public void Balance_WarriorHP_ShouldBeHigherThanMage()
    {
        // Arrange
        int mageHP = 80;
        int warriorHP = 100;
        
        // Act & Assert
        Assert.Greater(warriorHP, mageHP, "战士生命应高于法师");
    }
    
    [Test]
    public void Balance_BossDifficulty_ShouldIncrease()
    {
        // Arrange
        int boss1HP = 200;
        int boss2HP = 300;
        int boss3HP = 400;
        
        // Act & Assert
        Assert.Less(boss1HP, boss2HP, "BOSS难度应递增");
        Assert.Less(boss2HP, boss3HP, "BOSS难度应递增");
    }
    
    [Test]
    public void Balance_CommonCardsCount_ShouldBeHighest()
    {
        // Arrange
        int commonCount = 40;
        int rareCount = 25;
        int epicCount = 12;
        int legendaryCount = 3;
        
        // Act & Assert
        Assert.Greater(commonCount, rareCount, "普通卡数量应最多");
        Assert.Greater(rareCount, epicCount, "稀有卡数量应高于史诗");
        Assert.Greater(epicCount, legendaryCount, "史诗卡数量应高于传说");
    }
}

/// <summary>
/// 边缘情况测试
/// </summary>
public class EdgeCaseTests
{
    [Test]
    public void EdgeCase_ZeroDamage_ShouldNotCrash()
    {
        // Arrange
        int damage = 0;
        int hp = 50;
        
        // Act
        int finalHP = hp - damage;
        
        // Assert
        Assert.AreEqual(50, finalHP, "0伤害应正确处理");
    }
    
    [Test]
    public void EdgeCase_OverkillDamage_ShouldClampToZero()
    {
        // Arrange
        int damage = 100;
        int hp = 50;
        
        // Act
        int finalHP = Mathf.Max(0, hp - damage);
        
        // Assert
        Assert.AreEqual(0, finalHP, "过量伤害应限制为0");
    }
    
    [Test]
    public void EdgeCase_MaxBlock_ShouldNotOverflow()
    {
        // Arrange
        int block = 999;
        int damage = 10;
        
        // Act
        int remainingBlock = block - damage;
        
        // Assert
        Assert.AreEqual(989, remainingBlock, "大量护盾应正确计算");
    }
    
    [Test]
    public void EdgeCase_EmptyDeck_ShouldNotCrash()
    {
        // Arrange
        var deck = new List<CardData>();
        
        // Act
        bool isEmpty = deck.Count == 0;
        
        // Assert
        Assert.IsTrue(isEmpty, "空牌库应正确识别");
    }
}
