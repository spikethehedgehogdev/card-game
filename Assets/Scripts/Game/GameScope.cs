using BetweenRedKit.Core;
using Framework.Services;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public class GameScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(new BetweenProcessor(initialCapacity: 128));
            builder.Register<AddressablesService>(Lifetime.Singleton);

            builder.RegisterEntryPoint<Ticker>();
            builder.RegisterEntryPoint<Bootstrap>();
        }
    }
}