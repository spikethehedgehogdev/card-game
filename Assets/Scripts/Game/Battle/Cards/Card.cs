using System.Threading;
using BetweenRedKit.Core;
using BetweenRedKit.Easing;
using BetweenRedKit.Integrations.Cysharp.Integrations.CySharp;
using BetweenRedKit.Integrations.Unity;
using Cysharp.Threading.Tasks;
using Shared.Structs;
using UnityEngine;

namespace Game.Battle.Cards
{
    public interface ICard
    {
        public int Type { get; }

        void Spawn(Socket socket, bool animated = true);
        UniTask Despawn(bool animated = true);

        public UniTask PlaceIntoAsync(Socket socket, float duration = 0.2f);

        Collider Collider { get; }
    }

    public class Card : ICard
    {
        private readonly BetweenProcessor _between;
        private readonly Transform _transform;

        private CancellationTokenSource _cts;

        public Card(int type, ICardPrefab prefab, BetweenProcessor between)
        {
            _between = between;
            Collider = prefab.Collider;
            _transform = prefab.Transform;
            Type = type;
        }
        
        public Collider Collider { get; }

        public string Name => _transform.gameObject.name;
        public int Type { get; }

        public void Spawn(Socket socket, bool animated = true)
        {
            CancelTween();
            
            _transform.position = socket.Position;
            _transform.rotation = socket.Rotation;

            _transform.gameObject.SetActive(true);
            _transform.ScaleTo(_between, Vector3.one, animated ? .2f : 0f, EaseType.OutQuint);
        }

        public async UniTask Despawn(bool animated = true)
        {
            CancelTween();

            var token = _cts.Token;
            
            await _transform.ScaleTo(_between, Vector3.zero, animated ? .2f : .0f, EaseType.InQuint).AwaitCompletion(token);
            
            _transform.gameObject.SetActive(false);
        }

        public void PlaceInto(Socket socket)
        {
            _transform.position = socket.Position;
            _transform.rotation = socket.Rotation;
        }

        public async UniTask PlaceIntoAsync(Socket socket, float duration = 0.2f)
        {
            CancelTween();

            var token = _cts.Token;
            
            var move = _transform.MoveTo(_between, socket.Position, duration).AwaitCompletion(token);
            var rotate = _transform.RotateTo(_between, socket.Rotation, duration).AwaitCompletion(token);

            await UniTask.WhenAll(move, rotate);
        }

        private void CancelTween()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }
        
    }
}