using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Battle.Cards;
using Game.Battle.World;
using UnityEngine;

namespace Game.Battle.Battler
{
    public class Opponent
    {
        public Opponent(IHand hand)
        {
            Hand = hand;
        }

        public IHand Hand { get; }

        public async UniTask<ICard> WaitForCardPlay(CancellationToken token)
        {
            ICard played = null;
            var count = Hand.CardCount;
            var choice = Random.Range(0, count);

            for (var r = 0; r < 2; r++)
            {
                for (var i = 0; i < count; i++)
                {
                    Hand.Select(i);
                    await UniTask.Delay(250, cancellationToken: token);

                    if (r != 1 || i != choice) continue;
                    played = Hand.Play();
                    break;

                }
            }

            return played;
        }
    }
}