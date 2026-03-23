# 音效资源占位符

## 背景音乐 (BGM)

**已创建文件夹**: `Assets/Audio/BGM/`

### 待添加内容
- [ ] MainMenu.mp3 - 主菜单音乐（轻松、神秘）
- [ ] Battle.mp3 - 战斗音乐（紧张、节奏感）
- [ ] Boss.mp3 - BOSS战音乐（史诗、激昂）
- [ ] Shop.mp3 - 商店音乐（平静、温暖）

### 建议来源
1. **免费音乐库**:
   - [FreePD](https://freepd.com/)
   - [Incompetech](https://incompetech.com/)
   - [Purple Planet](https://www.purple-planet.com/)

2. **AI生成**:
   - [Suno AI](https://suno.ai/)
   - [AIVA](https://www.aiva.ai/)

3. **外包**（预算$500-1000）:
   - Fiverr
   - Upwork

### 音乐参数
- **格式**: MP3 或 OGG
- **比特率**: 128-192 kbps
- **循环**: 无缝循环
- **时长**: 2-4分钟

---

## 音效 (SFX)

**已创建文件夹**: `Assets/Audio/SFX/`

### 待添加内容

#### 卡牌音效
- [ ] card_draw.wav - 抽牌
- [ ] card_play.wav - 打出卡牌
- [ ] card_shuffle.wav - 洗牌
- [ ] card_discard.wav - 弃牌

#### 融合音效
- [ ] fusion_start.wav - 融合开始
- [ ] fusion_complete.wav - 融合完成
- [ ] fusion_fail.wav - 融合失败（可选）

#### 战斗音效
- [ ] attack_hit.wav - 攻击命中
- [ ] block.wav - 格挡
- [ ] damage.wav - 受伤
- [ ] heal.wav - 治疗
- [ ] death.wav - 死亡

#### UI音效
- [ ] button_click.wav - 按钮点击
- [ ] button_hover.wav - 按钮悬停
- [ ] page_turn.wav - 翻页
- [ ] notification.wav - 通知

#### 特殊音效
- [ ] victory.wav - 胜利
- [ ] defeat.wav - 失败
- [ ] level_up.wav - 升级（可选）
- [ ] rare_card.wav - 获得稀有卡

### 建议来源
1. **免费音效库**:
   - [Freesound](https://freesound.org/)
   - [Zapsplat](https://www.zapsplat.com/)
   - [Mixkit](https://mixkit.co/free-sound-effects/)

2. **Unity Asset Store**:
   - 搜索 "Card Game SFX" / "UI Sound Pack"

### 音效参数
- **格式**: WAV
- **采样率**: 44.1kHz
- **位深度**: 16-bit
- **时长**: 0.1-2秒

---

## 音频管理器

### 功能需求
- [x] 音量控制（BGM/SFX分离）
- [x] 淡入淡出
- [x] 音效池（避免重复音效冲突）
- [ ] 实际音频文件（占位符）

### AudioManager.cs (示例)
```csharp
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    
    [Header("BGM Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip battleBGM;
    public AudioClip bossBGM;
    
    [Header("SFX Clips")]
    public AudioClip cardDrawSFX;
    public AudioClip cardPlaySFX;
    public AudioClip attackSFX;
    
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
    
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }
    
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
```

---

## 当前状态

**所有音效为占位符，游戏静音但功能完整**

可以使用Unity自带的音效测试，或直接跳过音效验证核心玩法。

---

**最后更新**: 2026-03-23 15:15
