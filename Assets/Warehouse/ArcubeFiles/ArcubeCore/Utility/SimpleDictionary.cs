using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arcube
{
    [System.Serializable]
    public class DictionaryItem<U, T> where T : class
    {
        public U key;
        public T item;
    }
    
    [System.Serializable]
    public class SimpleDictionary<U, T> where T : class
    {
        [SerializeField] private List<DictionaryItem<U, T>> items = new();
        private Dictionary<U, T> _lookup;
        private void EnsureLookup()
        {
            if (_lookup != null) return;

            _lookup = new Dictionary<U, T>();
            foreach (var item in items)
            {
                if (!_lookup.ContainsKey(item.key))
                {
                    _lookup.Add(item.key, item.item);
                }
                else
                {
                    Debug.LogWarning($"Duplicate key: {item.key}");
                }
            }
        }
        
        public T this[U key]
        {
            get
            {
                EnsureLookup();

                if (_lookup.TryGetValue(key, out var value))
                    return value;

                Debug.LogWarning($"Key '{key}' not found.");
                return null;
            }
        }
        
        public bool TryGetValue(U key, out T value)
        {
            EnsureLookup();
            return _lookup.TryGetValue(key, out value);
        }
        
        public void Add(U key, T item)
        {
            EnsureLookup();
            _lookup.Add(key, item);
            items.Add(new DictionaryItem<U, T> { key = key, item = item });
        }

        public void Remove(U key)
        {
            EnsureLookup();
            _lookup.Remove(key);
            items.RemoveAll(item => EqualityComparer<U>.Default.Equals(item.key, key));
        }

        public void Clear()
        {
            _lookup?.Clear();
            items.Clear();
        }

        public bool ContainsKey(U key)
        {
            EnsureLookup();
            return _lookup.ContainsKey(key);
        }

        public int Count => items.Count;
        public IEnumerable<U> Keys => items.Select(item => item.key);
        public IEnumerable<T> Values => items.Select(item => item.item);
    }
}