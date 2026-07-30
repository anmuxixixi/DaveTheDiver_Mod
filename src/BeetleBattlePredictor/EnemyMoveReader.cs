using InsectBattle;
using UnityEngine;

namespace BeetleBattlePredictor;

internal static class EnemyMoveReader
{
    private static float _nextErrorLogAt;

    internal static void Poll()
    {
        try
        {
            var manager = InsectBattleManager.Instance;
            var enemy = manager?.EnemyBattleInsect;
            if (manager is null || enemy is null)
            {
                PredictionState.Clear();
                return;
            }

            var stateManager = InsectBattleStateManager.Instance;
            if (stateManager is null ||
                stateManager.CurrentState != InsectBattleStateManager.InsectBattleState.Waiting ||
                manager.IsSelectedBehaviour)
            {
                return;
            }

            // The base game generates the enemy move only after the player confirms.
            // Generate it at the first focus event instead, then mark it as selected so
            // the original confirmation handler keeps this exact result rather than rerolling.
            if (!manager.IsSelectEnemyBehaviour)
            {
                var player = manager.EntryPlayerBattleInsect;
                if (player is null)
                {
                    return;
                }

                var playerMove = (BeetleMove)(int)player.CurrentBehaviourType;
                if (playerMove is < BeetleMove.PaperDefense or > BeetleMove.RockRush)
                {
                    return;
                }

                var controller = UnityEngine.Object.FindObjectOfType<WaitingStateController>();
                if (controller is null)
                {
                    return;
                }

                controller.SetRandomEnemyBehaviourType();
                manager.IsSelectEnemyBehaviour = true;
                Plugin.ModLog.LogInfo("Enemy move generated early and locked before confirmation.");
            }

            var move = (BeetleMove)(int)enemy.CurrentBehaviourType;
            if (move is < BeetleMove.PaperDefense or > BeetleMove.RockRush)
            {
                return;
            }

            var turn = manager.NowTurn?.Value ?? -1;
            if (PredictionState.OpponentMove == move && PredictionState.Turn == turn)
            {
                return;
            }

            PredictionState.Set(move, turn);
            Plugin.ModLog.LogInfo($"Locked enemy move: {move}, turn {turn}");
        }
        catch (Exception ex)
        {
            PredictionState.Clear();
            if (Time.unscaledTime >= _nextErrorLogAt)
            {
                _nextErrorLogAt = Time.unscaledTime + 5f;
                Plugin.ModLog.LogWarning($"Early prediction failed: {ex}");
            }
        }
    }
}
