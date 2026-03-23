using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RuneChronicles
{
    /// <summary>
    /// 简单的测试场景控制器
    /// </summary>
    public class TestSceneController : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI playerHealthText;
        public TextMeshProUGUI enemyHealthText;
        public TextMeshProUGUI manaText;
        public Button drawCardButton;
        public Button fusionButton;
        public Button endTurnButton;
        
        [Header("Test Cards")]
        public CardData[] testCards;
        
        private void Start()
        {
            // 初始化管理器
            if (GameManager.Instance != null)
            {
                GameManager.Instance.currentEnemy = Resources.Load<EnemyData>("Enemies/TestSlime");
                if (GameManager.Instance.currentEnemy != null)
                {
                    GameManager.Instance.currentEnemy.Initialize();
                }
            }
            
            // 绑定按钮
            if (drawCardButton != null)
                drawCardButton.onClick.AddListener(OnDrawCard);
            
            if (fusionButton != null)
                fusionButton.onClick.AddListener(OnFusion);
            
            if (endTurnButton != null)
                endTurnButton.onClick.AddListener(OnEndTurn);
            
            UpdateUI();
        }
        
        private void Update()
        {
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            if (GameManager.Instance != null)
            {
                if (playerHealthText != null)
                    playerHealthText.text = $"HP: {GameManager.Instance.playerHealth}";
                
                if (manaText != null)
                    manaText.text = $"Mana: {GameManager.Instance.currentMana}/{GameManager.Instance.maxMana}";
                
                if (enemyHealthText != null && GameManager.Instance.currentEnemy != null)
                    enemyHealthText.text = $"{GameManager.Instance.currentEnemy.enemyName} HP: {GameManager.Instance.currentEnemy.currentHealth}";
            }
        }
        
        private void OnDrawCard()
        {
            if (CardManager.Instance != null)
            {
                CardManager.Instance.DrawCards(1);
                Debug.Log($"Hand size: {CardManager.Instance.hand.Count}");
            }
        }
        
        private void OnFusion()
        {
            if (FusionSystem.Instance != null)
            {
                var result = FusionSystem.Instance.TryFusion();
                if (result != null)
                {
                    Debug.Log($"Fusion successful! Created: {result.cardName}");
                }
            }
        }
        
        private void OnEndTurn()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndTurn();
                
                // 回合开始抽卡
                if (CardManager.Instance != null)
                {
                    CardManager.Instance.DrawCards(CardManager.Instance.drawPerTurn);
                }
            }
        }
    }
}
