# 可测试性设计文档 (Testability Design)

> **版本：** 1.0  
> **最后更新：** 2026-03-19  
> **负责人：** 架构师 + 测试架构师

---

## 📋 目标

**让代码"天生易于测试"，而非事后补救**

### 核心原则
1. **可观测性**：系统状态可查询、可验证
2. **可控制性**：可注入测试数据、可模拟边界条件
3. **可隔离性**：模块间松耦合，可独立测试
4. **可重复性**：相同输入产生相同输出

---

## 1. 架构级可测试性设计

### 1.1 依赖注入（Dependency Injection）

**问题：** 硬编码依赖导致难以测试

❌ **反例：**
```csharp
public class BattleManager
{
    private CardDatabase cardDB = new CardDatabase(); // 硬编码依赖
    
    public void DrawCard()
    {
        Card card = cardDB.GetRandomCard(); // 无法控制返回值
        hand.Add(card);
    }
}

// 测试时无法控制抽到哪张卡，难以验证逻辑
```

✅ **正例：**
```csharp
public class BattleManager
{
    private ICardDatabase cardDB; // 依赖接口
    
    // 构造函数注入
    public BattleManager(ICardDatabase cardDB)
    {
        this.cardDB = cardDB;
    }
    
    public void DrawCard()
    {
        Card card = cardDB.GetRandomCard();
        hand.Add(card);
    }
}

// 测试时可注入Mock对象
[Test]
public void Test_DrawCard()
{
    // Given: 注入假的数据库
    var mockDB = new MockCardDatabase();
    mockDB.SetNextCard(CardDatabase.GetCard("ATK_001")); // 控制返回值
    
    var battleMgr = new BattleManager(mockDB);
    
    // When: 抽牌
    battleMgr.DrawCard();
    
    // Then: 验证手牌包含指定卡牌
    Assert.Contains(CardDatabase.GetCard("ATK_001"), battleMgr.GetHand());
}
```

---

### 1.2 接口抽象（Interface Abstraction）

**为关键模块定义接口，便于 Mock**

```csharp
// 卡牌数据库接口
public interface ICardDatabase
{
    Card GetCard(string id);
    Card GetRandomCard(Rarity rarity);
    List<Card> GetAllCards();
}

// 敌人AI接口
public interface IEnemyAI
{
    EnemyAction DecideAction(BattleState state);
}

// 存档系统接口
public interface ISaveSystem
{
    void Save(GameData data);
    GameData Load();
    bool Exists();
}

// 随机数生成器接口（可控随机性）
public interface IRandomGenerator
{
    int Range(int min, int max);
    float Value(); // [0, 1)
}
```

**生产环境实现：**
```csharp
public class UnityRandomGenerator : IRandomGenerator
{
    public int Range(int min, int max) => UnityEngine.Random.Range(min, max);
    public float Value() => UnityEngine.Random.value;
}
```

**测试环境实现（可控随机）：**
```csharp
public class DeterministicRandomGenerator : IRandomGenerator
{
    private Queue<int> intValues = new Queue<int>();
    private Queue<float> floatValues = new Queue<float>();
    
    public void SetNextInt(int value) => intValues.Enqueue(value);
    public void SetNextFloat(float value) => floatValues.Enqueue(value);
    
    public int Range(int min, int max) => intValues.Dequeue();
    public float Value() => floatValues.Dequeue();
}

// 测试时可完全控制随机性
[Test]
public void Test_Random_Card_Draw()
{
    var rng = new DeterministicRandomGenerator();
    rng.SetNextInt(5); // 下次 Range(0, 10) 返回 5
    
    var cardMgr = new CardManager(rng);
    Card card = cardMgr.DrawRandomCard(); // 可预测结果
    
    Assert.AreEqual("ATK_005", card.id);
}
```

---

## 2. 测试接口设计

### 2.1 Debug 专用方法

**在 #if DEBUG 下暴露内部状态，便于测试**

```csharp
public class BattleManager : MonoBehaviour
{
    // 私有字段（生产代码不暴露）
    private Player player;
    private List<Enemy> enemies;
    private BattleState state;
    
    // ========== DEBUG 测试接口 ==========
    #if UNITY_EDITOR || DEBUG
    
    // 玩家状态控制
    public void DEBUG_SetPlayerHP(int hp) 
    { 
        player.currentHP = Mathf.Clamp(hp, 0, player.maxHP); 
    }
    
    public void DEBUG_SetPlayerEnergy(int energy) 
    { 
        player.currentEnergy = Mathf.Clamp(energy, 0, player.maxEnergy); 
    }
    
    public void DEBUG_AddCardToHand(string cardID)
    {
        Card card = CardDatabase.GetCard(cardID);
        player.hand.Add(card);
    }
    
    public void DEBUG_ClearHand()
    {
        player.hand.Clear();
    }
    
    // 敌人状态控制
    public void DEBUG_SetEnemyHP(int index, int hp)
    {
        if (index >= 0 && index < enemies.Count)
            enemies[index].currentHP = hp;
    }
    
    public void DEBUG_AddEnemyBuff(int index, BuffType buff, int stacks)
    {
        if (index >= 0 && index < enemies.Count)
            enemies[index].AddBuff(buff, stacks);
    }
    
    // 状态查询
    public BattleState DEBUG_GetState() => state;
    public Player DEBUG_GetPlayer() => player;
    public List<Enemy> DEBUG_GetEnemies() => new List<Enemy>(enemies); // 返回副本
    
    // 战斗流程控制
    public void DEBUG_ForceEndTurn()
    {
        EndTurn(); // 强制结束回合，跳过动画
    }
    
    public void DEBUG_SkipEnemyTurn()
    {
        state = BattleState.PlayerTurn; // 直接跳到玩家回合
    }
    
    #endif
}
```

**测试示例：**
```csharp
[UnityTest]
public IEnumerator Test_Player_Death()
{
    // Given: 玩家只剩1点HP
    BattleManager.StartBattle(new List<Enemy> { TestEnemy.CreateBasic() });
    yield return null;
    
    BattleManager.DEBUG_SetPlayerHP(1); // 设置濒死状态
    
    // When: 敌人攻击（伤害6）
    BattleManager.DEBUG_ForceEndTurn(); // 跳过玩家回合
    yield return new WaitForSeconds(0.5f);
    
    // Then: 玩家应该死亡
    Assert.AreEqual(BattleState.PlayerDefeated, BattleManager.DEBUG_GetState());
}
```

---

### 2.2 Debug Console（游戏内测试工具）

**快速测试卡牌效果，无需重启游戏**

```csharp
public class DebugConsole : MonoBehaviour
{
    private bool isOpen = false;
    private string command = "";
    
    void Update()
    {
        // ~ 键打开/关闭控制台
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            isOpen = !isOpen;
        }
    }
    
    void OnGUI()
    {
        if (!isOpen) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 500, 400));
        GUILayout.Label("=== Debug Console ===");
        
        command = GUILayout.TextField(command);
        
        if (GUILayout.Button("Execute") || Event.current.keyCode == KeyCode.Return)
        {
            ExecuteCommand(command);
            command = "";
        }
        
        GUILayout.EndArea();
    }
    
    void ExecuteCommand(string cmd)
    {
        var parts = cmd.Split(' ');
        
        switch (parts[0])
        {
            case "hp":
                BattleManager.DEBUG_SetPlayerHP(int.Parse(parts[1]));
                break;
            
            case "energy":
                BattleManager.DEBUG_SetPlayerEnergy(int.Parse(parts[1]));
                break;
            
            case "add":
                BattleManager.DEBUG_AddCardToHand(parts[1]); // 例如: add ATK_001
                break;
            
            case "enemy_hp":
                BattleManager.DEBUG_SetEnemyHP(int.Parse(parts[1]), int.Parse(parts[2]));
                break;
            
            case "kill":
                BattleManager.DEBUG_SetEnemyHP(int.Parse(parts[1]), 0);
                break;
            
            case "fuse":
                // 融合指定两张卡：fuse ATK_001 ATK_006
                Card c1 = CardDatabase.GetCard(parts[1]);
                Card c2 = CardDatabase.GetCard(parts[2]);
                Card fused = FusionSystem.Fuse(c1, c2);
                Debug.Log($"融合结果: {fused.name}");
                break;
            
            default:
                Debug.LogWarning($"Unknown command: {parts[0]}");
                break;
        }
    }
}
```

**使用示例：**
```
~ 键打开控制台
> hp 1          # 设置玩家HP为1（测试濒死）
> add ATK_001   # 添加火焰弹到手牌
> enemy_hp 0 10 # 设置敌人0的HP为10
> kill 1        # 直接杀死敌人1
> fuse ATK_001 ATK_006  # 测试火焰弹+冰霜箭融合
```

---

## 3. 数据驱动设计

### 3.1 JSON 配置 + 校验脚本

**所有数值配置外部化，便于调整和测试**

**卡牌配置示例：**
```json
{
  "id": "ATK_001",
  "name": "火焰弹",
  "energy": 1,
  "rarity": "普通",
  "type": "攻击",
  "damage": 8,
  "effects": []
}
```

**配置校验脚本（Python）：**
```python
import json

def validate_cards(json_path):
    with open(json_path, 'r', encoding='utf-8') as f:
        cards = json.load(f)
    
    errors = []
    
    # 1. 检查必填字段
    required_fields = ['id', 'name', 'energy', 'rarity', 'type']
    for card in cards:
        for field in required_fields:
            if field not in card:
                errors.append(f"卡牌 {card.get('name', '???')} 缺少字段: {field}")
    
    # 2. 检查数值合法性
    for card in cards:
        if card.get('energy', -1) < 0:
            errors.append(f"卡牌 {card['name']} 能量为负: {card['energy']}")
        
        if card.get('rarity') not in ['普通', '罕见', '史诗', '传说']:
            errors.append(f"卡牌 {card['name']} 稀有度非法: {card.get('rarity')}")
    
    # 3. 检查ID唯一性
    ids = [c['id'] for c in cards]
    if len(ids) != len(set(ids)):
        errors.append("存在重复的卡牌ID")
    
    # 4. 检查数值平衡（基于公式）
    for card in cards:
        if card['type'] == '攻击':
            expected = card['energy'] * 6.5
            actual = card.get('damage', 0)
            
            if abs(actual - expected) > expected * 0.2:
                errors.append(
                    f"卡牌 {card['name']} 数值失衡: "
                    f"期望 {expected}, 实际 {actual}"
                )
    
    return errors

if __name__ == '__main__':
    errors = validate_cards('Assets/Data/cards.json')
    
    if errors:
        print(f"发现 {len(errors)} 个错误:")
        for err in errors:
            print(f"  - {err}")
        exit(1)
    else:
        print("✅ 所有配置验证通过")
        exit(0)
```

**集成到 Git Hook：**
```bash
# .git/hooks/pre-commit
#!/bin/bash
python scripts/validate_cards.py
if [ $? -ne 0 ]; then
    echo "❌ 卡牌配置验证失败，请修复后再提交"
    exit 1
fi
```

---

### 3.2 数据版本管理

**卡牌配置加版本号，防止存档不兼容**

```json
{
  "version": "1.2",
  "cards": [
    { "id": "ATK_001", ... }
  ]
}
```

**加载时校验版本：**
```csharp
public class CardDatabase
{
    private const string CURRENT_VERSION = "1.2";
    
    public void Load(string jsonPath)
    {
        var data = JsonUtility.FromJson<CardData>(File.ReadAllText(jsonPath));
        
        if (data.version != CURRENT_VERSION)
        {
            Debug.LogWarning($"卡牌数据版本不匹配: {data.version} vs {CURRENT_VERSION}");
            // 尝试迁移或提示玩家更新
        }
        
        // ...
    }
}
```

---

## 4. 战斗重放系统

### 4.1 设计目标

**记录每回合状态，可复现任何bug**

### 4.2 实现方案

```csharp
public class BattleRecorder
{
    private List<BattleSnapshot> snapshots = new List<BattleSnapshot>();
    
    public void RecordSnapshot()
    {
        var snapshot = new BattleSnapshot
        {
            turn = BattleManager.CurrentTurn,
            playerHP = BattleManager.GetPlayerHP(),
            playerEnergy = BattleManager.GetPlayerEnergy(),
            hand = new List<Card>(BattleManager.GetHand()),
            enemies = BattleManager.GetEnemies().Select(e => new EnemySnapshot(e)).ToList(),
            timestamp = Time.time
        };
        
        snapshots.Add(snapshot);
    }
    
    public void SaveToFile(string path)
    {
        var json = JsonUtility.ToJson(new { snapshots }, prettyPrint: true);
        File.WriteAllText(path, json);
    }
    
    public void LoadFromFile(string path)
    {
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<BattleReplayData>(json);
        snapshots = data.snapshots;
    }
    
    public void Replay(int startTurn = 0)
    {
        // 从指定回合开始重放
        foreach (var snapshot in snapshots.Skip(startTurn))
        {
            // 恢复状态
            BattleManager.DEBUG_SetPlayerHP(snapshot.playerHP);
            BattleManager.DEBUG_SetPlayerEnergy(snapshot.playerEnergy);
            BattleManager.DEBUG_ClearHand();
            
            foreach (var card in snapshot.hand)
            {
                BattleManager.DEBUG_AddCardToHand(card.id);
            }
            
            // 恢复敌人状态
            for (int i = 0; i < snapshot.enemies.Count; i++)
            {
                BattleManager.DEBUG_SetEnemyHP(i, snapshot.enemies[i].hp);
            }
            
            // 暂停等待查看
            Debug.Log($"回合 {snapshot.turn} 状态已恢复");
            yield return new WaitForSeconds(1f);
        }
    }
}

[Serializable]
public class BattleSnapshot
{
    public int turn;
    public int playerHP;
    public int playerEnergy;
    public List<Card> hand;
    public List<EnemySnapshot> enemies;
    public float timestamp;
}

[Serializable]
public class EnemySnapshot
{
    public int hp;
    public int shield;
    public List<BuffData> buffs;
    
    public EnemySnapshot(Enemy enemy)
    {
        hp = enemy.currentHP;
        shield = enemy.currentShield;
        buffs = enemy.GetBuffs().Select(b => new BuffData(b)).ToList();
    }
}
```

**使用场景：**
```csharp
// 玩家报bug："第5回合敌人血量显示错误"
// 1. 玩家上传重放文件: bug_report_20260319.json
// 2. 开发者加载重放：
BattleRecorder.LoadFromFile("bug_report_20260319.json");
BattleRecorder.Replay(startTurn: 4); // 从第4回合开始重放

// 3. 逐帧调试，定位问题
```

---

## 5. 单元测试最佳实践

### 5.1 测试命名规范

```csharp
// 格式: Test_[被测方法]_[场景]_[期望结果]
[Test]
public void Test_CalculateDamage_WithShield_ReturnsReducedDamage()
{
    // Given: 敌人有10点护盾
    Enemy enemy = new Enemy(50, 10);
    
    // When: 火焰弹造成8点伤害
    int damage = BattleCalculator.CalculateDamage(8, enemy);
    
    // Then: 实际伤害为0（被护盾完全吸收）
    Assert.AreEqual(0, damage);
    Assert.AreEqual(2, enemy.currentShield); // 护盾剩余2点
}
```

### 5.2 Given-When-Then 结构

```csharp
[Test]
public void Test_Fusion_FireBall_IceArrow()
{
    // ===== Given（准备阶段）=====
    Card fireBall = CardDatabase.GetCard("ATK_001");
    Card iceArrow = CardDatabase.GetCard("ATK_006");
    
    // ===== When（执行阶段）=====
    Card fusedCard = FusionSystem.Fuse(fireBall, iceArrow);
    
    // ===== Then（验证阶段）=====
    Assert.AreEqual("冰火符文", fusedCard.name);
    Assert.AreEqual(14, fusedCard.GetDamage());
    Assert.IsTrue(fusedCard.HasEffect("灼烧"));
    Assert.IsTrue(fusedCard.HasEffect("冻结"));
}
```

### 5.3 边界测试

```csharp
[Test]
public void Test_DrawCard_WhenDeckEmpty_ShouldShuffleDi scardPile()
{
    // Given: 牌库只剩1张牌，弃牌堆有10张
    var cardMgr = new CardManager();
    cardMgr.DEBUG_SetDeckSize(1);
    cardMgr.DEBUG_SetDiscardPileSize(10);
    
    // When: 抽2张牌（触发重洗）
    cardMgr.DrawCard();
    cardMgr.DrawCard();
    
    // Then: 弃牌堆应该被洗回牌库
    Assert.Greater(cardMgr.GetDeckSize(), 0);
    Assert.AreEqual(0, cardMgr.GetDiscardPileSize());
}

[Test]
public void Test_PlayerDeath_WhenHPReachesZero()
{
    // Given: 玩家剩1点HP
    BattleManager.DEBUG_SetPlayerHP(1);
    
    // When: 受到10点伤害
    BattleManager.TakeDamage(10);
    
    // Then: 玩家应该死亡
    Assert.IsTrue(BattleManager.IsPlayerDead());
    Assert.AreEqual(BattleState.PlayerDefeated, BattleManager.GetState());
}
```

---

## 6. 性能测试工具

### 6.1 性能监控面板

```csharp
public class PerformanceMonitor : MonoBehaviour
{
    private float deltaTime = 0.0f;
    
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();
        
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;
        
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS", fps);
        
        GUI.Label(rect, text, style);
        
        // 内存占用
        rect.y += 30;
        long memory = System.GC.GetTotalMemory(false) / 1024 / 1024;
        text = $"Memory: {memory} MB";
        GUI.Label(rect, text, style);
    }
}
```

### 6.2 压力测试脚本

```csharp
[Test]
public void Stress_Test_1000_Fusions()
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    
    for (int i = 0; i < 1000; i++)
    {
        Card c1 = CardDatabase.GetRandomCard();
        Card c2 = CardDatabase.GetRandomCard();
        FusionSystem.Fuse(c1, c2);
    }
    
    sw.Stop();
    
    Debug.Log($"1000次融合耗时: {sw.ElapsedMilliseconds}ms");
    Assert.Less(sw.ElapsedMilliseconds, 1000, "融合性能不达标");
}
```

---

## 7. 持续集成（CI）

### 7.1 GitHub Actions 自动化测试

```yaml
name: Unity Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v2
      
      - name: Run Unity Tests
        uses: game-ci/unity-test-runner@v2
        with:
          unityVersion: 2022.3.15f1
          testMode: all
      
      - name: Validate Card Data
        run: python scripts/validate_cards.py
      
      - name: Upload Test Results
        uses: actions/upload-artifact@v2
        with:
          name: test-results
          path: test-results/
```

---

## 📊 可测试性检查清单

**在 Week 1 完成前，确保以下项全部 ✅：**

| 检查项 | 状态 | 备注 |
|-------|------|------|
| 核心模块使用依赖注入 | ⏳ | BattleManager, CardManager 等 |
| 定义关键接口（ICardDatabase等）| ⏳ | 至少5个接口 |
| 实现 DEBUG 测试接口 | ⏳ | 至少10个 DEBUG_ 方法 |
| 实现 Debug Console | ⏳ | 支持 hp, add, fuse 等命令 |
| 实现数据校验脚本 | ⏳ | Python 脚本 + Git Hook |
| 实现战斗重放系统 | ⏳ | 可保存/加载/重放 |
| 编写至少10个单元测试 | ⏳ | 覆盖核心逻辑 |
| 性能监控面板 | ⏳ | 显示FPS和内存 |

---

**文档版本历史：**
- v1.0 (2026-03-19) - 初始版本，完整可测试性设计
