using UnityEngine;
using UnityEngine.UI;

namespace RuneChronicles
{
    /// <summary>
    /// 自动本地化的 Text 组件
    /// 挂载到任何 Text 上，会自动根据当前语言显示对应文本
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [Header("本地化键")]
        [Tooltip("在 LocalizationManager 中定义的文本键，例如: ui.title")]
        public string localizationKey;
        
        [Header("格式化参数（可选）")]
        [Tooltip("如果文本需要参数（如 '手牌: {0} 张'），可在这里动态提供")]
        public bool useDynamicParams = false;
        
        private Text textComponent;
        private string[] dynamicParams;
        
        private void Awake()
        {
            textComponent = GetComponent<Text>();
        }
        
        private void Start()
        {
            UpdateText();
            
            // 订阅语言切换事件
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged += UpdateText;
            }
        }
        
        private void OnDestroy()
        {
            // 取消订阅
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
            }
        }
        
        /// <summary>
        /// 手动设置动态参数
        /// </summary>
        public void SetParams(params object[] args)
        {
            dynamicParams = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                dynamicParams[i] = args[i].ToString();
            }
            UpdateText();
        }
        
        private void UpdateText()
        {
            if (textComponent == null || LocalizationManager.Instance == null)
                return;
            
            if (string.IsNullOrEmpty(localizationKey))
            {
                Debug.LogWarning($"LocalizedText on {gameObject.name} has no localization key set!");
                return;
            }
            
            if (useDynamicParams && dynamicParams != null && dynamicParams.Length > 0)
            {
                textComponent.text = LocalizationManager.Instance.GetText(localizationKey, dynamicParams);
            }
            else
            {
                textComponent.text = LocalizationManager.Instance.GetText(localizationKey);
            }
        }
    }
}
