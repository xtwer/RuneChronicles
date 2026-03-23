using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置UI - BGM/SFX音量滑块、全屏切换
/// </summary>
public class SettingsUI : MonoBehaviour
{
    private GameObject overlayObj;
    private GameObject panelObj;

    void Start()
    {
        CreateSettingsUI();
    }

    void OnDestroy()
    {
        if (overlayObj != null) Destroy(overlayObj);
        if (panelObj != null) Destroy(panelObj);
    }

    void CreateSettingsUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("SettingsCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 半透明遮罩
        overlayObj = new GameObject("Overlay");
        overlayObj.transform.SetParent(canvas.transform, false);
        var overlayRect = overlayObj.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        overlayObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.6f);

        // 设置面板
        panelObj = new GameObject("SettingsPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        var panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.3f, 0.2f);
        panelRect.anchorMax = new Vector2(0.7f, 0.85f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelObj.AddComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f, 0.98f);

        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panelObj.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.85f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        ChineseUI.CreateTitle(titleObj, "游戏设置");

        // BGM音量
        CreateLabeledSlider(panelObj.transform,
            "背景音乐音量",
            new Vector2(0.05f, 0.68f), new Vector2(0.95f, 0.78f),
            AudioManager.Instance != null ? AudioManager.Instance.bgmVolume : 0.7f,
            OnBGMVolumeChanged);

        // SFX音量
        CreateLabeledSlider(panelObj.transform,
            "音效音量",
            new Vector2(0.05f, 0.52f), new Vector2(0.95f, 0.62f),
            AudioManager.Instance != null ? AudioManager.Instance.sfxVolume : 1.0f,
            OnSFXVolumeChanged);

        // 全屏切换
        CreateFullscreenToggle(panelObj.transform);

        // 返回按钮
        var btnObj = new GameObject("BackButton");
        btnObj.transform.SetParent(panelObj.transform, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.25f, 0.05f);
        btnRect.anchorMax = new Vector2(0.75f, 0.18f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;
        btnObj.AddComponent<Image>().color = new Color(0.2f, 0.4f, 0.8f);
        var btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(OnBack);

        var btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        var btnTextRect = btnTextObj.AddComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;
        ChineseUI.CreateButtonText(btnTextObj, "返回");

        Debug.Log("[SettingsUI] 设置界面已创建");
    }

    void CreateLabeledSlider(Transform parent, string label,
        Vector2 anchorMin, Vector2 anchorMax,
        float initialValue, UnityEngine.Events.UnityAction<float> onValueChanged)
    {
        // 标签
        var labelObj = new GameObject($"Label_{label}");
        labelObj.transform.SetParent(parent, false);
        var labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(anchorMin.x, anchorMin.y + 0.05f);
        labelRect.anchorMax = new Vector2(0.45f, anchorMax.y);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(labelObj, label, 26, TextAnchor.MiddleLeft, Color.white);

        // 滑块容器
        var sliderObj = new GameObject($"Slider_{label}");
        sliderObj.transform.SetParent(parent, false);
        var sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.48f, anchorMin.y + 0.05f);
        sliderRect.anchorMax = new Vector2(anchorMax.x, anchorMax.y);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        // 背景
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgObj.AddComponent<Image>().color = new Color(0.25f, 0.25f, 0.35f);

        // 填充区域
        var fillAreaObj = new GameObject("Fill Area");
        fillAreaObj.transform.SetParent(sliderObj.transform, false);
        var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(5, 0);
        fillAreaRect.offsetMax = new Vector2(-15, 0);

        var fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillAreaObj.transform, false);
        var fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        fillObj.AddComponent<Image>().color = new Color(0.3f, 0.6f, 1f);

        // 滑块手柄区域
        var handleAreaObj = new GameObject("Handle Slide Area");
        handleAreaObj.transform.SetParent(sliderObj.transform, false);
        var handleAreaRect = handleAreaObj.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);

        var handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(handleAreaObj.transform, false);
        var handleRect = handleObj.AddComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0, 0);
        handleRect.anchorMax = new Vector2(0, 1);
        handleRect.sizeDelta = new Vector2(20, 0);
        handleObj.AddComponent<Image>().color = Color.white;

        var slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = initialValue;
        slider.onValueChanged.AddListener(onValueChanged);
    }

    void CreateFullscreenToggle(Transform parent)
    {
        // 标签
        var labelObj = new GameObject("FullscreenLabel");
        labelObj.transform.SetParent(parent, false);
        var labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.05f, 0.35f);
        labelRect.anchorMax = new Vector2(0.6f, 0.46f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(labelObj, "全屏模式", 26, TextAnchor.MiddleLeft, Color.white);

        // Toggle背景
        var toggleObj = new GameObject("FullscreenToggle");
        toggleObj.transform.SetParent(parent, false);
        var toggleRect = toggleObj.AddComponent<RectTransform>();
        toggleRect.anchorMin = new Vector2(0.65f, 0.36f);
        toggleRect.anchorMax = new Vector2(0.78f, 0.45f);
        toggleRect.offsetMin = Vector2.zero;
        toggleRect.offsetMax = Vector2.zero;

        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(toggleObj.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.25f, 0.25f, 0.35f);

        var checkmarkObj = new GameObject("Checkmark");
        checkmarkObj.transform.SetParent(toggleObj.transform, false);
        var checkmarkRect = checkmarkObj.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0.1f, 0.1f);
        checkmarkRect.anchorMax = new Vector2(0.9f, 0.9f);
        checkmarkRect.offsetMin = Vector2.zero;
        checkmarkRect.offsetMax = Vector2.zero;
        var checkmarkImage = checkmarkObj.AddComponent<Image>();
        checkmarkImage.color = new Color(0.3f, 0.6f, 1f);

        var toggle = toggleObj.AddComponent<Toggle>();
        toggle.targetGraphic = bgImage;
        toggle.graphic = checkmarkImage;
        toggle.isOn = Screen.fullScreen;
        toggle.onValueChanged.AddListener(OnFullscreenChanged);
    }

    void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBGMVolume(value);
    }

    void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
    }

    void OnBack()
    {
        Debug.Log("[SettingsUI] 关闭设置");
        Destroy(gameObject);
    }
}
