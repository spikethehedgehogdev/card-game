using UnityEngine;

namespace Game.Battle.Config
{
    [CreateAssetMenu(fileName = "BattleFlowConfig", menuName = "Game/Battle/BattleFlowConfig")]
    public class BattleFlowConfig : ScriptableObject
    {
        [Header("Hand Reveal")]
        public float revealDelay = 0.1f;

        [Header("Card Play")]
        public float cardRevealDuration = 0.15f;

        [Header("Discard")]
        public float discardDelay = 0.15f;
        public float discardStepDelay = 0.1f;

        [Header("Round Flow")]
        public float postRoundDelay = 1.0f;
    }
}