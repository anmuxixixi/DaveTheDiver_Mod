using HarmonyLib;
using JDLC;
using JDLC.RPG;
using UnityEngine;

namespace UncollectedBeetleSpawner;

[HarmonyPatch(typeof(GameObjectSelector), nameof(GameObjectSelector.GetGameObjectToSelect))]
internal static class GameObjectSelectorPatch
{
    private static readonly System.Reflection.PropertyInfo RandomObjectsProperty =
        AccessTools.Property(typeof(GameObjectSelector), "_randomObjects");
    private static readonly System.Reflection.PropertyInfo SequentialObjectsProperty =
        AccessTools.Property(typeof(GameObjectSelector), "_sequentialObjects");
    private static readonly System.Reflection.PropertyInfo SelectTypeProperty =
        AccessTools.Property(typeof(GameObjectSelector), "_selectType");
    private static bool _reportedFailure;
    private static bool _reportedSpawnGuaranteeFailure;

    internal static void ValidateInteropBindings()
    {
        if (RandomObjectsProperty == null || SequentialObjectsProperty == null || SelectTypeProperty == null)
        {
            throw new MissingMemberException("The GameObjectSelector interop properties required for insect filtering were not found.");
        }
    }

    [HarmonyPrefix]
    private static bool Prefix(GameObjectSelector __instance, ref GameObject __result, out SelectorState __state)
    {
        __state = null;
        try
        {
            var candidates = ReadCandidates(__instance);
            if (candidates.Count == 0)
            {
                return true;
            }

            // Most GameObjectSelectors have nothing to do with insects. Identify insect
            // candidates before touching Jungle save data so unrelated scenes remain vanilla.
            var insects = new List<InsectCandidate>();
            foreach (var candidate in candidates)
            {
                var insect = candidate.GameObject.GetComponentInChildren<JungleInsect>(true);
                if (insect == null)
                {
                    continue;
                }

                var spawnTid = insect.TID;
                var collectionTid = ResolveCollectionTid(spawnTid);
                var isButterfly = candidate.GameObject.GetComponentInChildren<UlyssesController>(true) != null;
                insects.Add(new InsectCandidate(
                    candidate.GameObject,
                    spawnTid,
                    collectionTid,
                    candidate.Weight,
                    isButterfly));
            }

            if (insects.Count == 0)
            {
                return true;
            }

            // The vanilla selector can include an empty result as its no-spawn chance.
            // Keep the insect candidates so the postfix can replace only that empty result.
            var isButterflySelector = insects.All(candidate => candidate.IsButterfly);
            __state = new SelectorState(insects, forceSpawnOnEmpty: !isButterflySelector);

            // Do not enumerate InsectCodexDatas here. Its generated IL2CPP IEnumerable
            // wrapper can expose an invalid Current pointer and cause an unrecoverable
            // AccessViolationException. Init also synchronizes previously looted insects
            // into IsUnlocked, even when the codex dictionary was created earlier.
            var codex = JungleInsectCodex.Instance;
            if (codex == null)
            {
                return true;
            }

            codex.Init();
            var codexDataByTid = codex._insectCodexData;
            var jungleSave = RPGUtils.GetJDLCSaveData();
            if (codexDataByTid == null && jungleSave == null)
            {
                return true;
            }

            var beetles = new List<WeightedCandidate<GameObject>>(insects.Count);
            var candidateStates = new List<string>(insects.Count);
            foreach (var candidate in insects)
            {
                JungleInsectCodexData codexData = null;
                var codexUnlocked = codexDataByTid != null &&
                                    codexDataByTid.TryGetValue(candidate.CollectionTid, out codexData) &&
                                    codexData != null &&
                                    codexData.IsUnlocked;
                var codexSaved = jungleSave != null &&
                                 jungleSave.GetJungleInsectCodexSave(candidate.CollectionTid) != null;
                var battleSaved = jungleSave != null &&
                                  jungleSave.GetJungleBattleInsectSave(candidate.CollectionTid) != null;
                var inventorySaved = jungleSave != null &&
                                     jungleSave.HasJungleVilItemSaveData(candidate.CollectionTid);
                var isCollected = codexUnlocked || codexSaved || battleSaved || inventorySaved;
                beetles.Add(new WeightedCandidate<GameObject>(
                    candidate.GameObject,
                    candidate.CollectionTid,
                    candidate.Weight,
                    isCollected));
                var evidence = string.Join("+", new[]
                {
                    codexUnlocked ? "codex" : null,
                    codexSaved ? "codex-save" : null,
                    battleSaved ? "battle-save" : null,
                    inventorySaved ? "inventory" : null,
                }.Where(value => value != null));
                candidateStates.Add(
                    $"spawn:{candidate.SpawnTid}/item:{candidate.CollectionTid}=" +
                    $"{(isCollected ? $"collected[{evidence}]" : "uncollected")}");
            }

            var hasUncollected = beetles.Any(candidate => !candidate.IsCollected);
            __state.ForceSpawnOnEmpty = InsectSpawnPolicy.ShouldForceEmptyResult(
                isButterflySelector,
                hasUncollected);

            // A completed butterfly pool returns entirely to vanilla, including its
            // original no-spawn chance. Other insects retain the existing 100% rule.
            if (!hasUncollected)
            {
                return true;
            }

            var selected = WeightedCandidateSelector.SelectUncollected(beetles, UnityEngine.Random.value);
            if (selected == null)
            {
                return true;
            }

            __result = selected;
            var selectedTid = beetles.First(candidate => candidate.Value == selected).Tid;
            Plugin.ModLog.LogInfo(
                $"Filtered insect spawn to uncollected TID {selectedTid} " +
                $"({beetles.Count(candidate => !candidate.IsCollected)}/{beetles.Count} local candidates uncollected). " +
                $"Candidate states: {string.Join(", ", candidateStates)}.");
            return false;
        }
        catch (Exception exception)
        {
            // A game update should degrade to vanilla spawning instead of breaking scene loading.
            if (!_reportedFailure)
            {
                _reportedFailure = true;
                Plugin.ModLog.LogWarning($"Could not filter an insect spawn; using the original game logic. {exception}");
            }

            return true;
        }
    }

    [HarmonyPostfix]
    private static void Postfix(ref GameObject __result, SelectorState __state)
    {
        if (__state == null || !__state.ForceSpawnOnEmpty || __result != null)
        {
            return;
        }

        try
        {
            var candidates = __state.Insects
                .Select(candidate => new WeightedCandidate<GameObject>(
                    candidate.GameObject,
                    candidate.CollectionTid,
                    candidate.Weight,
                    false))
                .ToList();
            var selected = WeightedCandidateSelector.SelectAny(candidates, UnityEngine.Random.value);
            if (selected == null)
            {
                return;
            }

            __result = selected;
            var selectedTid = candidates.First(candidate => candidate.Value == selected).Tid;
            Plugin.ModLog.LogInfo($"Forced insect spawn at a vanilla no-spawn roll. Selected TID {selectedTid}.");
        }
        catch (Exception exception)
        {
            if (!_reportedSpawnGuaranteeFailure)
            {
                _reportedSpawnGuaranteeFailure = true;
                Plugin.ModLog.LogWarning($"Could not force an insect spawn; keeping the original result. {exception}");
            }
        }
    }

    private static List<RawCandidate> ReadCandidates(GameObjectSelector selector)
    {
        var result = new List<RawCandidate>();
        var selectType = Convert.ToInt32(SelectTypeProperty?.GetValue(selector) ?? 0);

        if (selectType is 0 or 2)
        {
            ReadList(RandomObjectsProperty?.GetValue(selector), item =>
            {
                var type = item.GetType();
                var target = AccessTools.Property(type, "Targe")?.GetValue(item) as GameObject;
                var weightValue = AccessTools.Property(type, "RandomWeight")?.GetValue(item);
                if (target != null)
                {
                    result.Add(new RawCandidate(target, weightValue is float weight ? weight : 1f));
                }
            });
        }

        if (selectType == 1)
        {
            ReadList(SequentialObjectsProperty?.GetValue(selector), item =>
            {
                if (item is GameObject target)
                {
                    result.Add(new RawCandidate(target, 1f));
                }
            });
        }

        return result;
    }

    private static void ReadList(object list, Action<object> readItem)
    {
        if (list == null)
        {
            return;
        }

        var type = list.GetType();
        var countProperty = AccessTools.Property(type, "Count");
        var itemProperty = AccessTools.Property(type, "Item");
        if (countProperty == null || itemProperty == null)
        {
            return;
        }

        var count = (int)(countProperty.GetValue(list) ?? 0);
        for (var index = 0; index < count; index++)
        {
            var item = itemProperty.GetValue(list, new object[] { index });
            if (item != null)
            {
                readItem(item);
            }
        }
    }

    internal static int ResolveCollectionTid(int spawnTid)
    {
        // JungleInsect.TID is the interaction-drop row (11200xx), while the
        // codex and save dictionaries are keyed by the dropped item (410103xx).
        // The game itself performs this same lookup when an insect is caught.
        var drop = DataManager.Instance?.GetJungleInteractionDrop(spawnTid);
        var itemTids = drop?.ItemDropListTID;
        if (itemTids != null)
        {
            // Il2CppSystem.IReadOnlyList<T> does not expose Count through the
            // generated compile-time interface. Its concrete List<T> still has
            // safe Count/Item properties, so read those without enumerating it.
            var listType = itemTids.GetType();
            var countProperty = AccessTools.Property(listType, "Count");
            var itemProperty = AccessTools.Property(listType, "Item");
            var count = (int)(countProperty?.GetValue(itemTids) ?? 0);
            if (count > 0 && itemProperty?.GetValue(itemTids, new object[] { 0 }) is int itemTid)
            {
                return itemTid;
            }
        }

        // Current insect rows have a stable one-to-one suffix. Keep this as a
        // defensive fallback so a temporarily unavailable data manager does not
        // make every insect appear uncollected again during scene startup.
        return InsectIdMapping.GetFallbackCollectionTid(spawnTid);
    }

    internal static bool IsCollected(int collectionTid)
    {
        var codex = JungleInsectCodex.Instance;
        codex?.Init();

        JungleInsectCodexData codexData = null;
        var codexDataByTid = codex?._insectCodexData;
        var codexUnlocked = codexDataByTid != null &&
                            codexDataByTid.TryGetValue(collectionTid, out codexData) &&
                            codexData != null &&
                            codexData.IsUnlocked;

        var jungleSave = RPGUtils.GetJDLCSaveData();
        return codexUnlocked ||
               jungleSave?.GetJungleInsectCodexSave(collectionTid) != null ||
               jungleSave?.GetJungleBattleInsectSave(collectionTid) != null ||
               jungleSave?.HasJungleVilItemSaveData(collectionTid) == true;
    }

    private readonly record struct RawCandidate(GameObject GameObject, float Weight);
    private readonly record struct InsectCandidate(
        GameObject GameObject,
        int SpawnTid,
        int CollectionTid,
        float Weight,
        bool IsButterfly);
    private sealed class SelectorState
    {
        internal SelectorState(List<InsectCandidate> insects, bool forceSpawnOnEmpty)
        {
            Insects = insects;
            ForceSpawnOnEmpty = forceSpawnOnEmpty;
        }

        internal List<InsectCandidate> Insects { get; }
        internal bool ForceSpawnOnEmpty { get; set; }
    }
}
