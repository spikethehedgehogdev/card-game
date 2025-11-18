using UnityEngine;

namespace Game.Battle.Config
{
    [CreateAssetMenu(fileName = "HandConfig", menuName = "Game/Battle/HandConfig")]
    public class HandConfig : ScriptableObject
    {
        public int capacity = 6;
        public float selectOffset = 0.3f;
        public float sidedOffset = 0.3f;
        public float gap = 0.1f;
    }
}