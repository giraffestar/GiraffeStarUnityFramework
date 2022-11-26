using System.Collections.Generic;
using UnityEngine;

namespace GiraffeStar
{
    public static class JsonUtilityEx
    {
        public static string DictionaryToJson<TKey, TValue>(Dictionary<TKey, TValue> dictionary, bool prettyPrint)
        {
            var wrapper = dictionary.ToJsonUtilitySerializable();
            var serialized = JsonUtility.ToJson(wrapper, true);

            return serialized;
        }

        public static Dictionary<TKey, TValue> DictionaryFromJson<TKey, TValue>(string serialized)
        {
            var deserialized = JsonUtility.FromJson<DictionaryWrapper<TKey, TValue>>(serialized);
            var dictionary = deserialized.ToDictionary();

            return dictionary;
        }
    }
}