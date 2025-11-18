using Game.Battle.Cards;
using Game.Battle.Config;
using Shared.Structs;
using UnityEngine;

namespace Game.Battle.World
{
    public struct BoardSidePreset
    {
        public BoardSidePreset(Transform handRoot, Transform openedSocket, Transform closedSocket)
        {
            HandRoot = new Socket(handRoot.position, handRoot.rotation);
            OpenedSocket = new Socket(openedSocket.position, openedSocket.rotation);
            ClosedSocket = new Socket(closedSocket.position, closedSocket.rotation);
        }
        public Socket HandRoot { get; }
        public Socket OpenedSocket { get; }
        public Socket ClosedSocket { get; }
    }
    public interface IBoard
    {
        public BoardSidePreset PlayerSidePreset { get; }
        public BoardSidePreset OpponentSidePreset { get; }
        
        public float EdgeOffsetX  { get; }
        public Vector2 MiddleOffset  { get; }
        
        Pile DiscardPile { get; }

    }

    public class Board : IBoard
    {
        public BoardSidePreset PlayerSidePreset { get; }
        public BoardSidePreset OpponentSidePreset { get; }
        
        public float EdgeOffsetX  { get; }
        public Vector2 MiddleOffset  { get; }
        
        public Pile DiscardPile { get; }
        
        public Board(BoardView view, BoardConfig config)
        {
            PlayerSidePreset = new BoardSidePreset(
                view.Player.HandRoot, 
                view.Player.OpenedCardSocket, 
                view.Player.ClosedCardSocket);
            OpponentSidePreset = new BoardSidePreset(
                view.Opponent.HandRoot, 
                view.Opponent.OpenedCardSocket, 
                view.Opponent.ClosedCardSocket);

            EdgeOffsetX = config.edgeOffsetX;
            MiddleOffset = config.middleOffset;
            
            DiscardPile = new Pile(view.DiscardPileRoot);

        }

        
    }
}