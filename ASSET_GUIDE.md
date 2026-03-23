# 🎨 美术与音效资源指南

**项目**: 符文编年史  
**最后更新**: 2026-03-23

---

## 📁 资源文件夹结构

```
Assets/
├── Art/                        # 美术资源
│   ├── Cards/                  # 卡牌图片（125张）
│   │   ├── Card_ATK_001.png
│   │   ├── Card_ATK_002.png
│   │   └── ...
│   ├── Characters/             # 角色立绘（2个）
│   │   ├── Character_Mage.png
│   │   └── Character_Warrior.png
│   ├── Enemies/                # 敌人图片（21个）
│   │   ├── Enemy_001.png
│   │   ├── Enemy_002.png
│   │   └── ...
│   ├── UI/                     # UI资源
│   │   ├── Backgrounds/        # 背景图
│   │   │   ├── BG_MainMenu.png
│   │   │   ├── BG_Battle.png
│   │   │   ├── BG_Map.png
│   │   │   └── BG_Shop.png
│   │   └── Icons/              # 图标
│   │       ├── Icon_Heart.png
│   │       ├── Icon_Shield.png
│   │       ├── Icon_Energy.png
│   │       ├── Icon_Gold.png
│   │       └── Icon_FusionPoint.png
│   └── Effects/                # 特效（可选）
└── Audio/                      # 音频资源
    ├── BGM/                    # 背景音乐
    │   ├── MainMenu.ogg
    │   ├── Battle.ogg
    │   ├── Boss.ogg
    │   ├── Shop.ogg
    │   └── Victory.ogg
    └── SFX/                    # 音效
        ├── Card/               # 卡牌音效
        │   ├── card_draw.wav
        │   ├── card_play.wav
        │   ├── card_shuffle.wav
        │   └── card_fusion.wav
        ├── Battle/             # 战斗音效
        │   ├── attack_hit.wav
        │   ├── shield_gain.wav
        │   ├── damage_taken.wav
        │   ├── enemy_death.wav
        │   └── player_death.wav
        └── UI/                 # UI音效
            ├── button_click.wav
            ├── button_hover.wav
            ├── menu_open.wav
            ├── reward_get.wav
            ├── shop_buy.wav
            └── error.wav
```

---

## 🎨 美术资源需求

### 1. 卡牌图片（125张）

**规格**:
- 尺寸：200x280px（推荐）
- 格式：PNG（支持透明）
- 颜色：按稀有度分
  - 普通：灰白色 (0.9, 0.9, 0.9)
  - 稀有：蓝色 (0.7, 0.85, 1.0)
  - 史诗：紫色 (0.8, 0.6, 1.0)
  - 传说：金色 (1.0, 0.8, 0.3)

**内容**:
- 10张基础卡（ATK_001~ATK_005, SKL_001~SKL_003, PWR_001~PWR_002）
- 10张融合卡（FUS_001~FUS_010）
- 30张战士卡（WAR_001~WAR_030）
- 75张扩展卡（ATK_006~ATK_040, SKL_004~SKL_028, PWR_003~PWR_017）

**生成方式**:
1. **AI生成** - Midjourney, DALL-E, Stable Diffusion
   - Prompt示例："fantasy card game art, magic spell, fantasy style, detailed illustration"
2. **资产商店** - Unity Asset Store搜索"card game assets"
3. **外包美术** - Fiverr, Upwork（约$5-15/张）

### 2. 角色立绘（2个）

**规格**:
- 尺寸：300x400px
- 格式：PNG
- 风格：奇幻、魔法主题

**内容**:
- **Mage（法师）** - 蓝色法袍，手持法杖
- **Warrior（战士）** - 红色盔甲，手持剑盾

**生成方式**:
- AI生成：Midjourney, NovelAI
- Prompt示例：
  - "fantasy mage character, blue robes, staff, detailed character art"
  - "fantasy warrior character, red armor, sword and shield, detailed character art"

### 3. 敌人图片（21个）

**规格**:
- 尺寸：200x250px
- 格式：PNG
- 类型：
  - 10个普通怪（绿色调）
  - 5个精英怪（紫色调）
  - 3个BOSS（红色调，更大更威猛）

**内容建议**:
- 傀儡、元素、恶魔、不死生物等奇幻怪物

**生成方式**:
- AI生成：Midjourney, Stable Diffusion
- Prompt示例："fantasy monster, golem/demon/elemental, game art style"

### 4. UI资源

#### 背景图（4张）
- **BG_MainMenu.png** (1920x1080) - 深蓝色星空/城堡
- **BG_Battle.png** (1920x1080) - 战场/竞技场
- **BG_Map.png** (1920x1080) - 地图/冒险路径
- **BG_Shop.png** (1920x1080) - 商店/集市

#### 图标（5个）
- **Icon_Heart.png** (64x64) - 红心（生命）
- **Icon_Shield.png** (64x64) - 盾牌（护盾）
- **Icon_Energy.png** (64x64) - 闪电（能量）
- **Icon_Gold.png** (64x64) - 金币
- **Icon_FusionPoint.png** (64x64) - 紫色星星（融合点）

---

## 🔊 音效资源需求

### 1. 背景音乐（5首）

**规格**:
- 格式：OGG（Unity推荐）或 MP3
- 长度：2-3分钟（循环）
- 风格：奇幻、魔法主题

**内容**:
1. **MainMenu.ogg** - 轻快、神秘（探险感）
2. **Battle.ogg** - 紧张、激烈（战斗节奏）
3. **Boss.ogg** - 史诗、宏大（BOSS威压感）
4. **Shop.ogg** - 轻松、悠闲（购物氛围）
5. **Victory.ogg** - 欢快、胜利（庆祝感）

**获取方式**:
1. **免费音乐网站**:
   - [Incompetech](https://incompetech.com) - Kevin MacLeod免费音乐
   - [Free Music Archive](https://freemusicarchive.org)
   - [Bensound](https://www.bensound.com)

2. **Unity Asset Store**:
   - 搜索"fantasy music", "rpg music"
   - 价格：免费~$30

3. **AI生成**:
   - [Suno AI](https://suno.ai) - AI音乐生成
   - [AIVA](https://www.aiva.ai) - AI作曲工具

4. **委托作曲**:
   - Fiverr, Upwork
   - 价格：$50-200/首

### 2. 音效（15个）

**规格**:
- 格式：WAV（无损）
- 长度：0.5-2秒
- 音量：统一标准化

**卡牌音效**（4个）:
- `card_draw.wav` - 抽牌（快速翻动纸张声）
- `card_play.wav` - 打出卡牌（卡片飞出声）
- `card_shuffle.wav` - 洗牌（翻动声）
- `card_fusion.wav` - 融合成功（魔法闪光声）

**战斗音效**（5个）:
- `attack_hit.wav` - 攻击命中（剑击声）
- `shield_gain.wav` - 获得护盾（盾牌展开）
- `damage_taken.wav` - 受到伤害（痛苦声）
- `enemy_death.wav` - 敌人死亡（倒地声）
- `player_death.wav` - 玩家死亡（游戏结束音效）

**UI音效**（6个）:
- `button_click.wav` - 按钮点击（清脆点击）
- `button_hover.wav` - 按钮悬停（轻微提示音）
- `menu_open.wav` - 菜单打开（弹出声）
- `reward_get.wav` - 获得奖励（欢快音效）
- `shop_buy.wav` - 购买成功（金币声）
- `error.wav` - 错误提示（警告音）

**获取方式**:
1. **免费音效网站**:
   - [Freesound.org](https://freesound.org) - 需注册，海量免费
   - [Zapsplat.com](https://www.zapsplat.com) - 免费下载
   - [Mixkit](https://mixkit.co/free-sound-effects/) - 高质量免费

2. **Unity Asset Store**:
   - 搜索"UI sounds", "RPG sfx"
   - 很多免费包

3. **自己录制**:
   - 使用手机录音
   - Audacity编辑

4. **AI生成**:
   - [ElevenLabs](https://elevenlabs.io) - 音效生成（需付费）
   - [Soundraw](https://soundraw.io) - AI音乐/音效

---

## 🛠️ 使用资源的方式

### 方法1：自动生成占位符（已实现）

在Unity Editor中：
1. 菜单栏 → `Tools` → `Generate Placeholder Assets`
2. 自动生成所有占位符图片（纯色方块）
3. 后续直接替换真实资源即可

### 方法2：手动替换资源

1. **准备资源文件**
   - 按照上述命名规则准备文件
   - 例如：`Card_ATK_001.png`, `Character_Mage.png`

2. **放入对应文件夹**
   ```
   拖拽文件到Unity Project窗口的对应文件夹
   ```

3. **设置为Resources**
   - 将 `Art/` 和 `Audio/` 文件夹移到 `Assets/Resources/` 下
   - 或在现有位置创建 `Resources` 符号链接

4. **刷新Unity**
   - 右键 → `Reimport`
   - 或 `Ctrl+R` / `Cmd+R`

### 方法3：使用ResourceManager

游戏已内置 `ResourceManager.cs`，会自动加载Resources文件夹中的资源：

```csharp
// 获取卡牌图片
Sprite cardSprite = ResourceManager.Instance.GetCardSprite("ATK_001");

// 获取角色图片
Sprite charSprite = ResourceManager.Instance.GetCharacterSprite("Mage");

// 获取BGM
AudioClip bgm = ResourceManager.Instance.GetBGM("Battle");

// 播放BGM
AudioManager.Instance.PlayBGM(bgm);
```

---

## 📝 资源替换流程

### 快速替换流程（推荐）

1. **准备所有资源文件**
   - 按命名规则重命名
   - 检查格式和尺寸

2. **批量导入Unity**
   - 拖拽文件夹到 `Assets/Art/` 或 `Assets/Audio/`
   - Unity自动识别

3. **刷新资源**
   - `Ctrl+R` / `Cmd+R` 刷新
   - ResourceManager会自动重新加载

4. **测试游戏**
   - 点击Play查看效果
   - 检查是否有缺失资源

### 渐进式替换（推荐EA开发）

#### Phase 1：核心卡牌（10-20张）
- 先替换最常见的10-20张卡牌
- 测试加载和显示

#### Phase 2：角色和敌人
- 2个角色立绘
- 3-5个常见敌人
- 3个BOSS

#### Phase 3：UI美化
- 4个背景图
- 5个图标

#### Phase 4：音效音乐
- 2-3首核心BGM（主菜单、战斗）
- 5-10个核心音效（按钮、卡牌）

#### Phase 5：完整内容
- 剩余卡牌图片
- 剩余敌人图片
- 完整音效库

---

## 🎯 美术风格建议

### 整体风格
- **奇幻魔法主题** - 魔法、符文、元素
- **暗黑奇幻** - 偏暗色调，神秘感
- **手绘风格** - 类似《杀戮尖塔》、《欲火焚身》

### 配色方案
- **主色调**：深蓝、紫色（神秘感）
- **辅助色**：金色（珍贵）、红色（危险）
- **背景**：深色（不抢眼）

### 参考游戏
- **Slay the Spire** - 卡牌Roguelike标杆
- **Monster Train** - 精美卡牌美术
- **Inscryption** - 独特风格

---

## 💰 预算参考

### 低成本方案（$0-100）
- ✅ AI生成所有美术（Midjourney $10/月）
- ✅ 免费音效网站
- ✅ 免费BGM（CC授权音乐）
- **总成本**: $10-50

### 中等预算（$500-1000）
- ✅ AI生成 + 部分外包美术
- ✅ 购买音效包（Unity Asset Store）
- ✅ 委托2-3首原创BGM
- **总成本**: $500-800

### 高品质方案（$2000-5000）
- ✅ 全部外包美术（专业美术师）
- ✅ 原创音乐（专业作曲）
- ✅ 专业音效设计
- **总成本**: $2000-5000

---

## ✅ 检查清单

### 美术资源
- [ ] 125张卡牌图片
- [ ] 2个角色立绘
- [ ] 21个敌人图片
- [ ] 4个背景图
- [ ] 5个图标

### 音效资源
- [ ] 5首背景音乐
- [ ] 4个卡牌音效
- [ ] 5个战斗音效
- [ ] 6个UI音效

### Unity集成
- [ ] 所有资源放入正确文件夹
- [ ] ResourceManager可加载资源
- [ ] AudioManager可播放音乐
- [ ] UI显示正确图片
- [ ] 无报错

---

## 📚 延伸阅读

- [Unity Asset Store](https://assetstore.unity.com/)
- [Freesound](https://freesound.org/)
- [Midjourney](https://midjourney.com/)
- [Suno AI](https://suno.ai/)
- [游戏美术设计指南](https://docs.unity3d.com/Manual/Sprites.html)

---

**当前状态**: 🟡 占位符已生成，等待真实资源替换  
**优先级**: 中（不影响核心玩法）  
**预计工作量**: 2-3周（如使用AI生成+购买音效包）
