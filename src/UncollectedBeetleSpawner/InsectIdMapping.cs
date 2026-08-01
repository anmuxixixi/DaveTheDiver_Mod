namespace UncollectedBeetleSpawner;

internal static class InsectIdMapping
{
    internal static int GetFallbackCollectionTid(int spawnTid)
    {
        return spawnTid is >= 1120002 and <= 1120036
            ? 41010300 + (spawnTid - 1120000)
            : spawnTid;
    }
}
