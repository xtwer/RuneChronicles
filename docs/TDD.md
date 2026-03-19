# 技术设计文档（TDD）
# 符文编年史 Rune Chronicles

**版本：** v0.1  
**最后更新：** 2026-03-18

---

## 目录
1. [技术栈](#1-技术栈)
2. [架构设计](#2-架构设计)
3. [核心系统](#3-核心系统)
4. [数据设计](#4-数据设计)
5. [性能优化](#5-性能优化)
6. [开发工具](#6-开发工具)

---

## 1. 技术栈

### 1.1 游戏引擎
- **Unity 2022.3 LTS**
- 原因：稳定、资源多、跨平台

### 1.2 编程语言
- **C# (.NET Standard 2.1)**

### 1.3 版本控制
- **Git + GitHub**
- 分支策略：
  - `main` - 稳定版本
  - `develop` - 开发版本
  - `feature/*` - 功能分支

### 1.4 构建工具
- Unity Cloud Build（自动化构建）
- GitHub Actions（CI/CD）

---

## 2. 架构设计

### 2.1 整体架构

```
Game (单例)
├── GameManager
├── BattleManager
├── CardManager
├── EnemyManager
├── EventManager
├── SaveManager
└── AudioManager
```

### 2.2 设计模式

**单例模式（Singleton）：**
- GameManager
- AudioManager
- SaveManager

**观察者模式（Observer）：**
- 事件系统（HP变化、卡牌使用等）

**工厂模式（Factory）：**
- CardFactory（生成卡牌）
- EnemyFactory（生成敌人）

**状态机（State Machine）：**
- 游戏状态（菜单/地图/战斗）
- 战斗状态（玩家回合/敌人回合/结算）

**命令模式（Command）：**
- 卡牌效果执行
- 撤销/重做（可选）

---

## 3. 核心系统

### 3.1 战斗系统

**BattleManager.cs**

```csharp
public class BattleManager : MonoBehaviour
{
    // 单例
    public static BattleManager Instance { get; private set; }
    
    // 状态机
    private BattleStateMachine stateMachine;
    
    // 核心数据
    public Player player;
    public List<Enemy> enemies;
    public int currentTurn;
    
    // 卡牌系统
    private Deck deck;
    private Hand hand;
    private DiscardPile discardPile;
    
    // 能量系统
    public int currentEnergy;
    public int maxEnergy = 3;
    
    // 融合系统
    public int fusionPoints;
    public int maxFusionPoints = 10;
    
    // 核心方法
    public void StartBattle();
    public void StartPlayerTurn();
    public void EndPlayerTurn();
    public void StartEnemyTurn();
    public void EndBattle(bool victory);
    
    // 卡牌操作
    public void PlayCard(Card card, Target target);
    public void FuseCards(Card card1, Card card2);
    public void DrawCard(int count);
    
    // 事件
    public event Action<Card> OnCardPlayed;
    public event Action<Enemy> OnEnemyDefeated;
    public event Action<bool> OnBattleEnd;
}
```

**核心流程：**

```
StartBattle()
  ↓
StartPlayerTurn()
  ├─ ResetEnergy()
  ├─ DrawCards(5)
  └─ WaitForPlayerAction()
       ├─ PlayCard()
       ├─ FuseCards()
       └─ EndTurn()
            ↓
StartEnemyTurn()
  ├─ ExecuteEnemyActions()
  └─ EndEnemyTurn()
       ↓
CheckBattleEnd()
  ├─ AllEnemiesDefeated() → Victory
  └─ PlayerDefeated() → Defeat
```

### 3.2 卡牌系统

**Card.cs**

```csharp
[Serializable]
public class Card
{
    public string id;
    public string name;
    public CardType type; // Attack, Skill, Power
    public Rarity rarity; // Common, Rare, Epic, Legendary
    public int energyCost;
    public string description;
    public Sprite artwork;
    
    // 效果数据
    public List<CardEffect> effects;
    
    // 升级数据
    public int upgradeLevel;
    public bool canUpgrade;
    
    // 融合相关
    public bool isFusionCard;
    public string fusionRecipeId;
    
    // 方法
    public void Play(Target target);
    public void Upgrade();
    public Card Clone();
}

public enum CardType { Attack, Skill, Power, Fusion }
public enum Rarity { Common, Rare, Epic, Legendary }
```

**CardEffect.cs**

```csharp
[Serializable]
public abstract class CardEffect
{
    public abstract void Execute(Target target, BattleContext context);
}

// 具体效果示例
public class DamageEffect : CardEffect
{
    public int amount;
    public override void Execute(Target target, BattleContext context)
    {
        target.TakeDamage(amount);
    }
}

public class BlockEffect : CardEffect
{
    public int amount;
    public override void Execute(Target target, BattleContext context)
    {
        target.GainBlock(amount);
    }
}

public class DrawEffect : CardEffect
{
    public int amount;
    public override void Execute(Target target, BattleContext context)
    {
        BattleManager.Instance.DrawCard(amount);
    }
}
```

### 3.3 融合系统

**FusionManager.cs**

```csharp
public class FusionManager : MonoBehaviour
{
    // 融合配方数据库
    private Dictionary<string, FusionRecipe> recipes;
    
    // 融合方法
    public Card FuseCards(Card card1, Card card2)
    {
        // 1. 检查融合点
        if (BattleManager.Instance.fusionPoints < 3)
            return null;
        
        // 2. 查找配方
        string recipeKey = GetRecipeKey(card1.id, card2.id);
        FusionRecipe recipe = recipes.GetValueOrDefault(recipeKey);
        
        // 3. 生成融合卡牌
        Card fusedCard;
        if (recipe != null)
        {
            // 预设配方
            fusedCard = CreateCardFromRecipe(recipe);
        }
        else
        {
            // 随机融合
            fusedCard = CreateRandomFusionCard(card1, card2);
        }
        
        // 4. 消耗融合点
        BattleManager.Instance.fusionPoints -= 3;
        
        // 5. 触发事件
        OnCardsFused?.Invoke(card1, card2, fusedCard);
        
        return fusedCard;
    }
    
    private string GetRecipeKey(string id1, string id2)
    {
        // 确保顺序一致
        return id1.CompareTo(id2) < 0 ? $"{id1}+{id2}" : $"{id2}+{id1}";
    }
    
    private Card CreateRandomFusionCard(Card card1, Card card2)
    {
        // 随机融合逻辑
        // 合并效果、提升数值等
        // ...
    }
}

[Serializable]
public class FusionRecipe
{
    public string card1Id;
    public string card2Id;
    public string resultCardId;
    public string resultName;
    public List<CardEffect> resultEffects;
}
```

### 3.4 敌人AI系统

**Enemy.cs**

```csharp
public class Enemy : MonoBehaviour
{
    public string id;
    public string name;
    public int currentHP;
    public int maxHP;
    public int block;
    
    // AI行为
    public List<EnemyAction> actionPattern;
    private int currentActionIndex;
    
    // 方法
    public void TakeTurn()
    {
        EnemyAction action = GetNextAction();
        action.Execute(this);
        
        currentActionIndex++;
        if (currentActionIndex >= actionPattern.Count)
            currentActionIndex = 0;
    }
    
    public void TakeDamage(int amount);
    public void GainBlock(int amount);
    public void Die();
    
    // 显示意图
    public EnemyAction GetNextAction()
    {
        return actionPattern[currentActionIndex];
    }
}

[Serializable]
public abstract class EnemyAction
{
    public string actionName;
    public Sprite intentIcon;
    
    public abstract void Execute(Enemy self);
}

// 具体行为
public class AttackAction : EnemyAction
{
    public int damage;
    public override void Execute(Enemy self)
    {
        BattleManager.Instance.player.TakeDamage(damage);
    }
}

public class DefendAction : EnemyAction
{
    public int blockAmount;
    public override void Execute(Enemy self)
    {
        self.GainBlock(blockAmount);
    }
}
```

---

## 4. 数据设计

### 4.1 数据格式

**使用 JSON 存储所有配置数据**

**卡牌数据（cards.json）：**

```json
{
  "cards": [
    {
      "id": "rune_strike",
      "name": "符文打击",
      "type": "Attack",
      "rarity": "Common",
      "energyCost": 1,
      "description": "造成{damage}点伤害",
      "effects": [
        {
          "type": "Damage",
          "amount": 6
        }
      ],
      "upgrades": [
        {
          "level": 1,
          "changes": {
            "effects[0].amount": 9
          }
        }
      ]
    }
  ]
}
```

**敌人数据（enemies.json）：**

```json
{
  "enemies": [
    {
      "id": "rune_puppet",
      "name": "符文傀儡",
      "maxHP": 40,
      "actions": [
        {
          "type": "Attack",
          "damage": 6
        },
        {
          "type": "Defend",
          "block": 4
        }
      ]
    }
  ]
}
```

**融合配方（fusion_recipes.json）：**

```json
{
  "recipes": [
    {
      "card1": "fire_bolt",
      "card2": "frost_arrow",
      "result": "ice_fire_rune",
      "resultName": "冰火符文",
      "effects": [
        {
          "type": "Damage",
          "amount": 12
        },
        {
          "type": "Burn",
          "duration": 2
        }
      ]
    }
  ]
}
```

### 4.2 存档系统

**存档数据（save.json）：**

```json
{
  "playerName": "玩家名",
  "currentRun": {
    "character": "rune_mage",
    "floor": 5,
    "currentHP": 60,
    "maxHP": 70,
    "gold": 150,
    "deck": ["card_id1", "card_id2", ...],
    "relics": ["relic_id1", "relic_id2", ...],
    "mapState": {...}
  },
  "unlocks": {
    "characters": ["rune_mage", "rune_warrior"],
    "cards": ["..."],
    "relics": ["..."]
  },
  "statistics": {
    "totalRuns": 10,
    "victories": 3,
    "highestFloor": 15
  }
}
```

**SaveManager.cs:**

```csharp
public class SaveManager : MonoBehaviour
{
    private const string SAVE_FILE = "save.json";
    
    public void SaveGame(GameState state)
    {
        string json = JsonUtility.ToJson(state, true);
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
        File.WriteAllText(path, json);
    }
    
    public GameState LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameState>(json);
        }
        return null;
    }
}
```

---

## 5. 性能优化

### 5.1 对象池

**卡牌对象池：**
- 预生成卡牌 GameObject
- 复用而非销毁重建

```csharp
public class CardPool : MonoBehaviour
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    public GameObject cardPrefab;
    
    public GameObject GetCard()
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        else
            return Instantiate(cardPrefab);
    }
    
    public void ReturnCard(GameObject card)
    {
        card.SetActive(false);
        pool.Enqueue(card);
    }
}
```

### 5.2 资源加载

**使用 Addressables：**
- 异步加载资源
- 减少内存占用

```csharp
using UnityEngine.AddressableAssets;

public class ResourceLoader
{
    public async Task<Sprite> LoadCardArtwork(string cardId)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>($"Cards/{cardId}");
        return await handle.Task;
    }
}
```

### 5.3 UI优化

- 使用 Canvas 分层（静态/动态）
- 关闭不可见的 Canvas
- 使用 TextMeshPro（文本渲染优化）

---

## 6. 开发工具

### 6.1 调试工具

**作弊菜单（开发版）：**
```csharp
public class CheatMenu : MonoBehaviour
{
    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            if (GUILayout.Button("Add Card"))
                // 添加卡牌
            if (GUILayout.Button("Heal Full"))
                // 满血
            if (GUILayout.Button("Kill All Enemies"))
                // 秒杀敌人
        }
    }
}
```

### 6.2 测试框架

**Unity Test Framework：**
- 单元测试（卡牌效果、数值计算）
- 集成测试（战斗流程）

```csharp
[Test]
public void Test_DamageEffect()
{
    var card = new Card { energyCost = 1 };
    card.effects.Add(new DamageEffect { amount = 10 });
    
    var enemy = new Enemy { currentHP = 50 };
    card.Play(enemy);
    
    Assert.AreEqual(40, enemy.currentHP);
}
```

### 6.3 关卡编辑器

**自定义编辑器：**
- 可视化编辑卡牌
- 快速配置敌人
- 地图编辑器

---

## 7. 跨平台支持

### 7.1 目标平台

- ✅ Windows (64-bit)
- ✅ macOS (Intel + Apple Silicon)
- ✅ Linux (Ubuntu 20.04+)

### 7.2 输入系统

**Unity Input System：**
- 支持鼠标/键盘
- 支持手柄（可选）

```csharp
public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    
    void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.UI.Click.performed += OnClick;
    }
}
```

---

## 8. 后续技术规划

### 8.1 多语言支持

**本地化系统：**
- 使用 Unity Localization Package
- 支持：简体中文、英文
- 后续扩展：日文、韩文

### 8.2 云存档

**Steam Cloud：**
- 自动同步存档
- 跨设备游玩

### 8.3 Mod支持

**后期考虑：**
- Lua 脚本支持
- 自定义卡牌/敌人
- Steam Workshop 集成

---

**文档结束**

**下一步：**
- [ ] 搭建 Unity 项目
- [ ] 实现核心战斗系统
- [ ] 创建卡牌数据库
