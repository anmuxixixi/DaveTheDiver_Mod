namespace BeetleBattlePredictor;

public enum BeetleMove
{
    None = -1,
    PaperDefense = 0,
    ScissorsHornAttack = 1,
    RockRush = 2,
}

internal readonly record struct PredictionText(string Opponent, string Counter, string ShortCounter);

internal static class Prediction
{
    internal static PredictionText Describe(BeetleMove move) => move switch
    {
        BeetleMove.PaperDefense => new("布 · 防御/回血", "剪刀 · 角攻击", "出剪刀"),
        BeetleMove.ScissorsHornAttack => new("剪刀 · 角攻击", "石头 · 冲锋", "出石头"),
        BeetleMove.RockRush => new("石头 · 冲锋", "布 · 防御/回血", "出布"),
        _ => new("尚未生成", "等待对手选择", "等待"),
    };

    internal static int CalculateRoundNumber(int maxTurnCount, int turnsRemaining)
    {
        if (maxTurnCount < 0 || turnsRemaining < 0 || turnsRemaining > maxTurnCount)
        {
            return -1;
        }

        // The game stores a countdown: on round 1 both values are 9, then
        // turnsRemaining decreases until round 10 where it reaches 0.
        return maxTurnCount - turnsRemaining + 1;
    }
}

internal static class PredictionState
{
    internal static BeetleMove OpponentMove { get; private set; } = BeetleMove.None;
    internal static int TurnsRemaining { get; private set; } = -1;
    internal static int RoundNumber { get; private set; } = -1;
    internal static bool HasPrediction => OpponentMove != BeetleMove.None;

    internal static void Set(BeetleMove move, int turnsRemaining, int roundNumber)
    {
        OpponentMove = move;
        TurnsRemaining = turnsRemaining;
        RoundNumber = roundNumber;
    }

    internal static void Clear()
    {
        OpponentMove = BeetleMove.None;
        TurnsRemaining = -1;
        RoundNumber = -1;
    }
}
