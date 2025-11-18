using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Services;
using Game.Battle.Battler;
using Game.Battle.Cards;
using Game.Battle.Config;
using Game.Battle.UI;
using Game.Battle.World;
using Shared.Enum;

namespace Game.Battle.Flow
{
    public class Orchestrator
    {
        private readonly IBoard _board;
        private readonly IDealer _dealer;
        private readonly IInput _input;
        private readonly Player _player;
        private readonly Opponent _opponent;
        private readonly IJudge _judge;
        private readonly BattleUI _ui;
        private readonly BattleFlowConfig _flow;

        public Orchestrator(
            IBoard board,
            IDealer dealer,
            IInput input,
            Player player,
            Opponent opponent,
            IJudge judge,
            BattleUI ui,
            BattleFlowConfig flow)
        {
            _board = board;
            _dealer = dealer;
            _input = input;
            _player = player;
            _opponent = opponent;
            _judge = judge;
            _ui = ui;
            _flow = flow;
        }

        public async UniTask StartBattleAsync(CancellationToken token)
        {
            await _ui.HideBattleResult();

            if (_judge is MatrixJudge matrixJudge)
                matrixJudge.Reset();

            await _dealer.Deal(token);

            for (var round = 1; round <= 3; round++)
            {
                var choices = await UniTask.WhenAll(
                    MakePlayerMoveAsync(token),
                    MakeOpponentMoveAsync(token));

                await RevealMovesAsync(choices.Item1, choices.Item2, token);
                await ProcessRoundResultAsync(choices.Item1, choices.Item2, token);
            }

            var cleanup = ClearBattleAsync(token);

            var final = _judge.BattleWinner switch
            {
                1 => BattleResult.Victory,
                2 => BattleResult.Defeat,
                _ => BattleResult.Draw
            };

            var showResult = _ui.ShowBattleResult(final);
            await UniTask.WhenAll(cleanup, showResult);
        }

        // ------------------------------------------------------------
        // MOVE LOGIC
        // ------------------------------------------------------------
        
        private async UniTask<ICard> MakePlayerMoveAsync(CancellationToken token)
        {
            _input.IsBlocked = false;

            var card = await _player.WaitForCardPlay(token);
            _input.IsBlocked = true;

            await card.PlaceIntoAsync(_board.PlayerSidePreset.ClosedSocket, _flow.revealDelay);
            return card;
        }

        private async UniTask<ICard> MakeOpponentMoveAsync(CancellationToken token)
        {
            var card = await _opponent.WaitForCardPlay(token);
            await card.PlaceIntoAsync(_board.OpponentSidePreset.ClosedSocket, _flow.cardRevealDuration);
            return card;
        }

        private async UniTask RevealMovesAsync(ICard player, ICard opponent, CancellationToken token)
        {
            await UniTask.WhenAll(
                player.PlaceIntoAsync(_board.PlayerSidePreset.OpenedSocket, _flow.cardRevealDuration),
                opponent.PlaceIntoAsync(_board.OpponentSidePreset.OpenedSocket, _flow.cardRevealDuration)
            );
        }

        // ------------------------------------------------------------
        // ROUND RESULT
        // ------------------------------------------------------------

        private async UniTask ProcessRoundResultAsync(ICard player, ICard opponent, CancellationToken token)
        {
            _judge.GetRoundWinner(player.Type, opponent.Type, out _);

            await UniTask.Delay(TimeSpan.FromSeconds(_flow.postRoundDelay), cancellationToken: token);

            var pDiscard = _board.DiscardPile.ReceiveCard(player);
            await UniTask.Delay(TimeSpan.FromSeconds(_flow.discardStepDelay), cancellationToken: token);
            var oDiscard = _board.DiscardPile.ReceiveCard(opponent);

            await UniTask.WhenAll(pDiscard, oDiscard);
        }

        // ------------------------------------------------------------
        // CLEANUP
        // ------------------------------------------------------------

        private async UniTask ClearBattleAsync(CancellationToken token)
        {
            await _board.DiscardPile.Clear(token);
            await _player.Hand.Clear(token);
            await _opponent.Hand.Clear(token);
        }
    }
}
