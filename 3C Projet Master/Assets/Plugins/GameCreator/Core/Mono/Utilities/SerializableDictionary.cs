namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[System.Serializable]
	public class SerializableDictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>,  UnityEngine.ISerializationCallbackReceiver
	{
		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private Dictionary<TKey, TValue> dictionary;

		[SerializeField] private TKey[] keys;
		[SerializeField] private TValue[] values;

		// PUBLIC METHODS: ---------------------------------------------------------------------------------------------

		public int Count
		{
			get 
			{ 
				return (this.dictionary != null ? this.dictionary.Count : 0);
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (this.dictionary == null) this.dictionary = new Dictionary<TKey, TValue>();
			this.dictionary.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			if (this.dictionary == null) return false;
			return this.dictionary.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get
			{
				if (this.dictionary == null) this.dictionary = new Dictionary<TKey, TValue>();
				return this.dictionary.Keys;
			}
		}

		public bool Remove(TKey key)
		{
			if (this.dictionary == null) return false;
			return this.dictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if(this.dictionary == null)
			{
				value = default(TValue);
				return false;
			}

			return this.dictionary.TryGetValue(key, out value);
		}

		public ICollection<TValue> Values
		{
			get
			{
				if (this.dictionary == null) this.dictionary = new Dictionary<TKey, TValue>();
				return this.dictionary.Values;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (this.dictionary == null) throw new KeyNotFoundException();
				return this.dictionary[key];
			}
			set
			{
				if (this.dictionary == null) this.dictionary = new Dictionary<TKey, TValue>();
				this.dictionary[key] = value;
			}
		}

		public void Clear()
		{
			if (this.dictionary != null) this.dictionary.Clear();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			if (this.dictionary == null) this.dictionary = new Dictionary<TKey, TValue>();
			(this.dictionary as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			if (this.dictionary == null) return false;
			return (this.dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (this.dictionary == null) return;
			(this.dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			if (this.dictionary == null) return false;
			return (this.dictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return false; }
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			if (this.dictionary == null) return default(Dictionary<TKey, TValue>.Enumerator);
			return this.dictionary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			if (this.dictionary == null) return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
			return this.dictionary.GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			if (this.dictionary == null) return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
			return this.dictionary.GetEnumerator();
		}

		// SERIALIZATION CALLBACKS: ------------------------------------------------------------------------------------

		void UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if(this.keys != null && this.values != null)
			{
				if (this.dictionary == null) this.dictionary = new Dictionary<TKey, TValue>(this.keys.Length);
				else this.dictionary.Clear();
				for(int i = 0; i < this.keys.Length; i++)
				{
					if (i < this.values.Length) this.dictionary[this.keys[i]] = this.values[i];
					else this.dictionary[this.keys[i]] = default(TValue);
				}
			}

			this.keys = null;
			this.values = null;
		}

		void UnityEngine.ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if(this.dictionary == null || this.dictionary.Count == 0)
			{
				this.keys = null;
				this.values = null;
			}
			else
			{
				int cnt = this.dictionary.Count;
				this.keys = new TKey[cnt];
				this.values = new TValue[cnt];
				int i = 0;
				var e = this.dictionary.GetEnumerator();
				while (e.MoveNext())
				{
					this.keys[i] = e.Current.Key;
					this.values[i] = e.Current.Value;
					i++;
				}
			}
		}
	}
}