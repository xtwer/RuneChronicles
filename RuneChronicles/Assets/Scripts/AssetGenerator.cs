using UnityEngine;
using System.IO;

/// <summary>
/// 资源生成器 - 生成占位符美术和音效资源
/// 使用方法：在Unity Editor菜单选择 Tools > Generate Placeholder Assets
/// </summary>
public class AssetGenerator : MonoBehaviour
{
    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Generate Placeholder Assets")]
    public static void GeneratePlaceholderAssets()
    {
        Debug.Log("[AssetGenerator] 开始生成占位符资源...");
        
        GenerateCardImages();
        GenerateCharacterImages();
        GenerateEnemyImages();
        GenerateUIAssets();
        GenerateAudioPlaceholders();
        
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("[AssetGenerator] 所有占位符资源已生成！");
    }
    
    static void GenerateCardImages()
    {
        string cardPath = "Assets/Art/Cards/";
        if (!Directory.Exists(cardPath))
            Directory.CreateDirectory(cardPath);
        
        // 生成125张卡牌占位图
        Color[] rarityColors = {
            new Color(0.9f, 0.9f, 0.9f), // Common
            new Color(0.7f, 0.85f, 1f),   // Rare
            new Color(0.8f, 0.6f, 1f),    // Epic
            new Color(1f, 0.8f, 0.3f)     // Legendary
        };
        
        if (CardManager.Instance != null)
        {
            var allCards = CardManager.Instance.GetAllCards();
            
            for (int i = 0; i < allCards.Count; i++)
            {
                var card = allCards[i];
                string filename = $"{cardPath}Card_{card.cardId}.png";
                
                // 如果文件已存在，跳过
                if (File.Exists(filename)) continue;
                
                // 生成占位图
                Texture2D texture = CreatePlaceholderTexture(200, 280, rarityColors[(int)card.rarity]);
                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(filename, bytes);
                
                Object.DestroyImmediate(texture);
            }
            
            Debug.Log($"[AssetGenerator] 生成 {allCards.Count} 张卡牌图片");
        }
    }
    
    static void GenerateCharacterImages()
    {
        string charPath = "Assets/Art/Characters/";
        if (!Directory.Exists(charPath))
            Directory.CreateDirectory(charPath);
        
        // 生成角色立绘
        string[] characters = { "Mage", "Warrior" };
        Color[] colors = {
            new Color(0.3f, 0.5f, 0.8f), // 法师蓝色
            new Color(0.8f, 0.3f, 0.3f)  // 战士红色
        };
        
        for (int i = 0; i < characters.Length; i++)
        {
            string filename = $"{charPath}Character_{characters[i]}.png";
            
            if (File.Exists(filename)) continue;
            
            Texture2D texture = CreatePlaceholderTexture(300, 400, colors[i]);
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(filename, bytes);
            
            Object.DestroyImmediate(texture);
        }
        
        Debug.Log("[AssetGenerator] 生成 2 个角色图片");
    }
    
    static void GenerateEnemyImages()
    {
        string enemyPath = "Assets/Art/Enemies/";
        if (!Directory.Exists(enemyPath))
            Directory.CreateDirectory(enemyPath);
        
        // 生成敌人图片
        // 普通怪：绿色，精英怪：紫色，BOSS：红色
        Color[] enemyColors = {
            new Color(0.3f, 0.6f, 0.3f), // 普通
            new Color(0.6f, 0.3f, 0.6f), // 精英
            new Color(0.8f, 0.2f, 0.2f)  // BOSS
        };
        
        // 生成21种敌人（简化版）
        for (int i = 1; i <= 21; i++)
        {
            string filename = $"{enemyPath}Enemy_{i:D3}.png";
            
            if (File.Exists(filename)) continue;
            
            // 根据编号选择颜色
            int colorIndex = i <= 10 ? 0 : (i <= 18 ? 1 : 2);
            
            Texture2D texture = CreatePlaceholderTexture(200, 250, enemyColors[colorIndex]);
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(filename, bytes);
            
            Object.DestroyImmediate(texture);
        }
        
        Debug.Log("[AssetGenerator] 生成 21 个敌人图片");
    }
    
    static void GenerateUIAssets()
    {
        // 生成UI背景
        string bgPath = "Assets/Art/UI/Backgrounds/";
        if (!Directory.Exists(bgPath))
            Directory.CreateDirectory(bgPath);
        
        string[] backgrounds = { "MainMenu", "Battle", "Map", "Shop" };
        Color[] bgColors = {
            new Color(0.1f, 0.1f, 0.15f),
            new Color(0.15f, 0.15f, 0.2f),
            new Color(0.15f, 0.2f, 0.25f),
            new Color(0.15f, 0.2f, 0.25f)
        };
        
        for (int i = 0; i < backgrounds.Length; i++)
        {
            string filename = $"{bgPath}BG_{backgrounds[i]}.png";
            
            if (File.Exists(filename)) continue;
            
            Texture2D texture = CreatePlaceholderTexture(1920, 1080, bgColors[i]);
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(filename, bytes);
            
            Object.DestroyImmediate(texture);
        }
        
        // 生成UI图标
        string iconPath = "Assets/Art/UI/Icons/";
        if (!Directory.Exists(iconPath))
            Directory.CreateDirectory(iconPath);
        
        string[] icons = { "Heart", "Shield", "Energy", "Gold", "FusionPoint" };
        Color[] iconColors = {
            Color.red,
            Color.cyan,
            Color.yellow,
            new Color(1f, 0.8f, 0.3f),
            new Color(1f, 0.6f, 1f)
        };
        
        for (int i = 0; i < icons.Length; i++)
        {
            string filename = $"{iconPath}Icon_{icons[i]}.png";
            
            if (File.Exists(filename)) continue;
            
            Texture2D texture = CreatePlaceholderTexture(64, 64, iconColors[i]);
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(filename, bytes);
            
            Object.DestroyImmediate(texture);
        }
        
        Debug.Log("[AssetGenerator] 生成 UI 资源");
    }
    
    static void GenerateAudioPlaceholders()
    {
        // 创建音效说明文件
        string sfxPath = "Assets/Audio/SFX/";
        string bgmPath = "Assets/Audio/BGM/";
        
        string sfxReadme = @"# 音效资源占位

## 需要的音效

### 卡牌音效 (Card)
- card_draw.wav - 抽牌音效
- card_play.wav - 打出卡牌
- card_shuffle.wav - 洗牌
- card_fusion.wav - 融合成功

### 战斗音效 (Battle)
- attack_hit.wav - 攻击命中
- shield_gain.wav - 获得护盾
- damage_taken.wav - 受到伤害
- enemy_death.wav - 敌人死亡
- player_death.wav - 玩家死亡

### UI音效 (UI)
- button_click.wav - 按钮点击
- button_hover.wav - 按钮悬停
- menu_open.wav - 菜单打开
- reward_get.wav - 获得奖励
- shop_buy.wav - 购买成功
- error.wav - 错误提示

## 获取方式
1. 免费资源网站：freesound.org, zapsplat.com
2. Unity Asset Store
3. 自己录制
4. AI生成（如ElevenLabs）
";
        
        File.WriteAllText($"{sfxPath}README.md", sfxReadme);
        
        string bgmReadme = @"# 背景音乐占位

## 需要的BGM

1. **MainMenu.ogg** - 主菜单音乐（轻快、神秘）
2. **Battle.ogg** - 战斗音乐（紧张、激烈）
3. **Boss.ogg** - BOSS战音乐（史诗、宏大）
4. **Shop.ogg** - 商店音乐（轻松、悠闲）
5. **Victory.ogg** - 胜利音乐（欢快、胜利）

## 音乐风格
- 奇幻风格
- 循环播放
- 长度：2-3分钟
- 格式：OGG（Unity推荐）

## 获取方式
1. 免费音乐网站：incompetech.com, freemusicarchive.org
2. Unity Asset Store
3. 委托作曲
4. AI生成（如Suno AI）
";
        
        File.WriteAllText($"{bgmPath}README.md", bgmReadme);
        
        Debug.Log("[AssetGenerator] 创建音效说明文件");
    }
    
    static Texture2D CreatePlaceholderTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return texture;
    }
    #endif
}
