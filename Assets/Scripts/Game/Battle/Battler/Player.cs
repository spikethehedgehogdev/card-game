using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Battle.Cards;
using Game.Battle.World;
using R3;
using UnityEngine;

namespace Game.Battle.Battler
{
    public class Player : IDisposable
    {
        private UniTaskCompletionSource<ICard> _tcs;
        
        private readonly CompositeDisposable _disposables = new();
        
        private readonly Dictionary<Collider, int> _map;
        private readonly CardCollision _cardCollision;
        private readonly Camera _camera;


        public Player(IHand hand, CardCollision cardCollision)
        {
            Hand = hand;
            _cardCollision = cardCollision;

            cardCollision.OnHoverEnter
                .Subscribe(Hand.Select)
                .AddTo(_disposables);
            cardCollision.OnHoverExit
                .Subscribe(_ => Hand.Release())
                .AddTo(_disposables);
            cardCollision.OnClick
                .Subscribe(_ => _tcs?.TrySetResult(Hand.Play()))
                .AddTo(_disposables);
            
            Hand.OnSetup
                .Subscribe(SetupCollisions)
                .AddTo(_disposables);
            Hand.OnCardRemoved
                .Subscribe(RemoveCollider)
                .AddTo(_disposables);
        }
        
        public IHand Hand { get; }

        private void SetupCollisions(List<ICard> cards)
        {
            _cardCollision.Setup(cards.Select(t => t.Collider).ToList());
        }
        private void RemoveCollider(ICard card)
        {
            _cardCollision.RemoveCollider(card.Collider);
        }
        
        public UniTask<ICard> WaitForCardPlay(CancellationToken token)
        {
            _tcs = new UniTaskCompletionSource<ICard>();

            token.Register(() =>
            {
                _tcs.TrySetCanceled();
            });

            return _tcs.Task;
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
    }
}