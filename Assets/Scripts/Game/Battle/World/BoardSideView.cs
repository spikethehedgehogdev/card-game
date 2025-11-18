using System;
using System.Collections.Generic;
using Shared.Gizmos;
using Shared.Structs;
using UnityEngine;

namespace Game.Battle.World
{
    [Serializable]
    public struct BoardSideView
    {
        public Transform handRoot;
        public Transform openedCardSocket, closedCardSocket;
        
        public Transform HandRoot => handRoot;
        public Transform OpenedCardSocket => openedCardSocket;
        public Transform ClosedCardSocket => closedCardSocket;
        
#if UNITY_EDITOR
        private BezierCurve _curve;
        
        private List<Socket> _cardSockets;

        public void Enable(float edgeOffsetX, Vector2 middleOffset)
        {
            var root = new Socket(handRoot.position, handRoot.rotation);
            _curve = new BezierCurve(root, edgeOffsetX, middleOffset);
            
        }

        public void Validate(Socket debugSocket, float edgeOffsetX, Vector2 middleOffset, float cardsGap, bool useDebug = false)
        {
            if (_curve == null) return;
            if (useDebug)
                _curve.UpdatePoints(debugSocket, edgeOffsetX, middleOffset);
            else
            {
                var root = new Socket(handRoot.position, handRoot.rotation);
                _curve.UpdatePoints(root, edgeOffsetX, middleOffset);
            }
            
            _cardSockets = _curve.GetLayout(6, cardsGap);
        }
        
        public void DrawGizmos(int curveSegments, float curveGap)
        {
            if (_curve == null) return;
            
            _curve.DrawGizmos(curveSegments, curveGap);
            
            if (_cardSockets == null) return;
            
            foreach (var socket in _cardSockets)
                BattleGizmos.DrawFakeCard(socket, Color.blue);
            
            
            BattleGizmos.DrawFakeCard(openedCardSocket, Color.green);
            BattleGizmos.DrawFakeCard(closedCardSocket, Color.red);
        }
        
#endif
    }
}