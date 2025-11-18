using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Framework.Services
{
    public class AddressablesService
    {
        private static AddressablesService _instance;
        private readonly LifetimeScope _gameScope;

        private readonly Dictionary<string, object> _cache = new();
        
        private AddressablesService(LifetimeScope lifetimeScope)
        {
            _gameScope = lifetimeScope;
        }
        
        public static async UniTask<AddressablesService> InitializeAsync(LifetimeScope lifetimeScope)
        {
            if (_instance != null) return _instance;
            
            _instance = new AddressablesService(lifetimeScope);
            var handle = Addressables.InitializeAsync();
            await handle.Task;

            return _instance;
        }

        public async UniTask LoadSceneAsync(string key, bool additive = false)
        {
            using (LifetimeScope.EnqueueParent(_gameScope))
            {
                var mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

                var handle = Addressables.LoadSceneAsync(key, mode);

                await handle.Task;

                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded) 
                    Debug.Log($"[Addressables] Scene loaded: {key}");
                else 
                    Debug.LogError($"[Addressables] Failed to load scene: {key}");
                
            }
        }
        public async UniTask<T> LoadConfig<T>(string key) where T : ScriptableObject
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            return await handle.Task;
        }
        public async UniTask<T> LoadAssetAsync<T>(string key) where T : class
        {
            if (_cache.TryGetValue(key, out var cached))
                return cached as T;

            var handle = Addressables.LoadAssetAsync<T>(key);
            var asset = await handle.Task;

            if (asset == null)
            {
                Debug.LogError($"[Addressables] Failed to load asset: {key}");
                return null;
            }

            _cache[key] = asset;
            return asset;
        }

    }
}