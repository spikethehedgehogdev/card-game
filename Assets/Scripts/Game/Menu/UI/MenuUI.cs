using Framework.Services;
using R3;
using Shared.Generated;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Menu.UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button exitButton;

        private AddressablesService _addressables;
        
        private readonly CompositeDisposable _disposables = new();
        
        [Inject]
        public void Construct(AddressablesService addressables)
        {
            _addressables = addressables;
        }

        private void Start()
        {
            startButton.OnClickAsObservable()
                .Subscribe(async _ => await _addressables.LoadSceneAsync(Scenes.Battle))
                .AddTo(_disposables);

            exitButton.OnClickAsObservable()
#if UNITY_WEBGL
                .Subscribe(_ => Debug.Log("Exit not supported on WebGL."))
#else
                .Subscribe(_ => Application.Quit())
#endif
                .AddTo(_disposables);
        }
        
        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}