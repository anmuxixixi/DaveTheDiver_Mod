namespace UncollectedBeetleSpawner;

using Xunit;

public class InsectSpawnPolicyTests
{
    [Fact]
    public void ButterflyPoint_IsForcedWhenAnUncollectedButterflyExists()
    {
        Assert.True(InsectSpawnPolicy.ShouldForceEmptyResult(
            isButterflySelector: true,
            hasUncollected: true));
    }

    [Fact]
    public void ButterflyPoint_ReturnsToVanillaWhenCollectionIsComplete()
    {
        Assert.False(InsectSpawnPolicy.ShouldForceEmptyResult(
            isButterflySelector: true,
            hasUncollected: false));
    }

    [Fact]
    public void ExistingBeetleRule_RemainsForcedWhenCollectionIsComplete()
    {
        Assert.True(InsectSpawnPolicy.ShouldForceEmptyResult(
            isButterflySelector: false,
            hasUncollected: false));
    }
}
