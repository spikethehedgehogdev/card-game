using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Battle.Cards;
using Game.Battle.Config;
using R3;
using Shared.Structs;
using UnityEngine;

namespace Game.Battle.World
{
    public interface IHand
    {
        public int CardCount { get; }
        public Subject<List<ICard>> OnSetup { get; }
        public Subject<ICard> OnCardRemoved { get; }
        
        public Subject<Unit> OnClear { get; }
        
        UniTask Setup(List<ICard> newCards, CancellationToken token);
        UniTask Reveal(CancellationToken token);
        UniTask Clear(CancellationToken token);
        
        void Select(int index);
        void Release();
        ICard Play();
    }
    public class Hand : IHand
    {
        public struct HandPreset
        {
            public HandPreset(int capacity, float selectOffset, float sidedOffset, float gap)
            {
                Capacity = capacity;
                SelectOffset = selectOffset;
                SidedOffset = sidedOffset;
                Gap = gap;
            }
            public readonly int Capacity;
            public readonly float SelectOffset;
            public readonly float SidedOffset;
            public readonly float Gap;

        }

        private readonly List<Socket>[] _layouts;
        private List<Socket> _currentLayout;

        private readonly List<ICard> _cards = new();
        private int _hovered;
        private readonly HandConfig _config;

        public int CardCount => _cards.Count;
        public Subject<List<ICard>> OnSetup { get; } = new();
        public Subject<ICard> OnCardRemoved { get; } = new();
        public Subject<Unit> OnClear { get; } = new();
        
        public Hand(HandConfig config, ICurve curve)
        {
            _config = config;
            _layouts = new List<Socket>[_config.capacity];
            
            for (var i = 0; i < _config.capacity; i++)
            {
                _layouts[i] = curve.GetLayout(i + 1, _config.gap);
            }
            
            _currentLayout = _layouts[_config.capacity - 1];
        }
        
        public async UniTask Setup(List<ICard> newCards, CancellationToken token)
        {
            _cards.Clear();
            _cards.AddRange(newCards);
            
            OnSetup.OnNext(_cards);
            
            await Reveal(token);
        }

        public async UniTask Reveal(CancellationToken token)
        {
            _currentLayout = _layouts[_cards.Count - 1];
            for (var i = 0; i < _cards.Count; i++)
            {
                _cards[i].Spawn(_currentLayout[i]);
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
            }
        }

        public async UniTask Clear(CancellationToken token)
        {
            foreach (var card in _cards)
            {
                card.Despawn();
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
            }

            _cards.Clear();
            _hovered = -1;
            OnClear.OnNext(Unit.Default);
        }
        
        public void Select(int index)
        {
            if (_cards.Count == 0)
                return;

            if (index < 0 || index >= _cards.Count)
                return;

            _hovered = index;

            for (var i = 0; i < _cards.Count; i++)
            {
                if (i == index)
                {
                    var lifted = _currentLayout[i].CopyWithOffset(new Vector3(0, _config.selectOffset, 0));
                    _cards[i].PlaceIntoAsync(lifted).Forget();
                }
                else
                {
                    var offsetX = (i < index) ? _config.sidedOffset : -_config.sidedOffset;

                    var sided = _currentLayout[i].CopyWithOffset(new Vector3(offsetX, 0, 0));
                    _cards[i].PlaceIntoAsync(sided).Forget();
                }
            }
        }
        
        public ICard Play()
        {
            var played = _cards[_hovered];
            _cards.Remove(played);
            OnCardRemoved.OnNext(played);

            _currentLayout = _layouts[_cards.Count - 1];
            Release();
            
            return played;
        }
        
        public void Release()
        {
            for (var i = 0; i < _cards.Count; i++) 
                _cards[i].PlaceIntoAsync(_currentLayout[i]).Forget();
        }

    }
}