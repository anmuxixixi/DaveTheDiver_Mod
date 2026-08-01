namespace UncollectedBeetleSpawner;

using Xunit;

public class InsectIdMappingTests
{
    [Theory]
    [InlineData(1120002, 41010302)]
    [InlineData(1120027, 41010327)]
    [InlineData(1120036, 41010336)]
    public void KnownSpawnTid_MapsToCollectionItemTid(int spawnTid, int expectedItemTid)
    {
        Assert.Equal(expectedItemTid, InsectIdMapping.GetFallbackCollectionTid(spawnTid));
    }

    [Fact]
    public void UnknownTid_IsLeftUnchanged()
    {
        Assert.Equal(999, InsectIdMapping.GetFallbackCollectionTid(999));
    }
}
