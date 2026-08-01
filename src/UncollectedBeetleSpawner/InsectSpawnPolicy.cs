namespace UncollectedBeetleSpawner;

internal static class InsectSpawnPolicy
{
    internal static bool ShouldForceEmptyResult(bool isButterflySelector, bool hasUncollected)
    {
        return !isButterflySelector || hasUncollected;
    }
}
