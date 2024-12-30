using System;
using System.Collections.Generic;

namespace Core.Factions
{
    public class FactionRuntimeData
    {
        private readonly Dictionary<object, object> _genericRuntimeData = new();

        public void Write(object key, object value)
        {
            if (!_genericRuntimeData.TryAdd(key, value))
            {
                _genericRuntimeData[key] = value;
            }
        }

        public bool HasKey(object key)
        {
            return _genericRuntimeData.ContainsKey(key);
        }

        public T Read<T>(object key)
        {
            if (!_genericRuntimeData.TryGetValue(key, out var value))
            {
                throw new KeyNotFoundException("The key was not found in the dictionary.");
            }

            if (value is T castedValue)
            {
                return castedValue;
            }

            throw new InvalidCastException($"The value associated with the key cannot be cast to type {typeof(T)}.");
        }
    }
}