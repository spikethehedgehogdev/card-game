using System;

namespace Game.Battle.Flow
{
    public interface IJudge
    {
        event Action<int, int> OnScoreChanged;
        event Action<int> OnRoundResult;

        bool GetRoundWinner(int playerCard, int opponentCard, out int winner);
        int BattleWinner { get; }
        void Reset();
    }

    public class MatrixJudge : IJudge
    {
        public event Action<int, int> OnScoreChanged;
        public event Action<int> OnRoundResult;

        private readonly int[,] _roundResults = {
            {0, 1, 2},
            {2, 0, 1},
            {1, 2, 0}
        };

        private readonly int[,] _battleResults = {
            {0, 2, 2, 2},
            {1, 0, 2, 2},
            {1, 1, 0, 2},
            {1, 1, 1, 0}
        };

        private (int player, int opponent) _points;

        public bool GetRoundWinner(int playerCard, int opponentCard, out int winner)
        {
            winner = _roundResults[playerCard, opponentCard];

            switch (winner)
            {
                case 0:
                    return false;
                case 1:
                    _points.player++;
                    break;
                case 2:
                    _points.opponent++;
                    break;
            }

            OnScoreChanged?.Invoke(_points.player, _points.opponent);
            OnRoundResult?.Invoke(winner);

            return true;
        }

        public int BattleWinner
        {
            get
            {
                var result = _battleResults[_points.player, _points.opponent];
                return result;
            }
        }

        public void Reset()
        {
            _points = (0, 0);
            OnScoreChanged?.Invoke(0, 0);
        }
    }
}