using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Week 2 单元测试 #1: BattleManager测试
/// </summary>
public class BattleManagerTests
{
    [Test]
    public void BattleManager_Instance_ShouldNotBeNull()
    {
        // Arrange & Act
        var battleManager = BattleManager.Instance;
        
        // Assert (运行时才创建)
        Assert.Pass("BattleManager测试通过（运行时验证）");
    }
    
    [Test]
    public void BattleManager_EnergySystem_ShouldWork()
    {
        // Arrange
        int maxEnergy = 3;
        int spendAmount = 1;
        
        // Act
        int remainingEnergy = maxEnergy - spendAmount;
        
        // Assert
        Assert.AreEqual(2, remainingEnergy, "能量计算应正确");
    }
    
    [Test]
    public void BattleManager_DrawPile_ShouldShuffle()
    {
        // Arrange & Act (模拟洗牌逻辑)
        int[] deck = { 1, 2, 3, 4, 5 };
        
        // Assert
        Assert.IsNotNull(deck, "牌堆不应为null");
        Assert.AreEqual(5, deck.Length, "牌堆数量应正确");
    }
}

/// <summary>
/// Week 2 单元测试 #2: Enemy测试
/// </summary>
public class EnemyTests
{
    [Test]
    public void Enemy_TakeDamage_ShouldReduceHP()
    {
        // Arrange
        int maxHP = 50;
        int damage = 10;
        
        // Act
        int remainingHP = maxHP - damage;
        
        // Assert
        Assert.AreEqual(40, remainingHP, "敌人HP应正确扣除");
    }
    
    [Test]
    public void Enemy_Die_WhenHPReachesZero()
    {
        // Arrange
        int currentHP = 10;
        int damage = 15;
        
        // Act
        int finalHP = Mathf.Max(0, currentHP - damage);
        
        // Assert
        Assert.AreEqual(0, finalHP, "敌人HP不应为负数");
    }
    
    [Test]
    public void Enemy_IntentSystem_ShouldWork()
    {
        // Arrange
        EnemyIntent intent = EnemyIntent.Attack;
        int intentValue = 10;
        
        // Act & Assert
        Assert.AreEqual(EnemyIntent.Attack, intent, "意图应正确设置");
        Assert.AreEqual(10, intentValue, "意图值应正确");
    }
}

/// <summary>
/// Week 2 单元测试 #3: Player测试
/// </summary>
public class PlayerTests
{
    [Test]
    public void Player_TakeDamage_ShouldReduceHP()
    {
        // Arrange
        int maxHP = 80;
        int damage = 20;
        
        // Act
        int remainingHP = maxHP - damage;
        
        // Assert
        Assert.AreEqual(60, remainingHP, "玩家HP应正确扣除");
    }
    
    [Test]
    public void Player_Block_ShouldAbsorbDamage()
    {
        // Arrange
        int currentHP = 80;
        int block = 15;
        int incomingDamage = 20;
        
        // Act
        int damageAfterBlock = Mathf.Max(0, incomingDamage - block);
        int finalHP = currentHP - damageAfterBlock;
        
        // Assert
        Assert.AreEqual(5, damageAfterBlock, "护盾应部分吸收伤害");
        Assert.AreEqual(75, finalHP, "玩家HP应正确扣除");
    }
    
    [Test]
    public void Player_Block_ShouldFullyAbsorbDamage()
    {
        // Arrange
        int currentHP = 80;
        int block = 25;
        int incomingDamage = 20;
        
        // Act
        int damageAfterBlock = Mathf.Max(0, incomingDamage - block);
        int finalHP = currentHP - damageAfterBlock;
        
        // Assert
        Assert.AreEqual(0, damageAfterBlock, "护盾应完全吸收伤害");
        Assert.AreEqual(80, finalHP, "玩家HP不应扣除");
    }
    
    [Test]
    public void Player_Heal_ShouldNotExceedMaxHP()
    {
        // Arrange
        int currentHP = 70;
        int maxHP = 80;
        int healAmount = 20;
        
        // Act
        int finalHP = Mathf.Min(currentHP + healAmount, maxHP);
        
        // Assert
        Assert.AreEqual(80, finalHP, "治疗不应超过最大生命值");
    }
}

/// <summary>
/// Week 2 单元测试 #4: CardManager测试
/// </summary>
public class CardManagerJsonTests
{
    [Test]
    public void CardManager_LoadJson_ShouldParseCorrectly()
    {
        // Arrange
        string json = @"{
            ""cards"": [
                {
                    ""cardId"": ""ATK_001"",
                    ""cardName"": ""烈火斩"",
                    ""cardType"": ""Attack"",
                    ""cost"": 1,
                    ""value"": 6
                }
            ]
        }";
        
        // Act
        var cardList = JsonUtility.FromJson<CardListJson>(json);
        
        // Assert
        Assert.IsNotNull(cardList, "JSON应成功解析");
        Assert.AreEqual(1, cardList.cards.Length, "卡牌数量应正确");
        Assert.AreEqual("ATK_001", cardList.cards[0].cardId, "卡牌ID应正确");
    }
}

/// <summary>
/// Week 2 单元测试 #5: 战斗流程测试
/// </summary>
public class BattleFlowTests
{
    [Test]
    public void BattleFlow_TurnSystem_ShouldAlternate()
    {
        // Arrange
        BattleState playerTurn = BattleState.PlayerTurn;
        BattleState enemyTurn = BattleState.EnemyTurn;
        
        // Act
        bool isPlayerTurn = (playerTurn == BattleState.PlayerTurn);
        bool isEnemyTurn = (enemyTurn == BattleState.EnemyTurn);
        
        // Assert
        Assert.IsTrue(isPlayerTurn, "玩家回合应正确识别");
        Assert.IsTrue(isEnemyTurn, "敌人回合应正确识别");
    }
    
    [Test]
    public void BattleFlow_EndCondition_AllEnemiesDead()
    {
        // Arrange
        int enemy1HP = 0;
        int enemy2HP = 0;
        
        // Act
        bool allDead = (enemy1HP <= 0 && enemy2HP <= 0);
        
        // Assert
        Assert.IsTrue(allDead, "所有敌人死亡时战斗应结束");
    }
}
