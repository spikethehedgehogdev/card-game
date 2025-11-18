using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Shared.Generated;
using Shared.Structs;
using UnityEngine;

namespace Game.Battle.Cards
{
    public interface IPile
    {
        UniTask Clear();
    }
    public class Pile
    {
        private readonly List<ICard> _cards;
        private int _cardCount = 0;
        private readonly Vector3 _offsetPosition;

        private readonly Vector3 _firstPosition;
        private readonly Quaternion _rotation;
        public Pile(Transform pileRoot)
        {
            _offsetPosition = new Vector3(0.0f, CardInfo.Size.z, 0.0f);
            _rotation = pileRoot.rotation;
            _firstPosition = pileRoot.position + _offsetPosition / 2.0f;
            _cards = new List<ICard>();
        }

        public async UniTask ReceiveCard(ICard card)
        {
            _cards.Add(card);
            await card.PlaceIntoAsync(GetFreeSocket());
        }

        private Socket GetFreeSocket()
        {
            var position = _firstPosition + _offsetPosition * _cardCount;
            _cardCount++;
            return new Socket(position, _rotation);
        }
        
        public async UniTask Clear(CancellationToken token)
        {
            for (var i = _cards.Count - 1; i >= 0; i--)
            {
                _cards[i].Despawn();
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
                
            }
            _cards.Clear();
            _cardCount = 0;
        }
    }
}