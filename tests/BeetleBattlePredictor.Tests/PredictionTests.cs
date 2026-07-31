using Xunit;

namespace BeetleBattlePredictor;

public sealed class PredictionTests
{
    [Theory]
    [InlineData(BeetleMove.PaperDefense, "剪刀", "出剪刀")]
    [InlineData(BeetleMove.ScissorsHornAttack, "石头", "出石头")]
    [InlineData(BeetleMove.RockRush, "布", "出布")]
    public void Describe_ReturnsWinningCounter(BeetleMove move, string counter, string shortCounter)
    {
        var result = Prediction.Describe(move);
        Assert.Contains(counter, result.Counter);
        Assert.Equal(shortCounter, result.ShortCounter);
    }

    [Theory]
    [InlineData(9, 9, 1)]
    [InlineData(9, 4, 6)]
    [InlineData(9, 0, 10)]
    [InlineData(-1, 9, -1)]
    [InlineData(9, -1, -1)]
    [InlineData(9, 10, -1)]
    public void CalculateRoundNumber_ConvertsCountdownToAscendingRound(
        int maxTurnCount,
        int turnsRemaining,
        int expected)
    {
        Assert.Equal(expected, Prediction.CalculateRoundNumber(maxTurnCount, turnsRemaining));
    }
}
