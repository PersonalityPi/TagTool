using TagTool.Cache;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using TagTool.Tags.Definitions;
using System.Linq;

namespace TagTool.Tags
{
	[TagStructure(Size = 0x8, MaxVersion = CacheVersion.Halo2Vista)]
	[TagStructure(Size = 0xC, MinVersion = CacheVersion.Halo3Retail)]
	public class TagBlock : TagStructure, IList<TagStructure>
	{
		/// <summary>
		/// The count of the referenced block.
		/// </summary>
		public int Count;

		/// <summary>
		/// The address of the referenced block.
		/// </summary>
		public CacheAddress Address;

		[TagField(Size = 0x4, MinVersion = CacheVersion.Halo3Retail)]
		public uint UnusedPointer;

		/// <summary>
		/// The list of elements within the tag block.
		/// </summary>
		[TagField(Runtime = true)]
		protected IList<TagStructure> Elements = new List<TagStructure> { };

		public Type ElementType { get; protected set; }

		public TagBlock() : this(0, new CacheAddress()) { }
		public TagBlock(int count, CacheAddress address)
		{
			Count = count;
			Address = address;
			Elements = new List<TagStructure>(count);
		}

		#region IList<TagStructure> Implementation
		int ICollection<TagStructure>.Count => Elements.Count;
		public bool IsReadOnly => Elements.IsReadOnly;
		public TagStructure this[int index] { get => Elements[index]; set => Elements[index] = value; }
		public void Add(TagStructure item)
		{
			Elements.Add(item);
			Count = Elements.Count;
		}

		public void Clear() => Elements.Clear();
		public bool Contains(TagStructure item) => Elements.Contains(item);
		public void CopyTo(TagStructure[] array, int arrayIndex) => Elements.CopyTo(array, arrayIndex);
		public IEnumerator<TagStructure> GetEnumerator() => Elements.GetEnumerator();
		public int IndexOf(TagStructure item) => Elements.IndexOf(item);
		public void Insert(int index, TagStructure item)
		{
			Elements.Insert(index, item);
			Count = Elements.Count;
		}

		public bool Remove(TagStructure item)
		{
			var result = Elements.Remove(item);
			Count = Elements.Count;
			return result;
		}

		public void RemoveAt(int index)
		{
			Elements.RemoveAt(index);
			Count = Elements.Count;
		}
		IEnumerator IEnumerable.GetEnumerator() => Elements.GetEnumerator();
		#endregion
	}

	[TagStructure(Size = 0x0)]
	public class TagBlock<T> : TagBlock, IList<T> where T : TagStructure
	{
		public TagBlock() : this(0, new CacheAddress()) { }
		public TagBlock(int count) : this(count, new CacheAddress()) { }
		public TagBlock(int count, CacheAddress address) : base(count, address)
		{
			if (typeof(T) == typeof(TagBlock))
				throw new NotSupportedException($"Type parameter must not be `{nameof(TagBlock)}`: `{nameof(TagBlock<T>)}`.");
			ElementType = typeof(T);
		}

		public TagBlock(IEnumerable<T> source) : base(source.Count(), new CacheAddress())
		{
			if (typeof(T) == typeof(TagBlock))
				throw new NotSupportedException($"Type parameter must not be `{nameof(TagBlock)}`: `{nameof(TagBlock<T>)}`.");

			Elements = source.OfType<TagStructure>().ToList();
			ElementType = typeof(T);
		}

		#region IList<T> Implementation
		public new int Count => Elements.Count;
		public new T this[int index] { get => (T)Elements[index]; set => Elements[index] = value; }

		public void AddRange(IEnumerable<T> items)
		{
			foreach (var item in items)
				base.Add(item);
		}

		public void Add(T item) => base.Add(item);

		public bool Contains(T value) => Elements.Contains(value);
		public void CopyTo(T[] array, int arrayIndex) => Elements.CopyTo(array, arrayIndex);
		public int IndexOf(T value) => Elements.IndexOf(value);
		public void Insert(int index, T value) => base.Insert(index, value);
		public bool Remove(T item) => base.Remove(item);
		public new IEnumerator<T> GetEnumerator() => Elements.OfType<T>().GetEnumerator();

		public void ForEach(Action<T> action) => Elements.OfType<T>().ToList().ForEach(action);

		public T FirstOrDefault(Func<T, bool> p) => Elements.OfType<T>().FirstOrDefault(p);

		public IEnumerable<TResult> Select<TResult>(Func<T, TResult> p) => Elements.OfType<T>().Select(p);

		public IOrderedEnumerable<T> OrderBy<TKey>(Func<T, TKey> p) => Elements.OfType<T>().OrderBy(p);

		public T Find(Func<T, bool> p) => Elements.OfType<T>().First(p);

		public T Last() => (T)Elements.Last();

		public IEnumerable<T> Where(Func<T, bool> p) => Elements.OfType<T>().Where(p);
		#endregion
	}
}
