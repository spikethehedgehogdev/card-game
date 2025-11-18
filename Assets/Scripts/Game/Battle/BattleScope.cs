using BetweenRedKit.Core;
using Framework.Services;
using Game.Battle.Battler;
using Game.Battle.Cards;
using Game.Battle.Config;
using Game.Battle.Flow;
using Game.Battle.UI;
using Game.Battle.World;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Battle
{
    public class BattleScope : LifetimeScope
    {
        [Header("Prefabs")]
        [SerializeField] private CardPrefab cardPrefab;

        [Header("Scene References")]
        [SerializeField] private BoardView boardView;
        [SerializeField] private BattleUI ui;
        [SerializeField] private Camera mainCamera;

        protected override void Configure(IContainerBuilder builder)
        {
            //
            // 1. UI
            //
            builder.RegisterInstance(ui).As<BattleUI>();

            //
            // 2. Scene references
            //
            builder.RegisterInstance(mainCamera);

            //
            // 3. Configs (из Bootstrap через ConfigsCache)
            //
            builder.RegisterInstance(ConfigsCache.Hand);
            builder.RegisterInstance(ConfigsCache.Flow);
            builder.RegisterInstance(ConfigsCache.Board);

            //
            // 4. Services
            //
            builder.Register<IInput, InputService>(Lifetime.Singleton);
            builder.Register<CardCollision>(Lifetime.Singleton).AsSelf();
            builder.Register<ICardFactory>(resolver =>
                new CardFactory(cardPrefab, resolver.Resolve<BetweenProcessor>()),
                Lifetime.Singleton);

            //
            // 5. Board
            //
            builder.Register<IBoard>(resolver =>
            {
                var cfg = resolver.Resolve<BoardConfig>();
                return new Board(boardView, cfg);
            }, Lifetime.Singleton);

            //
            // 6. Deck
            //
            builder.Register<IDeck, Deck>(Lifetime.Singleton);

            //
            // 7. Player & Opponent
            //
            builder.Register(resolver =>
            {
                var config = resolver.Resolve<HandConfig>();
                var board = resolver.Resolve<IBoard>();

                var curve = new BezierCurve(
                    board.PlayerSidePreset.HandRoot,
                    board.EdgeOffsetX,
                    board.MiddleOffset);

                var hand = new Hand(config, curve);
                var cardCollision = resolver.Resolve<CardCollision>();

                return new Player(hand, cardCollision);

            }, Lifetime.Singleton).AsSelf();

            builder.Register(resolver =>
            {
                var config = resolver.Resolve<HandConfig>();
                var board = resolver.Resolve<IBoard>();

                var curve = new BezierCurve(
                    board.OpponentSidePreset.HandRoot,
                    board.EdgeOffsetX,
                    board.MiddleOffset);

                var hand = new Hand(config, curve);
                return new Opponent(hand);

            }, Lifetime.Singleton);

            //
            // 8. Battle Logic
            //
            builder.Register<IDealer, Dealer>(Lifetime.Singleton);
            builder.Register<IJudge, MatrixJudge>(Lifetime.Singleton);
            builder.Register<Orchestrator>(Lifetime.Singleton);

            //
            // 9. Entry Points
            //
            builder.RegisterEntryPoint<BattleEntryPoint>();
        }
    }
}
