using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Services;
using Game;
using Game.Battle.Config;
using Shared.Generated;
using VContainer.Unity;

public class Bootstrap : IAsyncStartable
{
    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var gameScope = LifetimeScope.Find<GameScope>();

        var addressables = await AddressablesService.InitializeAsync(gameScope);

        ConfigsCache.Hand = await addressables.LoadConfig<HandConfig>("HandConfig");
        ConfigsCache.Flow = await addressables.LoadConfig<BattleFlowConfig>("BattleFlowConfig");
        ConfigsCache.Board = await addressables.LoadConfig<BoardConfig>("BoardConfig");

        await addressables.LoadSceneAsync(Scenes.Menu);
    }
}

public static class ConfigsCache
{
    public static HandConfig Hand { get; set; }
    public static BattleFlowConfig Flow { get; set; }
    public static BoardConfig Board { get; set; }
}