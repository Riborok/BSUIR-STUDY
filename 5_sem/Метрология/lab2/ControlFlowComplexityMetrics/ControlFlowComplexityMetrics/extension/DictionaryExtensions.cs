using System.Collections.Generic;

namespace ControlFlowComplexityMetrics.extension {
    public static class DictionaryExtensions {
        public static void ChangeKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey oldKey, TKey newKey) {
            if (!dictionary.TryGetValue(oldKey, out var value)) {
                return;
            }

            dictionary.Remove(oldKey);
            dictionary[newKey] = value;
        }
    }
}