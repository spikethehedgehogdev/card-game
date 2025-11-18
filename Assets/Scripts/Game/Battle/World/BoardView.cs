using Shared.Structs;
using UnityEngine;

namespace Game.Battle.World
{
    [ExecuteAlways]
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private BoardSideView player, opponent;
        
        [SerializeField] private Transform discardPileRoot;
        
        public BoardSideView Player => player;
        public BoardSideView Opponent => opponent;
        public Transform DiscardPileRoot => discardPileRoot;

#if UNITY_EDITOR

        public int curveSegments = 20;
        public float curveGap = 10f;
        public float cardsGap = 0.1f;

        public Vector3 rootPosition;
        public Vector3 rootRotation;
        public bool debug = false;

        private void OnEnable()
        {
            //player.Enable();
            //opponent.Enable();
        }

        private void OnValidate()
        {
            var debugSocketP = new Socket(rootPosition, Quaternion.Euler(rootRotation));
            
            //player.Validate(debugSocketP, edgeOffsetX, middleOffset, cardsGap, debug);
            
            var oPos = new Vector3(-rootPosition.x, rootPosition.y, -rootPosition.z);
            var oRot = new Vector3(rootRotation.x, rootRotation.y + 180, rootRotation.z);
            var debugSocketO = new Socket(oPos, Quaternion.Euler(oRot));
            
            //opponent.Validate(debugSocketO, edgeOffsetX, middleOffset, cardsGap, debug);
        }
        private void OnDrawGizmos()
        {
            //player.DrawGizmos(curveSegments, curveGap);
            //opponent.DrawGizmos(curveSegments, curveGap);
        }
#endif
    }
}