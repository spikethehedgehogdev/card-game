using System.Collections.Generic;
using BetweenRedKit.Core;
using Shared.Enum;
using Shared.Utils;
using UnityEngine;

namespace Game.Battle.Cards
{
    public interface ICardFactory
    {
        public ICard Create(int type, int side);
    }
    public class CardFactory : ICardFactory
    {
        private int _index = -1;
        private static readonly int MainTexSt = Shader.PropertyToID("_MainTex_ST");
        private readonly CardPrefab _prefab;
        private readonly BetweenProcessor _between;
        private readonly MaterialPropertyBlock _mpb;
        private readonly Dictionary<int, Vector4> _uvMap;
        private readonly int _targetSubMeshIndex;

        public CardFactory(CardPrefab prefab, BetweenProcessor between)
        {
            _prefab = prefab;
            _between = between;
            _mpb = new MaterialPropertyBlock();
            _uvMap = UVMap.Generate(new Vector2Int(2, 2), 0, 2);
            _targetSubMeshIndex = 1;
        }

        public ICard Create(int type, int side)
        {
            _index++;
            var instance = Object.Instantiate(_prefab);
            
            instance.name = GetName(type, side);
            instance.gameObject.SetActive(false);
            instance.transform.localScale = Vector3.zero;
            
            SetupType(instance.Renderer, type);
            
            return new Card(type, instance, _between);
        }

        private void SetupType(MeshRenderer renderer, int type)
        {
            renderer.GetPropertyBlock(_mpb, _targetSubMeshIndex);
            
            if (_uvMap.TryGetValue(type, out var uv))
                _mpb.SetVector(MainTexSt, uv);
            
            renderer.SetPropertyBlock(_mpb, _targetSubMeshIndex);
        }

        private string GetName(int type, int side)
        {
            var sideLabel = side == -1 ? "Draw" : ((Side)side).ToString();
            return $"Card_{_index} (type:{(CardType)type}, set:{sideLabel})";
        }
    }
}