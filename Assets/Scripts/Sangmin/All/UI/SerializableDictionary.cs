using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        // The internal dictionary used in runtime
        public Dictionary<TKey, TValue> KeyValuePair = new Dictionary<TKey, TValue>();

        // The lists used for serialization/deserialization in the Inspector
        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();

        // Save the dictionary to lists right before Unity serializes the object
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var pair in KeyValuePair)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // Load the dictionary from lists right after Unity deserializes the object
        public void OnAfterDeserialize()
        {
            KeyValuePair = new Dictionary<TKey, TValue>();
            for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
            {
                // Note: This implementation does not handle duplicate keys found during deserialization.
                KeyValuePair[keys[i]] = values[i];
            }
        }
    }
}