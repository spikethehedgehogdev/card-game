using UnityEngine;

namespace Game.Battle.Cards
{
    public interface ICardPrefab
    {
        Transform Transform { get; }
        Collider Collider { get; }
        MeshRenderer Renderer { get; }
    }

    [RequireComponent(typeof(Collider))]
    public class CardPrefab : MonoBehaviour, ICardPrefab
    {
        [SerializeField] private Collider pCollider;
        [SerializeField] private MeshRenderer pRenderer;

        public Transform Transform => transform;
        public Collider Collider => pCollider;
        public MeshRenderer Renderer => pRenderer;
    }
}