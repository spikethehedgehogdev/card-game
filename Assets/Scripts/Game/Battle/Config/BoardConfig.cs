using UnityEngine;

namespace Game.Battle.Config
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Game/Battle/BoardConfig")]
    public class BoardConfig : ScriptableObject
    {
        [Header("Bezier Layout")]
        public float edgeOffsetX = 2.0f;
        public Vector2 middleOffset = new(0, 1.5f);

        [Header("Discard")]
        public float discardGap = 0.1f;
    }
}