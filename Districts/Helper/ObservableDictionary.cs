using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Districts.Helper
{
    /// <summary>
    ///     Обозреваемая коллекция на основе Dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        private readonly Dictionary<TKey, TValue> _innerDictionary = new Dictionary<TKey, TValue>();

        public ObservableDictionary()
        {
        }

        public ObservableDictionary(IDictionary<TKey, TValue> source)
        {
            if (source == null || !source.Any())
                return;

            _innerDictionary = new Dictionary<TKey, TValue>(source);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _innerDictionary.Clear();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _innerDictionary.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex,
                    "Индекс меньше нуля или выходит за пределы массива");

            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Массив имеет недостаточную длину", nameof(arrayIndex));

            var count = Count;

            var entries = _innerDictionary.ToArray();

            for (var i = 0; i < count; ++i)
                if (entries[i].GetHashCode() >= 0)
                    array[i++] = new KeyValuePair<TKey, TValue>(entries[i].Key, entries[i].Value);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public int Count => _innerDictionary.Count;
        public bool IsReadOnly => false;

        public bool ContainsKey(TKey key)
        {
            return _innerDictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _innerDictionary.Add(key, value);

            CollectionChanged?.Invoke(value, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
        }

        public bool Remove(TKey key)
        {
            var result = _innerDictionary.Remove(key);
            if (result)
                CollectionChanged?.Invoke(key,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));

            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _innerDictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => _innerDictionary[key];
            set
            {
                var contains = _innerDictionary.ContainsKey(key);
                _innerDictionary[key] = value;
                var args = new NotifyCollectionChangedEventArgs(
                    contains
                        ? NotifyCollectionChangedAction.Replace
                        : NotifyCollectionChangedAction.Add);
                CollectionChanged?.Invoke(value, args);
            }
        }

        public ICollection<TKey> Keys => _innerDictionary.Keys;
        public ICollection<TValue> Values => _innerDictionary.Values;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}