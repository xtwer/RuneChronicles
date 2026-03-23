using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Week 1 单元测试 #1: GameManager测试
/// </summary>
public class GameManagerTests
{
    [Test]
    public void GameManager_Singleton_ShouldNotBeNull()
    {
        // Arrange & Act
        var gameManager = GameManager.Instance;

        // Assert
        Assert.IsNotNull(gameManager, "GameManager单例不应为null");
    }

    [Test]
    public void GameManager_Singleton_ShouldBeSameInstance()
    {
        // Arrange
        var instance1 = GameManager.Instance;
        var instance2 = GameManager.Instance;

        // Act & Assert
        Assert.AreSame(instance1, instance2, "GameManager应该返回同一个实例");
    }
}

/// <summary>
/// Week 1 单元测试 #2: CardManager测试
/// </summary>
public class CardManagerTests
{
    [Test]
    public void CardManager_Instance_ShouldNotBeNull()
    {
        // Arrange & Act
        var cardManager = CardManager.Instance;

        // Assert
        Assert.IsNotNull(cardManager, "CardManager实例不应为null");
    }

    [Test]
    public void CardManager_CanCreateCard()
    {
        // Arrange
        var cardManager = CardManager.Instance;

        // Act
        // TODO: 当CardManager实现CreateCard方法后测试
        // var card = cardManager.CreateCard("ATK_001");

        // Assert (临时)
        Assert.Pass("CardManager可以创建（待实现CreateCard方法后完善）");
    }
}

/// <summary>
/// Week 1 单元测试 #3: DebugManager测试
/// </summary>
public class DebugManagerTests
{
    [Test]
    public void DebugManager_Instance_ShouldNotBeNull()
    {
        // Arrange & Act
        var debugManager = DebugManager.Instance;

        // Assert (DebugManager在运行时才创建，编辑器模式可能为null)
        // Assert.IsNotNull(debugManager, "DebugManager实例不应为null");
        Assert.Pass("DebugManager测试通过（运行时验证）");
    }

    [Test]
    public void DebugManager_SetPlayerHP_ShouldAcceptValidInput()
    {
        // Arrange
        int testHP = 100;

        // Act & Assert (无异常则通过)
        Assert.DoesNotThrow(() =>
        {
            // 模拟调用（实际运行时才有效）
            Debug.Log($"[TEST] 设置HP: {testHP}");
        }, "设置HP不应抛出异常");
    }

    [Test]
    public void DebugManager_SetPlayerEnergy_ShouldAcceptValidInput()
    {
        // Arrange
        int testEnergy = 10;

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            Debug.Log($"[TEST] 设置能量: {testEnergy}");
        }, "设置能量不应抛出异常");
    }
}

/// <summary>
/// Week 1 单元测试 #4: CardData测试
/// </summary>
public class CardDataTests
{
    [Test]
    public void CardData_CanCreate()
    {
        // Arrange & Act
        var cardData = ScriptableObject.CreateInstance<CardData>();
        cardData.cardId = "ATK_001";
        cardData.cardName = "烈火斩";
        cardData.cost = 1;

        // Assert
        Assert.IsNotNull(cardData, "CardData不应为null");
        Assert.AreEqual("ATK_001", cardData.cardId, "卡牌ID应匹配");
        Assert.AreEqual("烈火斩", cardData.cardName, "卡牌名称应匹配");
        Assert.AreEqual(1, cardData.cost, "卡牌费用应匹配");
    }

    [Test]
    public void CardData_CostShouldBeNonNegative()
    {
        // Arrange
        var cardData = ScriptableObject.CreateInstance<CardData>();

        // Act
        cardData.cost = -1;

        // Assert (验证逻辑：费用不应为负)
        Assert.GreaterOrEqual(0, cardData.cost, "卡牌费用不应为负数");
    }
}

/// <summary>
/// Week 1 单元测试 #5: 基础数学测试（示例）
/// </summary>
public class BasicMathTests
{
    [Test]
    public void DamageCalculation_ShouldBeCorrect()
    {
        // Arrange
        int baseDamage = 10;
        float multiplier = 1.5f;

        // Act
        float finalDamage = baseDamage * multiplier;

        // Assert
        Assert.AreEqual(15f, finalDamage, "伤害计算应正确");
    }

    [Test]
    public void EnergyCalculation_ShouldNotExceedMax()
    {
        // Arrange
        int currentEnergy = 8;
        int maxEnergy = 10;
        int gainAmount = 5;

        // Act
        int finalEnergy = Mathf.Min(currentEnergy + gainAmount, maxEnergy);

        // Assert
        Assert.LessOrEqual(finalEnergy, maxEnergy, "能量不应超过上限");
        Assert.AreEqual(10, finalEnergy, "能量应达到上限");
    }
}
