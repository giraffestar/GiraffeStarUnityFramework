using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiraffeStar
{
    /// <summary>
    /// Wrapper to use dictionary collections with JsonUtility library
    /// Must be only used for serialization and deserialization
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class DictionaryWrapper<TKey, TValue>
    {
        public List<TKey> Keys;
        public List<TValue> Values;
    }

    public static class DictionaryWrapperEx
    {
        public static DictionaryWrapper<TKey, TValue> ToJsonUtilitySerializable<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException();
            }

            var wrapper = new DictionaryWrapper<TKey, TValue>
            {
                Keys = new List<TKey>(),
                Values = new List<TValue>()
            };

            foreach (var entry in dictionary)
            {
                wrapper.Keys.Add(entry.Key);
                wrapper.Values.Add(entry.Value);
            }

            return wrapper;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DictionaryWrapper<TKey, TValue> wrapper)
        {
            var dictionary = new Dictionary<TKey, TValue>();

            for (int i = 0; i < wrapper.Keys.Count; i++)
            {
                dictionary.Add(wrapper.Keys[i], wrapper.Values[i]);
            }

            return dictionary;
        }
    }
}