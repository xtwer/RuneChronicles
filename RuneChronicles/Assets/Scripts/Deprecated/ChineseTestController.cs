using UnityEngine;
using UnityEngine.UI;

namespace RuneChronicles
{
    public class ChineseTestController : MonoBehaviour
    {
        public Text statusText;
        public Button drawCardButton;
        public Button fusionButton;
        public Button endTurnButton;
        
        private void Start()
        {
            if (drawCardButton != null)
                drawCardButton.onClick.AddListener(OnDrawCard);
            
            if (fusionButton != null)
                fusionButton.onClick.AddListener(OnFusion);
            
            if (endTurnButton != null)
                endTurnButton.onClick.AddListener(OnEndTurn);
        }
        
        private void Update()
        {
            UpdateStatus();
        }
        
        private void UpdateStatus()
        {
            if (GameManager.Instance != null && statusText != null)
            {
                int handSize = CardManager.Instance != null ? CardManager.Instance.hand.Count : 0;
                int enemyHP = GameManager.Instance.currentEnemy != null ? 
                    GameManager.Instance.currentEnemy.currentHealth : 0;
                
                string gameStateText = "";
                if (GameManager.Instance.currentState == GameState.Win)
                {
                    gameStateText = "\n\n🎉 胜利！你击败了敌人！";
                }
                else if (GameManager.Instance.currentState == GameState.Lose)
                {
                    gameStateText = "\n\n💀 失败！你被击败了！";
                }
                
                int cardsLeft = GameManager.Instance.maxCardsPerTurn - GameManager.Instance.cardsPlayedThisTurn;
                
                statusText.text = $"回合: {GameManager.Instance.turnCount}\n" +
                                 $"生命: {GameManager.Instance.playerHealth}\n" +
                                 $"魔法: {GameManager.Instance.currentMana}/{GameManager.Instance.maxMana}\n" +
                                 $"本回合可打牌: {cardsLeft}/{GameManager.Instance.maxCardsPerTurn}\n" +
                                 $"敌人生命: {enemyHP}\n" +
                                 $"手牌: {handSize} 张" +
                                 gameStateText;
            }
        }
        
        private void OnDrawCard()
        {
            // 检查游戏状态
            if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing)
            {
                Debug.Log("游戏已结束，无法抽卡！");
                return;
            }
            
            if (CardManager.Instance != null)
            {
                CardManager.Instance.DrawCards(1);
            }
        }
        
        private void OnFusion()
        {
            if (FusionSystem.Instance != null)
            {
                var result = FusionSystem.Instance.TryFusion();
                if (result != null)
                {
                    Debug.Log($"融合成功！创建了: {result.cardName}");
                }
            }
        }
        
        private void OnEndTurn()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndTurn();
                
                if (CardManager.Instance != null)
                {
                    CardManager.Instance.DrawCards(CardManager.Instance.drawPerTurn);
                }
            }
        }
    }
}
