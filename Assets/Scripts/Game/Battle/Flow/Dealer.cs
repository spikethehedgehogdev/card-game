using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Battle.Battler;
using Game.Battle.Cards;
using Game.Battle.World;

namespace Game.Battle.Flow
{
    public interface IDealer
    {
        UniTask Deal(CancellationToken token);
    } 
    public class Dealer : IDealer
    {
        private readonly IBoard _board;
        private readonly IDeck _deck;
        private readonly IHand _playerHand;
        private readonly IHand _opponentHand;

        public Dealer(IBoard board, IDeck deck, Player player, Opponent opponent)
        {
            _board = board;
            _deck = deck;

            _playerHand = player.Hand;
            _opponentHand = opponent.Hand;
        }

        public async UniTask Deal(CancellationToken token)
        {
            _deck.ShuffleDraw();
            
            var playerSet = _deck.GetBattlerSet(0);
            var opponentSet = _deck.GetBattlerSet(1);
            
            var playerReceive = _playerHand.Setup(playerSet, token);
            var opponentReceive = _opponentHand.Setup(opponentSet, token);
            
            await UniTask.WhenAll(playerReceive, opponentReceive);
            
        }
    }
}