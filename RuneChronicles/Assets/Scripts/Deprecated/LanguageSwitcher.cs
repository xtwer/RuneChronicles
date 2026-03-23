using UnityEngine;
using UnityEngine.UI;

namespace RuneChronicles
{
    /// <summary>
    /// 语言切换按钮
    /// </summary>
    public class LanguageSwitcher : MonoBehaviour
    {
        public Button chineseButton;
        public Button englishButton;
        public Button japaneseButton;
        public Button koreanButton;
        
        private void Start()
        {
            if (chineseButton != null)
                chineseButton.onClick.AddListener(() => SwitchLanguage(LocalizationManager.Language.Chinese));
            
            if (englishButton != null)
                englishButton.onClick.AddListener(() => SwitchLanguage(LocalizationManager.Language.English));
            
            if (japaneseButton != null)
                japaneseButton.onClick.AddListener(() => SwitchLanguage(LocalizationManager.Language.Japanese));
            
            if (koreanButton != null)
                koreanButton.onClick.AddListener(() => SwitchLanguage(LocalizationManager.Language.Korean));
        }
        
        private void SwitchLanguage(LocalizationManager.Language language)
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.SetLanguage(language);
            }
        }
    }
}
