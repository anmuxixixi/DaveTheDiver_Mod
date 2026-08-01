using HarmonyLib;
using JDLC;

namespace UncollectedBeetleSpawner;

[HarmonyPatch(typeof(JungleSpawner), nameof(JungleSpawner.TrySpawnObject))]
internal static class JungleSpawnerPatch
{
    private static bool _reportedFailure;

    [HarmonyPrefix]
    private static void Prefix(JungleSpawner __instance)
    {
        try
        {
            // Butterflies in the forest are fixed JungleSpawners rather than
            // GameObjectSelectors. Preserve their original weather activation,
            // but allow an uncollected species to respawn if this point was
            // marked consumed or removed earlier in the current instance data.
            if (__instance == null || __instance.useTargetSelector || __instance.TargetPrefab == null)
            {
                return;
            }

            if (__instance.TargetPrefab.GetComponentInChildren<UlyssesController>(true) == null)
            {
                return;
            }

            var spawnTid = __instance.DataTID;
            var collectionTid = GameObjectSelectorPatch.ResolveCollectionTid(spawnTid);
            if (GameObjectSelectorPatch.IsCollected(collectionTid))
            {
                return;
            }

            var manager = JVillageSpawnManager.CurrentSpawnManager;
            var spawnData = manager?.GetSpawnData(__instance.UniqueID);
            if (spawnData == null ||
                spawnData.State is not (JVillageSpawnData.SpawnState.Removed or JVillageSpawnData.SpawnState.Consumed))
            {
                return;
            }

            var previousState = spawnData.State;
            spawnData.State = JVillageSpawnData.SpawnState.None;
            spawnData.ConsumeCount = 0;
            Plugin.ModLog.LogInfo(
                $"Re-enabled fixed uncollected butterfly spawn:{spawnTid}/item:{collectionTid} " +
                $"from state {previousState}.");
        }
        catch (Exception exception)
        {
            if (!_reportedFailure)
            {
                _reportedFailure = true;
                Plugin.ModLog.LogWarning(
                    $"Could not re-enable a fixed butterfly spawn; using the original game state. {exception}");
            }
        }
    }
}
