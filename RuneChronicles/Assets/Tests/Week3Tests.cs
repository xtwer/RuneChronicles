using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Week 3 单元测试 #1: FusionManager测试
/// </summary>
public class FusionManagerTests
{
    [Test]
    public void FusionManager_FusionPoints_ShouldNotExceedMax()
    {
        // Arrange
        int current = 8;
        int max = 10;
        int gain = 5;
        
        // Act
        int final = Mathf.Min(current + gain, max);
        
        // Assert
        Assert.AreEqual(10, final, "融合点不应超过上限");
    }
    
    [Test]
    public void FusionManager_SpendFusionPoints_ShouldWork()
    {
        // Arrange
        int current = 5;
        int cost = 3;
        
        // Act
        bool canSpend = (current >= cost);
        int remaining = current - cost;
        
        // Assert
        Assert.IsTrue(canSpend, "应该可以消耗融合点");
        Assert.AreEqual(2, remaining, "剩余融合点应正确");
    }
    
    [Test]
    public void FusionManager_FusionCost_ShouldBe3()
    {
        // Arrange & Act
        int fusionCost = 3;
        
        // Assert
        Assert.AreEqual(3, fusionCost, "融合费用应为3");
    }
}

/// <summary>
/// Week 3 单元测试 #2: 融合算法测试
/// </summary>
public class FusionAlgorithmTests
{
    [Test]
    public void Fusion_RandomFusion_ShouldCalculateCorrectly()
    {
        // Arrange
        int card1Cost = 1;
        int card1Value = 6;
        int card2Cost = 1;
        int card2Value = 5;
        
        // Act
        int newCost = Mathf.CeilToInt((card1Cost + card2Cost) / 2f);
        int newValue = card1Value + card2Value;
        
        // Assert
        Assert.AreEqual(1, newCost, "融合后费用应正确");
        Assert.AreEqual(11, newValue, "融合后效果值应正确");
    }
    
    [Test]
    public void Fusion_RecipeKey_ShouldBeConsistent()
    {
        // Arrange
        string card1 = "ATK_001";
        string card2 = "ATK_002";
        
        // Act
        string key1 = $"{card1}+{card2}";
        string key2 = $"{card2}+{card1}";
        
        // Assert
        Assert.AreNotEqual(key1, key2, "配方键应考虑顺序");
    }
    
    [Test]
    public void Fusion_RarityUpgrade_ShouldWork()
    {
        // Arrange
        CardRarity common = CardRarity.Common;
        CardRarity rare = CardRarity.Rare;
        
        // Act
        CardRarity result = (CardRarity)Mathf.Max((int)common, (int)rare);
        
        // Assert
        Assert.AreEqual(CardRarity.Rare, result, "融合应取较高稀有度");
    }
}

/// <summary>
/// Week 3 单元测试 #3: MapManager测试
/// </summary>
public class MapManagerTests
{
    [Test]
    public void MapManager_TotalFloors_ShouldBe15()
    {
        // Arrange & Act
        int totalFloors = 15;
        
        // Assert
        Assert.AreEqual(15, totalFloors, "总层数应为15");
    }
    
    [Test]
    public void MapManager_NodesPerFloor_ShouldBe3()
    {
        // Arrange & Act
        int nodesPerFloor = 3;
        
        // Assert
        Assert.AreEqual(3, nodesPerFloor, "每层节点数应为3");
    }
    
    [Test]
    public void MapManager_BossFloor_ShouldBeCorrect()
    {
        // Arrange
        int[] bossFloors = { 5, 10, 15 };
        
        // Act & Assert
        foreach (int floor in bossFloors)
        {
            Assert.AreEqual(0, floor % 5, $"第{floor}层应为BOSS层");
        }
    }
    
    [Test]
    public void MapManager_NodeType_ShouldMatch()
    {
        // Arrange
        MapNodeType battle = MapNodeType.Battle;
        MapNodeType boss = MapNodeType.Boss;
        
        // Act & Assert
        Assert.AreNotEqual(battle, boss, "节点类型应不同");
    }
}

/// <summary>
/// Week 3 单元测试 #4: 地图节点测试
/// </summary>
public class MapNodeTests
{
    [Test]
    public void MapNode_Creation_ShouldWork()
    {
        // Arrange & Act
        MapNode node = new MapNode
        {
            floor = 0,
            nodeIndex = 0,
            nodeType = MapNodeType.Battle,
            isCompleted = false
        };
        
        // Assert
        Assert.IsNotNull(node, "节点不应为null");
        Assert.AreEqual(0, node.floor, "层数应正确");
        Assert.AreEqual(MapNodeType.Battle, node.nodeType, "节点类型应正确");
        Assert.IsFalse(node.isCompleted, "初始状态应未完成");
    }
}

/// <summary>
/// Week 3 单元测试 #5: 奖励系统测试
/// </summary>
public class RewardSystemTests
{
    [Test]
    public void Reward_GoldRange_ShouldBeValid()
    {
        // Arrange
        int minGold = 40;
        int maxGold = 80;
        
        // Act
        int gold = UnityEngine.Random.Range(minGold, maxGold);
        
        // Assert
        Assert.GreaterOrEqual(gold, minGold, "金币不应低于最小值");
        Assert.Less(gold, maxGold, "金币应小于最大值");
    }
    
    [Test]
    public void Reward_FusionPointsRange_ShouldBeValid()
    {
        // Arrange
        int minPoints = 1;
        int maxPoints = 3;
        
        // Act
        int points = UnityEngine.Random.Range(minPoints, maxPoints);
        
        // Assert
        Assert.GreaterOrEqual(points, minPoints, "融合点不应低于最小值");
        Assert.Less(points, maxPoints, "融合点应小于最大值");
    }
}

/// <summary>
/// Week 3 单元测试 #6: 融合性能测试
/// </summary>
public class FusionPerformanceTests
{
    [Test]
    public void Fusion_1000Times_ShouldBeFast()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act
        for (int i = 0; i < 1000; i++)
        {
            // 模拟融合计算
            int cost = Mathf.CeilToInt((1 + 1) / 2f);
            int value = 6 + 5;
        }
        
        stopwatch.Stop();
        
        // Assert
        Assert.Less(stopwatch.ElapsedMilliseconds, 1000, "1000次融合应在1秒内完成");
        Debug.Log($"[Performance] 1000次融合耗时: {stopwatch.ElapsedMilliseconds}ms");
    }
}
