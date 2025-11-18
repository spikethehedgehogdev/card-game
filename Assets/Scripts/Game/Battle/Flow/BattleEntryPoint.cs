using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace Game.Battle.Flow
{
    public class BattleEntryPoint : IAsyncStartable
    {
        private readonly Orchestrator _controller;

        public BattleEntryPoint(Orchestrator controller)
        {
            _controller = controller;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            try
            {
                await _controller.StartBattleAsync(cancellation);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[BattleEntryPoint] Play Mode остановлен — ничего страшного");
            }
        }
    }
}