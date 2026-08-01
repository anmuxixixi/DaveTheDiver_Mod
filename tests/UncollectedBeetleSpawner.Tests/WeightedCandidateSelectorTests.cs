namespace UncollectedBeetleSpawner;

using Xunit;

public class WeightedCandidateSelectorTests
{
    [Fact]
    public void SelectUncollected_NeverReturnsCollectedCandidate()
    {
        var candidates = new[]
        {
            new WeightedCandidate<string>("collected", 1, 100f, true),
            new WeightedCandidate<string>("missing-a", 2, 1f, false),
            new WeightedCandidate<string>("missing-b", 3, 1f, false),
        };

        Assert.Equal("missing-a", WeightedCandidateSelector.SelectUncollected(candidates, 0.1f));
        Assert.Equal("missing-b", WeightedCandidateSelector.SelectUncollected(candidates, 0.9f));
    }

    [Fact]
    public void SelectUncollected_ReturnsDefaultWhenAreaIsComplete()
    {
        var candidates = new[]
        {
            new WeightedCandidate<string>("collected", 1, 1f, true),
        };

        Assert.Null(WeightedCandidateSelector.SelectUncollected(candidates, 0.5f));
    }

    [Fact]
    public void SelectUncollected_PreservesWeightsAmongMissingCandidates()
    {
        var candidates = new[]
        {
            new WeightedCandidate<string>("common", 1, 3f, false),
            new WeightedCandidate<string>("rare", 2, 1f, false),
        };

        Assert.Equal("common", WeightedCandidateSelector.SelectUncollected(candidates, 0.74f));
        Assert.Equal("rare", WeightedCandidateSelector.SelectUncollected(candidates, 0.76f));
    }

    [Fact]
    public void SelectUncollected_UsesUniformFallbackWhenAllWeightsAreZero()
    {
        var candidates = new[]
        {
            new WeightedCandidate<string>("first", 1, 0f, false),
            new WeightedCandidate<string>("second", 2, -1f, false),
        };

        Assert.Equal("first", WeightedCandidateSelector.SelectUncollected(candidates, 0.1f));
        Assert.Equal("second", WeightedCandidateSelector.SelectUncollected(candidates, 0.9f));
    }

    [Fact]
    public void SelectAny_CanForceSpawnFromCollectedCandidates()
    {
        var candidates = new[]
        {
            new WeightedCandidate<string>("common", 1, 3f, true),
            new WeightedCandidate<string>("rare", 2, 1f, true),
        };

        Assert.Equal("common", WeightedCandidateSelector.SelectAny(candidates, 0.74f));
        Assert.Equal("rare", WeightedCandidateSelector.SelectAny(candidates, 0.76f));
    }
}
