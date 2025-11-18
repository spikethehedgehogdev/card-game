using BetweenRedKit.Core;
using BetweenRedKit.Integrations.Cysharp.Integrations.CySharp;
using BetweenRedKit.Integrations.Unity;
using Cysharp.Threading.Tasks;
using Framework.Services;
using Game.Battle.Flow;
using R3;
using Shared.Enum;
using Shared.Generated;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.Battle.UI
{
    public class BattleUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private BattleScorePanel scorePanel;
        [SerializeField] private BattleResultPanel resultPanel;

        [Header("Buttons")]
        [SerializeField] private Button exitButton;

        public BattleScorePanel ScorePanel => scorePanel;
        public BattleResultPanel ResultPanel => resultPanel;

        private AddressablesService _addressables;
        private BetweenProcessor _between;
        private IJudge _judge;

        private readonly CompositeDisposable _disposables = new();

        // -----------------------------------------------------------
        // DI
        // -----------------------------------------------------------

        [Inject]
        public void Construct(
            AddressablesService addressables,
            BetweenProcessor between,
            IJudge judge)
        {
            _addressables = addressables;
            _between = between;

            _judge = judge;
            _judge.OnScoreChanged += OnScoreChanged;
            _judge.OnRoundResult += OnRoundResult;
        }

        // -----------------------------------------------------------
        // Unity
        // -----------------------------------------------------------

        private void Start()
        {
            exitButton.OnClickAsObservable()
                .Subscribe(async _ => await _addressables.LoadSceneAsync(Scenes.Menu))
                .AddTo(_disposables);

            resultPanel.MenuButton.OnClickAsObservable()
                .Subscribe(async _ => await _addressables.LoadSceneAsync(Scenes.Menu))
                .AddTo(_disposables);

            resultPanel.RestartButton.OnClickAsObservable()
                .Subscribe(async _ =>
                {
                    // вызываем рестарт через правильно найденный BattleEntryPoint
                    var scope = LifetimeScope.Find<BattleScope>();
                    var orchestrator = scope.Container.Resolve<Orchestrator>();
                    await orchestrator.StartBattleAsync(this.GetCancellationTokenOnDestroy());
                })
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _judge.OnScoreChanged -= OnScoreChanged;
            _judge.OnRoundResult -= OnRoundResult;

            _disposables.Dispose();
        }

        // -----------------------------------------------------------
        // UI updates (events from judge)
        // -----------------------------------------------------------

        private void OnScoreChanged(int player, int opponent)
        {
            scorePanel.UpdateScore(player, opponent);
        }

        private void OnRoundResult(int winner)
        {
            // Можно добавить визуальные эффекты, звук, анимацию
        }

        // -----------------------------------------------------------
        // UI animations
        // -----------------------------------------------------------

        public async UniTask ShowBattleResult(BattleResult result)
        {
            resultPanel.SetWinner(result);

            var hideScore = scorePanel.transform
                .ScaleTo(_between, Vector3.zero, 0.2f)
                .AwaitCompletion();

            var hideExit = exitButton.transform
                .ScaleTo(_between, Vector3.zero, 0.2f)
                .AwaitCompletion();

            var showPanel = resultPanel.Show();

            await UniTask.WhenAll(hideScore, hideExit, showPanel);
        }

        public async UniTask HideBattleResult()
        {
            await resultPanel.Hide();

            scorePanel.transform.localScale = Vector3.one;
            exitButton.transform.localScale = Vector3.one;
        }
    }
}
