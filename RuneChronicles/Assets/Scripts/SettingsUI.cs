using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// 设置UI - 音量、画质、全屏等
/// </summary>
public class SettingsUI : MonoBehaviour
{
    private Canvas canvas;
    private GameObject settingsPanel;
    
    // 音量设置
    private Slider bgmSlider;
    private Slider sfxSlider;
    
    // 显示设置
    private Toggle fullscreenToggle;
    private Dropdown qualityDropdown;
    private Dropdown resolutionDropdown;
    
    void Start()
    {
        CreateSettingsUI();
        LoadSettings();
    }
    
    void CreateSettingsUI()
    {
        // 查找或创建Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("SettingsCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // 最顶层
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // 半透明背景
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvas.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
        
        // 设置面板
        settingsPanel = new GameObject("SettingsPanel");
        settingsPanel.transform.SetParent(canvas.transform, false);
        var panelRect = settingsPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.25f, 0.15f);
        panelRect.anchorMax = new Vector2(0.75f, 0.85f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        var panelImage = settingsPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        
        // 标题
        CreateTitle();
        
        // 音量设置
        CreateVolumeSettings();
        
        // 显示设置
        CreateDisplaySettings();
        
        // 按钮
        CreateButtons();
        
        Debug.Log("[SettingsUI] 设置界面已创建");
    }
    
    void CreateTitle()
    {
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(settingsPanel.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        var titleText = ChineseUI.CreateTitle(titleObj, "游戏设置");
        titleText.fontSize = 48;
    }
    
    void CreateVolumeSettings()
    {
        float yPos = 0.7f;
        
        // BGM音量
        CreateSlider("BGM音量", yPos, (value) => {
            SetBGMVolume(value);
        }, out bgmSlider);
        
        // SFX音量
        CreateSlider("音效音量", yPos - 0.15f, (value) => {
            SetSFXVolume(value);
        }, out sfxSlider);
    }
    
    void CreateDisplaySettings()
    {
        float yPos = 0.4f;
        
        // 全屏开关
        CreateToggle("全屏模式", yPos, (value) => {
            Screen.fullScreen = value;
            PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
            PlayerPrefs.Save();
        }, out fullscreenToggle);
    }
    
    void CreateSlider(string label, float yPos, UnityEngine.Events.UnityAction<float> onValueChanged, out Slider slider)
    {
        var sliderObj = new GameObject($"Slider_{label}");
        sliderObj.transform.SetParent(settingsPanel.transform, false);
        
        var rect = sliderObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, yPos - 0.05f);
        rect.anchorMax = new Vector2(0.9f, yPos + 0.05f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        // 标签
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(sliderObj.transform, false);
        var labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0.3f, 1);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(labelObj, label, 28, TextAnchor.MiddleLeft, Color.white);
        
        // 滑块
        var sliderBgObj = new GameObject("SliderBg");
        sliderBgObj.transform.SetParent(sliderObj.transform, false);
        var sliderBgRect = sliderBgObj.AddComponent<RectTransform>();
        sliderBgRect.anchorMin = new Vector2(0.35f, 0.3f);
        sliderBgRect.anchorMax = new Vector2(0.95f, 0.7f);
        sliderBgRect.offsetMin = Vector2.zero;
        sliderBgRect.offsetMax = Vector2.zero;
        
        slider = sliderBgObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.onValueChanged.AddListener(onValueChanged);
        
        // Background
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderBgObj.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f);
        
        // Fill Area
        var fillAreaObj = new GameObject("Fill Area");
        fillAreaObj.transform.SetParent(sliderBgObj.transform, false);
        var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        
        var fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillAreaObj.transform, false);
        var fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        var fillImage = fillObj.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.6f, 1f);
        slider.fillRect = fillRect;
        
        // Handle
        var handleAreaObj = new GameObject("Handle Slide Area");
        handleAreaObj.transform.SetParent(sliderBgObj.transform, false);
        var handleAreaRect = handleAreaObj.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = Vector2.zero;
        handleAreaRect.offsetMax = Vector2.zero;
        
        var handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(handleAreaObj.transform, false);
        var handleRect = handleObj.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 0);
        var handleImage = handleObj.AddComponent<Image>();
        handleImage.color = Color.white;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        
        // 数值显示
        var valueObj = new GameObject("Value");
        valueObj.transform.SetParent(sliderObj.transform, false);
        var valueRect = valueObj.AddComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(0.96f, 0);
        valueRect.anchorMax = new Vector2(1.05f, 1);
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;
        var valueText = ChineseUI.CreateText(valueObj, "100%", 24, TextAnchor.MiddleLeft, Color.white);
        
        slider.onValueChanged.AddListener((value) => {
            valueText.text = Mathf.RoundToInt(value * 100) + "%";
        });
    }
    
    void CreateToggle(string label, float yPos, UnityEngine.Events.UnityAction<bool> onValueChanged, out Toggle toggle)
    {
        var toggleObj = new GameObject($"Toggle_{label}");
        toggleObj.transform.SetParent(settingsPanel.transform, false);
        
        var rect = toggleObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, yPos - 0.05f);
        rect.anchorMax = new Vector2(0.9f, yPos + 0.05f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        toggle = toggleObj.AddComponent<Toggle>();
        toggle.onValueChanged.AddListener(onValueChanged);
        
        // 背景
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(toggleObj.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.7f, 0.2f);
        bgRect.anchorMax = new Vector2(0.75f, 0.8f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f);
        toggle.targetGraphic = bgImage;

        // 勾选标记
        var checkObj = new GameObject("Checkmark");
        checkObj.transform.SetParent(bgObj.transform, false);
        var checkRect = checkObj.AddComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = Vector2.zero;
        checkRect.offsetMax = Vector2.zero;
        var checkImage = checkObj.AddComponent<Image>();
        checkImage.color = new Color(0.2f, 0.8f, 0.2f);
        toggle.graphic = checkImage;
        
        // 标签
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(toggleObj.transform, false);
        var labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(0.65f, 1);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(labelObj, label, 28, TextAnchor.MiddleLeft, Color.white);
    }
    
    void CreateButtons()
    {
        // 确定按钮
        CreateButton("确定", new Vector2(-120, -400), OnConfirm);
        
        // 取消按钮
        CreateButton("取消", new Vector2(120, -400), OnCancel);
    }
    
    void CreateButton(string text, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        var btnObj = new GameObject($"Button_{text}");
        btnObj.transform.SetParent(settingsPanel.transform, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0);
        btnRect.anchorMax = new Vector2(0.5f, 0);
        btnRect.pivot = new Vector2(0.5f, 0);
        btnRect.anchoredPosition = position;
        btnRect.sizeDelta = new Vector2(200, 60);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.5f, 0.8f);
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(onClick);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        ChineseUI.CreateButtonText(textObj, text);
    }
    
    void SetBGMVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(volume);
        }
        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
    }
    
    void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    
    void LoadSettings()
    {
        // 加载音量设置
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        if (bgmSlider != null)
            bgmSlider.value = bgmVolume;
        if (sfxSlider != null)
            sfxSlider.value = sfxVolume;
        
        // 加载显示设置
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = fullscreen;
    }
    
    void OnConfirm()
    {
        Debug.Log("[SettingsUI] 保存设置");
        PlayerPrefs.Save();
        CloseSettings();
    }
    
    void OnCancel()
    {
        Debug.Log("[SettingsUI] 取消设置");
        CloseSettings();
    }
    
    void CloseSettings()
    {
        Destroy(gameObject);
    }
}
