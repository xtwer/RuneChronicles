using UnityEngine;

/// <summary>
/// 音频管理器
/// Week 10: 音效与音乐系统
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    private AudioSource bgmSource;
    private AudioSource sfxSource;
    
    [Header("音量设置")]
    [Range(0f, 1f)] public float bgmVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;
    
    [Header("BGM状态")]
    private AudioClip currentBGM;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 创建AudioSource组件
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;
            
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBGM(AudioClip clip, bool forceRestart = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] BGM为null（占位符）");
            return;
        }
        
        // 如果已经在播放相同的音乐，不重新播放
        if (bgmSource.clip == clip && bgmSource.isPlaying && !forceRestart)
        {
            return;
        }
        
        currentBGM = clip;
        bgmSource.clip = clip;
        bgmSource.Play();
        
        Debug.Log($"[AudioManager] 播放BGM: {clip.name}");
    }
    
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
        currentBGM = null;
    }
    
    /// <summary>
    /// 暂停/恢复背景音乐
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
        else
        {
            bgmSource.UnPause();
        }
    }
    
    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] SFX为null（占位符）");
            return;
        }
        
        sfxSource.PlayOneShot(clip);
    }
    
    /// <summary>
    /// 设置BGM音量
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }
    
    /// <summary>
    /// 设置SFX音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }
    
    /// <summary>
    /// 淡入BGM
    /// </summary>
    public void FadeInBGM(AudioClip clip, float duration = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] BGM为null（占位符）");
            return;
        }
        
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(clip, duration));
    }
    
    /// <summary>
    /// 淡出BGM
    /// </summary>
    public void FadeOutBGM(float duration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(duration));
    }
    
    private System.Collections.IEnumerator FadeInCoroutine(AudioClip clip, float duration)
    {
        currentBGM = clip;
        bgmSource.clip = clip;
        bgmSource.volume = 0f;
        bgmSource.Play();
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, elapsed / duration);
            yield return null;
        }
        
        bgmSource.volume = bgmVolume;
    }
    
    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }
        
        bgmSource.Stop();
        bgmSource.volume = bgmVolume;
        currentBGM = null;
    }
    
    #region 占位符音效函数（暂时无音频文件）
    
    /// <summary>
    /// 播放卡牌抽取音效（占位符）
    /// </summary>
    public void PlayCardDrawSFX()
    {
        Debug.Log("[AudioManager] 播放：抽牌音效（占位符）");
        // TODO: PlaySFX(cardDrawClip);
    }
    
    /// <summary>
    /// 播放卡牌打出音效（占位符）
    /// </summary>
    public void PlayCardPlaySFX()
    {
        Debug.Log("[AudioManager] 播放：出牌音效（占位符）");
        // TODO: PlaySFX(cardPlayClip);
    }
    
    /// <summary>
    /// 播放攻击音效（占位符）
    /// </summary>
    public void PlayAttackSFX()
    {
        Debug.Log("[AudioManager] 播放：攻击音效（占位符）");
        // TODO: PlaySFX(attackClip);
    }
    
    /// <summary>
    /// 播放融合音效（占位符）
    /// </summary>
    public void PlayFusionSFX()
    {
        Debug.Log("[AudioManager] 播放：融合音效（占位符）");
        // TODO: PlaySFX(fusionClip);
    }
    
    /// <summary>
    /// 播放按钮点击音效（占位符）
    /// </summary>
    public void PlayButtonClickSFX()
    {
        Debug.Log("[AudioManager] 播放：按钮音效（占位符）");
        // TODO: PlaySFX(buttonClickClip);
    }
    
    #endregion
}
