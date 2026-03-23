using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 资源管理器 - 统一管理所有美术和音效资源
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    
    private Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> characterSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> enemySprites = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> iconSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> backgroundSprites = new Dictionary<string, Sprite>();
    
    private Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllResources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void LoadAllResources()
    {
        Debug.Log("[ResourceManager] 开始加载资源...");
        
        LoadCardSprites();
        LoadCharacterSprites();
        LoadEnemySprites();
        LoadUISprites();
        LoadAudioClips();
        
        Debug.Log("[ResourceManager] 资源加载完成");
    }
    
    void LoadCardSprites()
    {
        // 从 Resources 或 Art 文件夹加载卡牌图片
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/Cards");
        
        foreach (var sprite in sprites)
        {
            // 文件名格式：Card_ATK_001.png → cardId = ATK_001
            string cardId = sprite.name.Replace("Card_", "");
            cardSprites[cardId] = sprite;
        }
        
        Debug.Log($"[ResourceManager] 加载 {cardSprites.Count} 张卡牌图片");
    }
    
    void LoadCharacterSprites()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/Characters");
        
        foreach (var sprite in sprites)
        {
            // 文件名格式：Character_Mage.png → Mage
            string characterName = sprite.name.Replace("Character_", "");
            characterSprites[characterName] = sprite;
        }
        
        Debug.Log($"[ResourceManager] 加载 {characterSprites.Count} 个角色图片");
    }
    
    void LoadEnemySprites()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/Enemies");
        
        foreach (var sprite in sprites)
        {
            // 文件名格式：Enemy_001.png → 001
            string enemyId = sprite.name.Replace("Enemy_", "");
            enemySprites[enemyId] = sprite;
        }
        
        Debug.Log($"[ResourceManager] 加载 {enemySprites.Count} 个敌人图片");
    }
    
    void LoadUISprites()
    {
        // 加载图标
        Sprite[] icons = Resources.LoadAll<Sprite>("Art/UI/Icons");
        foreach (var icon in icons)
        {
            string iconName = icon.name.Replace("Icon_", "");
            iconSprites[iconName] = icon;
        }
        
        // 加载背景
        Sprite[] backgrounds = Resources.LoadAll<Sprite>("Art/UI/Backgrounds");
        foreach (var bg in backgrounds)
        {
            string bgName = bg.name.Replace("BG_", "");
            backgroundSprites[bgName] = bg;
        }
        
        Debug.Log($"[ResourceManager] 加载 {iconSprites.Count} 个图标，{backgroundSprites.Count} 个背景");
    }
    
    void LoadAudioClips()
    {
        // 加载BGM
        AudioClip[] bgms = Resources.LoadAll<AudioClip>("Audio/BGM");
        foreach (var bgm in bgms)
        {
            bgmClips[bgm.name] = bgm;
        }
        
        // 加载SFX
        AudioClip[] sfxs = Resources.LoadAll<AudioClip>("Audio/SFX");
        foreach (var sfx in sfxs)
        {
            sfxClips[sfx.name] = sfx;
        }
        
        Debug.Log($"[ResourceManager] 加载 {bgmClips.Count} 个BGM，{sfxClips.Count} 个音效");
    }
    
    // === 获取资源的公共接口 ===
    
    public Sprite GetCardSprite(string cardId)
    {
        if (cardSprites.ContainsKey(cardId))
            return cardSprites[cardId];
        
        // 如果没找到，返回默认占位符
        return null;
    }
    
    public Sprite GetCharacterSprite(string characterName)
    {
        if (characterSprites.ContainsKey(characterName))
            return characterSprites[characterName];
        
        return null;
    }
    
    public Sprite GetEnemySprite(string enemyId)
    {
        if (enemySprites.ContainsKey(enemyId))
            return enemySprites[enemyId];
        
        return null;
    }
    
    public Sprite GetIconSprite(string iconName)
    {
        if (iconSprites.ContainsKey(iconName))
            return iconSprites[iconName];
        
        return null;
    }
    
    public Sprite GetBackgroundSprite(string bgName)
    {
        if (backgroundSprites.ContainsKey(bgName))
            return backgroundSprites[bgName];
        
        return null;
    }
    
    public AudioClip GetBGM(string bgmName)
    {
        if (bgmClips.ContainsKey(bgmName))
            return bgmClips[bgmName];
        
        return null;
    }
    
    public AudioClip GetSFX(string sfxName)
    {
        if (sfxClips.ContainsKey(sfxName))
            return sfxClips[sfxName];
        
        return null;
    }
    
    // === 占位符颜色（当没有图片时使用）===
    
    public Color GetCardPlaceholderColor(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common: return new Color(0.9f, 0.9f, 0.9f);
            case CardRarity.Rare: return new Color(0.7f, 0.85f, 1f);
            case CardRarity.Epic: return new Color(0.8f, 0.6f, 1f);
            case CardRarity.Legendary: return new Color(1f, 0.8f, 0.3f);
            default: return Color.white;
        }
    }
    
    public Color GetEnemyPlaceholderColor(string enemyType)
    {
        if (enemyType.Contains("BOSS"))
            return new Color(0.8f, 0.2f, 0.2f);
        if (enemyType.Contains("Elite") || enemyType.Contains("精英"))
            return new Color(0.6f, 0.3f, 0.6f);
        return new Color(0.3f, 0.6f, 0.3f);
    }
}
