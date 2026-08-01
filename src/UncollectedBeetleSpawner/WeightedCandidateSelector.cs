namespace UncollectedBeetleSpawner;

internal readonly record struct WeightedCandidate<T>(T Value, int Tid, float Weight, bool IsCollected);

internal static class WeightedCandidateSelector
{
    internal static T SelectUncollected<T>(
        IReadOnlyList<WeightedCandidate<T>> candidates,
        float unitRandom)
    {
        var eligible = candidates.Where(candidate => !candidate.IsCollected).ToArray();
        return SelectEligible(eligible, unitRandom);
    }

    internal static T SelectAny<T>(
        IReadOnlyList<WeightedCandidate<T>> candidates,
        float unitRandom)
    {
        return SelectEligible(candidates, unitRandom);
    }

    private static T SelectEligible<T>(
        IReadOnlyList<WeightedCandidate<T>> eligible,
        float unitRandom)
    {
        if (eligible.Count == 0)
        {
            return default!;
        }

        var totalWeight = eligible.Sum(candidate => Math.Max(0f, candidate.Weight));
        if (totalWeight <= 0f)
        {
            var index = Math.Min((int)(Math.Clamp(unitRandom, 0f, 0.999999f) * eligible.Count), eligible.Count - 1);
            return eligible[index].Value;
        }

        var target = Math.Clamp(unitRandom, 0f, 0.999999f) * totalWeight;
        foreach (var candidate in eligible)
        {
            target -= Math.Max(0f, candidate.Weight);
            if (target < 0f)
            {
                return candidate.Value;
            }
        }

        return eligible[^1].Value;
    }
}
