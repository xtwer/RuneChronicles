# 符文编年史 - 美术资源生成指南

> **目标**: 生成完整的中国风水墨美术资源
> 
> **总数**: 180+ 张图片

---

## 🎨 核心美术风格

### 风格定义
- **主风格**: 中国水墨画 (Chinese Ink Wash Painting)
- **辅助元素**: 符文、图腾、神秘光效
- **色调**: 黑白灰主调 + 彩色符文点缀
- **参考**: 《山海镜花》《太吾绘卷》

### 通用提示词模板
```
Chinese ink wash painting style, [subject], minimal composition, 
elegant brushstrokes, mystical runes, [color theme], 
fantasy art, high quality, detailed
```

---

## 📋 生成清单

### 1. 卡牌美术 (125张)

#### 1.1 攻击卡 (35张 - ExtendedAttackCards.json)

**基础火系** (ATK_001 - ATK_010):
```
Prompt: Chinese ink wash painting, fire spell, flame burst, 
red and orange tones, glowing embers, mystical runes, 
fantasy card art, vertical 2:3 ratio
```

**风系攻击** (ATK_011 - ATK_020):
```
Prompt: Chinese ink wash painting, wind blade, swirling air currents,
cyan and white tones, flowing movement, mystical symbols,
fantasy card art, vertical 2:3 ratio
```

**雷系攻击** (ATK_021 - ATK_030):
```
Prompt: Chinese ink wash painting, lightning strike, electric energy,
purple and blue tones, crackling bolts, ancient runes,
fantasy card art, vertical 2:3 ratio
```

**冰系攻击** (ATK_031 - ATK_035):
```
Prompt: Chinese ink wash painting, ice shard, frozen energy,
light blue and white tones, crystalline structures, frost runes,
fantasy card art, vertical 2:3 ratio
```

#### 1.2 技能卡 (25张 - ExtendedSkillCards.json)

**防御技能** (SKL_001 - SKL_010):
```
Prompt: Chinese ink wash painting, magical shield, protective barrier,
blue and silver tones, flowing defense, guardian runes,
fantasy card art, vertical 2:3 ratio
```

**治疗技能** (SKL_011 - SKL_015):
```
Prompt: Chinese ink wash painting, healing light, life energy,
green and gold tones, gentle glow, restoration runes,
fantasy card art, vertical 2:3 ratio
```

**辅助技能** (SKL_016 - SKL_025):
```
Prompt: Chinese ink wash painting, mystical utility, support magic,
purple and cyan tones, flowing energy, assistance runes,
fantasy card art, vertical 2:3 ratio
```

#### 1.3 能力卡 (15张 - ExtendedPowerCards.json)

**增益能力** (PWR_001 - PWR_008):
```
Prompt: Chinese ink wash painting, power aura, enhancement magic,
gold and red tones, radiating energy, empowerment runes,
fantasy card art, vertical 2:3 ratio
```

**特殊能力** (PWR_009 - PWR_015):
```
Prompt: Chinese ink wash painting, unique power, special ability,
multi-color tones, complex patterns, legendary runes,
fantasy card art, vertical 2:3 ratio
```

#### 1.4 融合卡 (10张 - FusionCards.json)

**融合卡** (FUSION_001 - FUSION_010):
```
Prompt: Chinese ink wash painting, fused magic, combined elements,
rainbow spectrum, swirling energies, ancient fusion runes,
epic power, fantasy card art, vertical 2:3 ratio
```

#### 1.5 战士卡 (30张 - WarriorCards.json)

**物理攻击** (WAR_ATK_001 - WAR_ATK_015):
```
Prompt: Chinese ink wash painting, sword strike, martial arts,
silver and steel tones, sharp weapons, warrior runes,
fantasy card art, vertical 2:3 ratio
```

**战士技能** (WAR_SKL_001 - WAR_SKL_010):
```
Prompt: Chinese ink wash painting, defensive stance, combat technique,
bronze and iron tones, battle formations, warrior symbols,
fantasy card art, vertical 2:3 ratio
```

**战士能力** (WAR_PWR_001 - WAR_PWR_005):
```
Prompt: Chinese ink wash painting, berserker rage, warrior power,
red and black tones, intense energy, battle runes,
fantasy card art, vertical 2:3 ratio
```

#### 1.6 基础卡 (10张 - BasicCards.json)

**入门卡** (BASIC_001 - BASIC_010):
```
Prompt: Chinese ink wash painting, simple spell, beginner magic,
soft colors, clean design, basic runes,
fantasy card art, vertical 2:3 ratio
```

---

### 2. 角色美术 (2张)

#### 2.1 符文法师
```
Prompt: Chinese ink wash painting style, wise mage character,
flowing robes with rune patterns, holding mystical staff,
long beard, serene expression, floating scrolls,
full body portrait, majestic pose, blue and purple tones,
fantasy art, high detail, vertical composition
```

**尺寸**: 1024x1024
**文件名**: `Mage_Portrait.png`

#### 2.2 符文战士
```
Prompt: Chinese ink wash painting style, warrior character,
armored in ancient battle gear with rune engravings,
dual wielding swords, determined expression, battle scars,
full body portrait, heroic stance, silver and red tones,
fantasy art, high detail, vertical composition
```

**尺寸**: 1024x1024
**文件名**: `Warrior_Portrait.png`

---

### 3. 敌人美术 (21张)

#### 3.1 普通敌人 (3张 - BasicEnemies.json)

**符文傀儡**:
```
Prompt: Chinese ink wash painting, stone golem with glowing runes,
rocky texture, ancient guardian, blue mystical glow,
menacing but elegant, fantasy creature art
```

**灵狐**:
```
Prompt: Chinese ink wash painting, mystical fox spirit,
ethereal form, flowing tail, cyan spiritual energy,
graceful and mysterious, fantasy creature art
```

**岩石巨人**:
```
Prompt: Chinese ink wash painting, rock giant, massive form,
stone body with moss, earth element, powerful stance,
intimidating presence, fantasy creature art
```

#### 3.2 扩展敌人 (18张 - ExtendedEnemies.json)

**类型多样化**:
```
- 元素生物 (火/冰/雷/风)
- 妖兽 (虎/狼/蛇/鹰)
- 亡灵 (骷髅/幽灵/僵尸)
- 精英怪 (ELITE)
- BOSS (3种)

基础Prompt: Chinese ink wash painting, [creature type],
[element/feature], threatening but artistic,
fantasy creature, [color theme], detailed design
```

---

### 4. UI美术

#### 4.1 背景图 (4张)

**主菜单背景**:
```
Prompt: Chinese landscape ink painting, mystical mountains in mist,
floating ancient runes, atmospheric perspective, monochrome with
subtle color accents, serene yet mysterious, wide landscape,
1920x1080, high quality
```

**战斗背景** (3种):
```
1. Forest: Chinese ink painting, misty forest, ancient trees,
   dappled light, peaceful yet mysterious

2. Mountain: Chinese ink painting, rocky mountain peak,
   swirling clouds, dramatic height, epic scale

3. Temple: Chinese ink painting, ancient temple ruins,
   stone pillars with runes, mystical atmosphere
```

#### 4.2 UI元素

**按钮**:
```
Prompt: Chinese style UI button, ink wash border,
rune decoration, elegant minimal design, PNG transparent
```

**图标**:
```
Prompt: Chinese ink style icon, [theme], simple symbol,
clear silhouette, minimal, PNG transparent
```

**卡牌边框** (4种稀有度):
```
Common: Simple ink brush border, gray tones
Rare: Ornate border with subtle runes, blue tones
Epic: Elaborate frame with glowing runes, purple tones
Legendary: Magnificent border with complex runes, gold tones
```

---

## 🛠️ 生成工具推荐

### AI图像生成工具
1. **Midjourney** (推荐)
   - 质量最高
   - 适合艺术风格
   - 需订阅

2. **Stable Diffusion** (免费)
   - 本地运行
   - 可调参数多
   - 需要GPU

3. **DALL-E 3** (通过ChatGPT Plus)
   - 易用
   - 中等质量
   - 需订阅

### 后期处理
- **Photoshop**: 调整、裁剪、添加边框
- **GIMP** (免费): 开源替代方案
- **Aseprite**: 像素化处理

---

## 📦 文件规范

### 命名规范
```
卡牌: Card_[ID].png
      例: Card_ATK_001.png

角色: [Class]_Portrait.png
      例: Mage_Portrait.png

敌人: Enemy_[Name].png
      例: Enemy_RuneGolem.png

背景: BG_[Scene].png
      例: BG_MainMenu.png

边框: Frame_[Rarity].png
      例: Frame_Common.png
```

### 尺寸标准
- 卡牌: 512x768 (2:3比例)
- 角色: 1024x1024 (正方形)
- 敌人: 512x512 (普通) / 1024x1024 (BOSS)
- 背景: 1920x1080 (16:9)
- UI元素: 根据用途调整

### 格式要求
- 格式: PNG (支持透明)
- 色彩: RGB (8位/通道)
- 压缩: 适度压缩保持质量

---

## 📊 生成进度追踪

| 类别 | 数量 | 完成 | 进度 |
|------|------|------|------|
| 攻击卡 | 35 | 0 | 0% |
| 技能卡 | 25 | 0 | 0% |
| 能力卡 | 15 | 0 | 0% |
| 融合卡 | 10 | 0 | 0% |
| 战士卡 | 30 | 0 | 0% |
| 基础卡 | 10 | 0 | 0% |
| 角色 | 2 | 0 | 0% |
| 敌人 | 21 | 0 | 0% |
| 背景 | 4 | 0 | 0% |
| UI元素 | 10+ | 0 | 0% |
| **总计** | **162+** | **0** | **0%** |

---

**最后更新**: 2026-03-23 22:15
