using System;
using BetweenRedKit.Core;
using BetweenRedKit.Integrations.Cysharp.Integrations.CySharp;
using BetweenRedKit.Integrations.Unity;
using Cysharp.Threading.Tasks;
using Shared.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Battle.UI
{
    public class BattleResultPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup background;
        [SerializeField] private Transform content;
        [SerializeField] private Image banner;
        [SerializeField] private TMP_Text label;

        [SerializeField] private Button menuButton;
        [SerializeField] private Button restartButton;

        [SerializeField] private Sprite victorySprite;
        [SerializeField] private Sprite defeatSprite;
        [SerializeField] private Sprite drawSprite;

        [SerializeField] private Color warmTextColor;
        [SerializeField] private Color coldTextColor;

        private BetweenProcessor _between;

        public Button MenuButton => menuButton;
        public Button RestartButton => restartButton;

        [Inject]
        public void Construct(BetweenProcessor between)
        {
            _between = between;
        }

        private void Start()
        {
            content.localScale = Vector3.zero;
            content.gameObject.SetActive(true);

            background.alpha = 0f;
            background.gameObject.SetActive(false);
        }

        public void SetWinner(BattleResult result)
        {
            label.text = result.ToString();

            switch (result)
            {
                case BattleResult.Victory:
                    banner.sprite = victorySprite;
                    label.color = warmTextColor;
                    break;

                case BattleResult.Defeat:
                    banner.sprite = defeatSprite;
                    label.color = coldTextColor;
                    break;

                case BattleResult.Draw:
                    banner.sprite = drawSprite;
                    label.color = warmTextColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        public async UniTask Show()
        {
            background.gameObject.SetActive(true);

            var a = content.ScaleTo(_between, Vector3.one, 0.2f).AwaitCompletion();
            var b = background.FadeTo(_between, 1f, 0.2f).AwaitCompletion();

            await UniTask.WhenAll(a, b);
        }

        public async UniTask Hide()
        {
            var a = content.ScaleTo(_between, Vector3.zero, 0.2f).AwaitCompletion();
            var b = background.FadeTo(_between, 0f, 0.2f).AwaitCompletion();

            await UniTask.WhenAll(a, b);

            background.gameObject.SetActive(false);
        }
    }
}
