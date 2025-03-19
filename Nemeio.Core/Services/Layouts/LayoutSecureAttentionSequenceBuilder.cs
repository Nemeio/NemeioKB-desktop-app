using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;

namespace Nemeio.Core.Services.Layouts
{
    public static class LayoutSecureAttentionSequenceBuilder
    {
        /// <summary>
        /// Manage case :
        /// - Ctrl, Alt and Suppr on individual keys (only on None position)
        /// - All in one keys
        /// </summary>
        public static SpecialSequences Build(ILayout layout)
        {
            if (layout == null) 
            {
                throw new ArgumentNullException(nameof(layout));
            }

            var keys = layout.Keys;
            var layoutInfo = layout.LayoutInfo;

            if (keys == null)
            {
                return GetSAS(layoutInfo);
            }

            if (keys.Count == 0)
            {
                return GetSAS(layoutInfo);
            }

            try
            {
                var result = new Permutation<int>();

                var delKeys = SelectOnly(keys, KeyboardLiterals.Delete);
                if (delKeys != null && delKeys.Count() > 0)
                {
                    var ctrlKeys    = SelectOnly(keys, KeyboardLiterals.Ctrl);
                    var altKeys     = SelectOnly(keys, KeyboardLiterals.Alt);
                    var altGrKeys   = SelectOnly(keys, KeyboardLiterals.AltGr);

                    var globalMerge = new List<List<int>>();
                    foreach (var delKey in delKeys)
                    {
                        var mergeCtrlAlt = GetPermutations(
                            ctrlKeys.Concat(altKeys),
                            2
                        );

                        foreach (var permutation in mergeCtrlAlt.Values)
                        {
                            permutation.Add(delKey);
                        }

                        globalMerge = globalMerge.Concat(mergeCtrlAlt.Values).ToList();

                        var mergeCtrlAltGr = GetPermutations(
                            ctrlKeys.Concat(altGrKeys),
                            2
                        );

                        foreach (var permutation in mergeCtrlAltGr.Values)
                        {
                            permutation.Add(delKey);
                        }

                        globalMerge = globalMerge.Concat(mergeCtrlAltGr.Values).ToList();
                    }

                    result = result.Concat(globalMerge).ToPermutation();
                }

                var ctrlAltSuppr = SelectOnly(
                    keys, 
                    new List<string>()
                    {
                        KeyboardLiterals.Ctrl,
                        KeyboardLiterals.Alt,
                        KeyboardLiterals.Delete
                    }
                );

                var ctrlAltGrSuppr = SelectOnly(
                    keys,
                    new List<string>()
                    {
                        KeyboardLiterals.Ctrl,
                        KeyboardLiterals.AltGr,
                        KeyboardLiterals.Delete
                    }
                );

                var altCtrlSuppr = SelectOnly(
                    keys,
                    new List<string>()
                    {
                        KeyboardLiterals.Alt,
                        KeyboardLiterals.Ctrl,
                        KeyboardLiterals.Delete
                    }
                );

                var altGrCtrlSuppr = SelectOnly(
                    keys,
                    new List<string>()
                    {
                        KeyboardLiterals.AltGr,
                        KeyboardLiterals.Ctrl,
                        KeyboardLiterals.Delete
                    }
                );

                //  DeviceList of only one keys
                var merge = ctrlAltSuppr
                    .Concat(ctrlAltGrSuppr)
                    .Concat(altCtrlSuppr)
                    .Concat(altGrCtrlSuppr)
                    .ToList();

                if (merge.Count > 0)
                {
                    result.Add(merge);
                }

                if (!result.Any())
                {
                    return GetSAS(layoutInfo);
                }

                result.ForEach((items) =>
                {
                    items.ForEach((val) => { val += 1; });
                });

                return new SpecialSequences(result);
            }
            catch (ArgumentNullException)
            {
                //  If JSON is malformed by configurator, some data can by corrupted

                return GetSAS(layoutInfo);
            }
        }

        public static Permutation<int> GetPermutations(IEnumerable<int> list, int length)
        {
            if (length == 1)
            {
                return list.Select(t => new int[] { t }.ToList()).ToPermutation();
            }

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new int[] { t2 }).ToList()).ToPermutation();
        }
        private static SpecialSequences GetSAS(LayoutInfo infos) => infos != null && infos.Hid ? SpecialSequences.Default : SpecialSequences.Empty;

        private static IEnumerable<int> SelectOnly(IList<Key> keys, string keyToFind) => keys
            .Where(x => GetActionWithNoneAsModifier(x.Actions).Any(y => SubactionHasKey(y, keyToFind)))
            .Select(x => x.Index);

        private static IEnumerable<KeyAction> GetActionWithNoneAsModifier(IEnumerable<KeyAction> actions)
        {
            if (actions == null)
            {
                return Enumerable.Empty<KeyAction>();
            }

            return actions.Where((action) =>
            {
                if (action != null)
                {
                    return action.Modifier == KeyboardModifier.None;
                }

                return false;
            });
        }

        private static bool SubactionHasKey(KeyAction action, string key)
        {
            if (action == null || key == null)
            {
                return false;
            }

            if (action.Subactions == null)
            {
                return false;
            }

            return action.Subactions.Any((z) =>
            {
                var data = z.Data;

                if (string.IsNullOrEmpty(data))
                {
                    return false;
                }

                return key == data;
            });
        }

        private static IEnumerable<int> SelectOnly(IList<Key> keys, List<string> keyToFound)
        {
            var validActions = keys
                .SelectMany(x => GetActionWithNoneAsModifier(x.Actions))
                .Where(action => IsMatching(action.Subactions.ToList(), keyToFound));

            return keys.Where(x => x.Actions.All(s => validActions.Contains(s))).Select(x => x.Index);
        }

        private static bool IsMatching(List<KeySubAction> subActions, List<string> keys)
        {
            if (subActions == null || keys == null)
            {
                return false;
            }

            if (subActions.Count == 0 || keys.Count == 0)
            {
                return false;
            }

            var subsStr = subActions.Select(x => x.Data).Aggregate((i, j) => i + ";" + j);
            var keysStr = keys.Aggregate((i, j) => i + ";" + j);

            if (subsStr == null || keysStr == null)
            {
                return false;
            }

            return subsStr.Contains(keysStr);
        }
    }
}
