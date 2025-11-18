using System;
using System.Collections.Generic;
using Framework.Services;
using R3;
using UnityEngine;

namespace Game.Battle.Cards
{
    public class CardCollision : IDisposable
    {
        private readonly Camera _camera;

        private readonly CompositeDisposable _disposables = new();
        private readonly List<Collider> _colliders = new();
        private readonly Dictionary<Collider, int> _indices = new();
        
        private (bool isHovered, int index, Collider collider) _current;

        public Subject<int> OnHoverEnter { get; } = new();
        public Subject<Unit> OnHoverExit { get; } = new();
        public Subject<Unit> OnClick { get; } = new();
        
        private readonly int _layerMask;
        
        public CardCollision(IInput input, Camera camera)
        {
            _camera = camera;
            
            _layerMask = LayerMask.GetMask("Cards");
            
            input.OnMouseMoved
                .Subscribe(Check)
                .AddTo(_disposables);
            
            input.OnClicked
                .Subscribe(_ => Click())
                .AddTo(_disposables);
            
        }

        public void Setup(List<Collider> colliders)
        {
            _colliders.Clear();
            _indices.Clear();

            _colliders.AddRange(colliders);

            for (var i = 0; i < _colliders.Count; i++)
                _indices[_colliders[i]] = i;
        }

        public void RemoveCollider(Collider collider)
        {
            if (!_indices.TryGetValue(collider, out var index))
                return;

            _colliders.RemoveAt(index);
            _indices.Remove(collider);

            for (var i = index; i < _colliders.Count; i++)
                _indices[_colliders[i]] = i;
        }

        
        private void Check(Vector2 screenPosition) 
        {
            var ray = _camera.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out var hit, 999f, _layerMask))
            {
                if (_indices.TryGetValue(hit.collider, out var index))
                {
                    HandleHover(index, hit.collider);
                }
                else if (_current.isHovered)
                {
                    OnHoverExit.OnNext(Unit.Default);
                    _current.isHovered = false;
                }
            }
            else if (_current.isHovered)
            {
                OnHoverExit.OnNext(Unit.Default);
                _current.isHovered = false;
            }
        }
        private void HandleHover(int index, Collider collider)
        {
            if (_current.isHovered)
            {
                if (_current.collider == collider)
                    return;
            }
            else
            {
                _current.isHovered = true;
            }

            _current.index = index;
            _current.collider = collider;
            OnHoverEnter.OnNext(index);
        }


        private void Click()
        {
            if (!_current.isHovered) return;
            
            OnClick.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            
            OnHoverEnter.Dispose();
            OnHoverExit.Dispose();
            OnClick.Dispose();

            _colliders.Clear();
        }
    }
}