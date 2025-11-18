using TMPro;
using UnityEngine;

namespace Game.Battle.UI
{
    public class BattleScorePanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerScoreText;
        [SerializeField] private TMP_Text botScoreText;
        [SerializeField] private CanvasGroup group;
        
        public void UpdateScore(int player, int bot)
        {
            playerScoreText.text = player.ToString();
            botScoreText.text = bot.ToString();
        }
    }
}