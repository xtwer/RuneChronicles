# 最小可玩原型 (MVP) 开发指南

> **目标：** 2-3天内做出可玩的融合系统原型  
> **范围：** 2张卡 + 1次融合 + 1个敌人

---

## 🎯 原型目标

**验证核心问题：** 融合系统好不好玩？

**最小功能：**
- ✅ 可以打出卡牌攻击敌人
- ✅ 可以选择2张卡融合
- ✅ 融合后生成新卡
- ✅ 打出融合卡可以看到效果
- ✅ 敌人死亡显示胜利

**不需要：**
- ❌ 美术（用白色方块）
- ❌ 动画（瞬间生效）
- ❌ 音效
- ❌ 存档
- ❌ 地图
- ❌ 事件

---

## 📋 开发步骤

### Day 1：基础框架（4-6小时）

#### 1.1 创建文件夹结构
```
Assets/
├── Scripts/
│   ├── Core/           # 核心管理器
│   ├── Battle/         # 战斗系统
│   ├── Card/           # 卡牌系统
│   └── Data/           # 数据类
├── Scenes/
│   └── BattleScene.unity
├── Prefabs/
│   ├── Card.prefab
│   └── Enemy.prefab
└── Data/
    └── cards.json
```

#### 1.2 创建核心脚本

**GameManager.cs**（单例）
```csharp
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
```

**Card.cs**（卡牌数据）
```csharp
using System;

[Serializable]
public class Card
{
    public string id;
    public string name;
    public int energy;
    public int damage;
    public bool hasFrozen;  // 是否有冻结效果
    
    public Card(string id, string name, int energy, int damage, bool hasFrozen = false)
    {
        this.id = id;
        this.name = name;
        this.energy = energy;
        this.damage = damage;
        this.hasFrozen = hasFrozen;
    }
}
```

**Enemy.cs**（敌人）
```csharp
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHP = 40;
    public int currentHP;
    public Text hpText;  // 显示血量
    
    void Start()
    {
        currentHP = maxHP;
        UpdateHPDisplay();
    }
    
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        
        UpdateHPDisplay();
        
        if (currentHP == 0)
        {
            Die();
        }
    }
    
    void UpdateHPDisplay()
    {
        if (hpText != null)
            hpText.text = $"HP: {currentHP}/{maxHP}";
    }
    
    void Die()
    {
        Debug.Log("敌人死亡！");
        // TODO: 显示胜利界面
        gameObject.SetActive(false);
    }
}
```

---

### Day 2：卡牌系统（4-6小时）

#### 2.1 CardManager.cs（卡牌管理）
```csharp
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    
    public List<Card> hand = new List<Card>();  // 手牌
    public Transform handTransform;  // 手牌显示位置
    public GameObject cardPrefab;    // 卡牌预制体
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        // 初始手牌：2张卡
        AddCardToHand(new Card("ATK_001", "火焰弹", 1, 8));
        AddCardToHand(new Card("ATK_006", "冰霜箭", 1, 6, hasFrozen: true));
    }
    
    public void AddCardToHand(Card card)
    {
        hand.Add(card);
        
        // 创建卡牌UI
        GameObject cardObj = Instantiate(cardPrefab, handTransform);
        CardUI cardUI = cardObj.GetComponent<CardUI>();
        cardUI.SetCard(card);
    }
    
    public void RemoveCardFromHand(Card card)
    {
        hand.Remove(card);
    }
}
```

#### 2.2 CardUI.cs（卡牌UI）
```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    public Card card;
    public Text nameText;
    public Text damageText;
    
    public void SetCard(Card card)
    {
        this.card = card;
        nameText.text = card.name;
        damageText.text = $"伤害: {card.damage}";
        
        if (card.hasFrozen)
            damageText.text += "\n冻结";
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // 点击卡牌 → 打出或选择融合
        BattleManager.Instance.OnCardClicked(card);
    }
}
```

---

### Day 3：融合系统 + 战斗（4-6小时）

#### 3.1 BattleManager.cs（战斗管理）
```csharp
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    
    public Enemy enemy;
    public List<Card> selectedCards = new List<Card>();  // 选中的卡牌（用于融合）
    
    void Awake()
    {
        Instance = this;
    }
    
    public void OnCardClicked(Card card)
    {
        // 如果已选中1张卡，则进行融合
        if (selectedCards.Count == 1)
        {
            selectedCards.Add(card);
            FuseCards();
        }
        // 如果未选中卡，则选中这张卡
        else if (selectedCards.Count == 0)
        {
            selectedCards.Add(card);
            Debug.Log($"选中卡牌: {card.name}，再点击一张卡进行融合");
        }
    }
    
    void FuseCards()
    {
        Card card1 = selectedCards[0];
        Card card2 = selectedCards[1];
        
        Debug.Log($"融合: {card1.name} + {card2.name}");
        
        // 融合逻辑：火焰弹 + 冰霜箭 = 冰火符文
        if ((card1.id == "ATK_001" && card2.id == "ATK_006") ||
            (card1.id == "ATK_006" && card2.id == "ATK_001"))
        {
            // 移除原卡
            CardManager.Instance.RemoveCardFromHand(card1);
            CardManager.Instance.RemoveCardFromHand(card2);
            
            // 生成融合卡
            Card fusedCard = new Card("FUSED_001", "冰火符文", 2, 14, hasFrozen: true);
            CardManager.Instance.AddCardToHand(fusedCard);
            
            Debug.Log("融合成功！生成：冰火符文");
        }
        
        selectedCards.Clear();
    }
    
    public void PlayCard(Card card)
    {
        // 打出卡牌攻击敌人
        Debug.Log($"打出卡牌: {card.name}，造成 {card.damage} 点伤害");
        
        enemy.TakeDamage(card.damage);
        
        if (card.hasFrozen)
        {
            Debug.Log("敌人被冻结！");
        }
        
        CardManager.Instance.RemoveCardFromHand(card);
    }
}
```

---

## 🎨 UI 布局（极简版）

### Scene 布局
```
Canvas
├── HandPanel (手牌区域)
│   └── Card Prefab (动态生成)
├── EnemyPanel (敌人区域)
│   ├── EnemySprite (白色方块)
│   └── HPText (血量显示)
└── InfoText (提示信息)
```

### Card Prefab 结构
```
Card (Image - 白色方块)
├── NameText (卡牌名称)
└── DamageText (伤害/效果)
```

---

## ✅ 验收标准

**完成后应该能：**
1. [ ] 看到2张卡在手牌区域
2. [ ] 点击第1张卡 → 显示"选中"
3. [ ] 点击第2张卡 → 自动融合 → 生成新卡
4. [ ] 点击融合卡 → 敌人扣血 → 显示冻结
5. [ ] 敌人血量归零 → 显示"胜利"

**测试流程：**
```
1. 运行游戏
2. 点击"火焰弹" → 控制台显示"选中卡牌"
3. 点击"冰霜箭" → 控制台显示"融合成功"
4. 手牌出现"冰火符文"
5. 点击"冰火符文" → 敌人HP: 40 → 26
6. 再次融合+打出 → 敌人HP: 26 → 12
7. 再次融合+打出 → 敌人HP: 12 → 0 → 死亡
```

---

## 🚀 快速开始

### 1. 创建场景
1. Unity → File → New Scene
2. 保存为 `BattleScene.unity`

### 2. 创建脚本
1. 复制上面的代码到对应文件
2. 挂载到场景对象上

### 3. 运行测试
1. 点击 Play 按钮
2. 按照测试流程验证

---

## 📝 开发日志

**记录每天进度：**

### Day 1 (YYYY-MM-DD)
- [ ] 创建文件夹结构
- [ ] 实现 GameManager
- [ ] 实现 Card 数据类
- [ ] 实现 Enemy 基础功能

### Day 2 (YYYY-MM-DD)
- [ ] 实现 CardManager
- [ ] 实现 CardUI
- [ ] 手牌显示正常

### Day 3 (YYYY-MM-DD)
- [ ] 实现 BattleManager
- [ ] 实现融合逻辑
- [ ] 完整流程测试通过

---

## 🎉 完成后

**如果原型验证成功（融合系统有趣）：**
- ✅ 继续扩展（10张卡 + 3个敌人）
- ✅ 添加美术
- ✅ 完善战斗流程

**如果原型验证失败（融合系统不好玩）：**
- ⚠️ 重新设计融合系统
- ⚠️ 或考虑其他核心玩法

---

**预计时间：** 2-3天（每天4-6小时）

**下一步：** 开始 Day 1 开发！🚀
