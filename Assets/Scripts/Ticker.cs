using BetweenRedKit.Core;
using VContainer.Unity;

public sealed class Ticker : ITickable
{
    private readonly BetweenProcessor _between;

    public Ticker(BetweenProcessor between) => _between = between;

    public void Tick() => _between.Tick();
}