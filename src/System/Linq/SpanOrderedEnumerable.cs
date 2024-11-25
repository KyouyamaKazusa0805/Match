namespace System.Linq;

/// <summary>
/// Represents an enumerable instance that is based on a <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <typeparam name="T">Indicates the type of each element.</typeparam>
/// <param name="values">Indicates the values.</param>
/// <param name="selectors">
/// <para>Indicates the selector functions that return <typeparamref name="T"/> instances, to be used as comparison.</para>
/// <include file="../../global-doc-comments.xml" path="//g/csharp11/feature[@name='scoped-keyword']"/>
/// <include file="../../global-doc-comments.xml" path="//g/csharp12/feature[@name='params-collections']/target[@name='parameter']"/>
/// </param>
[DebuggerStepThrough]
public readonly ref partial struct SpanOrderedEnumerable<T>(
	ReadOnlySpan<T> values,
	[UnscopedRef] params ReadOnlySpan<Func<T, T, int>> selectors
) :
	IEnumerable<T>,
	IReadOnlyCollection<T>
{
	/// <summary>
	/// Indicates the backing values.
	/// </summary>
	private readonly ReadOnlySpan<T> _values = values;

	/// <summary>
	/// Indicates the backing selectors.
	/// </summary>
	private readonly ReadOnlySpan<Func<T, T, int>> _selectors = selectors;


	/// <summary>
	/// Indicates the number of elements stored in the collection.
	/// </summary>
	public int Length => _values.Length;

	/// <inheritdoc/>
	int IReadOnlyCollection<T>.Count => Length;

	/// <summary>
	/// Creates an ordered <see cref="Span{T}"/> instance.
	/// </summary>
	/// <returns>An ordered <see cref="Span{T}"/> instance, whose value is from the current enumerable instance.</returns>
	private Span<T> Span
	{
		get
		{
			// Copy field in order to make the variable can be used inside lambda.
			var selectors = _selectors.ToArray();

			// Sort the span of values.
			var result = new T[_values.Length].AsSpan();
			_values.CopyTo(result);
			result.Sort(
				(l, r) =>
				{
					foreach (var selector in selectors)
					{
						if (selector(l, r) is var tempResult and not 0)
						{
							return tempResult;
						}
					}
					return 0;
				}
			);
			return result;
		}
	}


	/// <inheritdoc cref="ReadOnlySpan{T}.this[int]"/>
	public ref readonly T this[int index] => ref _values[index];


	/// <summary>
	/// Projects each element into a new transform.
	/// </summary>
	/// <typeparam name="TResult">The type of the result values.</typeparam>
	/// <param name="selector">The selector to be used by transforming the <typeparamref name="T"/> instances.</param>
	/// <returns>A span of <typeparamref name="TResult"/> values.</returns>
	public ReadOnlySpan<TResult> Select<TResult>(Func<T, TResult> selector)
	{
		var result = new List<TResult>(_values.Length);
		foreach (var element in Span)
		{
			result.AddRef(selector(element));
		}
		return result.AsSpan();
	}

	/// <summary>
	/// Filters the collection using the specified condition.
	/// </summary>
	/// <param name="condition">The condition to be used.</param>
	/// <returns>A span of <typeparamref name="T"/> instances.</returns>
	public ReadOnlySpan<T> Where(Func<T, bool> condition)
	{
		var result = new List<T>(_values.Length);
		foreach (var element in Span)
		{
			if (condition(element))
			{
				result.AddRef(in element);
			}
		}
		return result.AsSpan();
	}

	/// <summary>
	/// <para>Same as <see cref="ThenBy{TKey}(Func{T, TKey})"/>.</para>
	/// <para>
	/// In query expression level, this method will be a little different with standard LINQ design -
	/// two adjacent <see langword="orderby"/> clauses will be translated into two methods invocation:
	/// methods <see cref="OrderBy{TKey}(Func{T, TKey})"/> and <see cref="OrderByDescending{TKey}(Func{T, TKey})"/>.
	/// However, due to consideration of optimization on syntax, the second <see langword="orderby"/> clause
	/// will be treated as <see cref="ThenBy{TKey}(Func{T, TKey})"/> or <see cref="ThenByDescending{TKey}(Func{T, TKey})"/>
	/// invocation instead.
	/// </para>
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <param name="selector">The selector.</param>
	/// <returns>A <see cref="SpanOrderedEnumerable{T}"/> instance, with a new selector added in the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SpanOrderedEnumerable<T> OrderBy<TKey>(Func<T, TKey> selector) => ThenBy(selector);

	/// <summary>
	/// <para>Same as <see cref="ThenByDescending{TKey}(Func{T, TKey})"/>.</para>
	/// <para><inheritdoc cref="OrderBy{TKey}(Func{T, TKey})" path="/summary/para[2]"/></para>
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <param name="selector">The selector.</param>
	/// <returns>A <see cref="SpanOrderedEnumerable{T}"/> instance, with a new selector added in the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SpanOrderedEnumerable<T> OrderByDescending<TKey>(Func<T, TKey> selector) => ThenByDescending(selector);

	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SpanOrderedEnumerable<T> ThenBy<TKey>(Func<T, TKey> selector)
		=> new(
			_values,
			(Func<T, T, int>[])[
				.. _selectors,
				(l, r) => (selector(l), selector(r)) switch
				{
					(IComparable<TKey> left, var right) => left.CompareTo(right),
					var (a, b) => Comparer<TKey>.Default.Compare(a, b)
				}
			]
		);

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SpanOrderedEnumerable<T> ThenByDescending<TKey>(Func<T, TKey> selector)
		=> new(
			_values,
			(Func<T, T, int>[])[
				.. _selectors,
				(l, r) => (selector(l), selector(r)) switch
				{
					(IComparable<TKey> left, var right) => -left.CompareTo(right),
					var (a, b) => -Comparer<TKey>.Default.Compare(a, b)
				}
			]
		);

	/// <inheritdoc cref="SpanEnumerable.GroupBy{TSource, TKey}(ReadOnlySpan{TSource}, Func{TSource, TKey})"/>
	public ReadOnlySpan<SpanGrouping<T, TKey>> GroupBy<TKey>(Func<T, TKey> keySelector) where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<T>>(_values.Length >> 2);
		foreach (var element in Span)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].AddRef(in element);
			}
		}

		var result = new List<SpanGrouping<T, TKey>>(tempDictionary.Count);
		foreach (var key in tempDictionary.Keys)
		{
			var tempValues = tempDictionary[key];
			result.AddRef(new([.. tempValues], key));
		}
		return result.AsSpan();
	}

	/// <inheritdoc cref="SpanEnumerable.GroupBy{TSource, TKey, TElement}(ReadOnlySpan{TSource}, Func{TSource, TKey}, Func{TSource, TElement})"/>
	public ReadOnlySpan<SpanGrouping<TElement, TKey>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
		where TKey : notnull
	{
		var tempDictionary = new Dictionary<TKey, List<T>>(_values.Length >> 2);
		foreach (var element in Span)
		{
			var key = keySelector(element);
			if (!tempDictionary.TryAdd(key, [element]))
			{
				tempDictionary[key].AddRef(in element);
			}
		}

		var result = new List<SpanGrouping<TElement, TKey>>(tempDictionary.Count);
		foreach (var key in tempDictionary.Keys)
		{
			var tempValues = tempDictionary[key];
			var valuesConverted = from value in tempValues select elementSelector(value);
			result.AddRef(new(valuesConverted.ToArray(), key));
		}
		return result.AsSpan();
	}

	/// <inheritdoc cref="ISliceMethod{TSelf, TSource}.Slice(int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<T> Slice(int start, int length) => Span.Slice(start, length);

	/// <inheritdoc cref="ReadOnlySpan{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<T>.Enumerator GetEnumerator() => Span.GetEnumerator();

	/// <inheritdoc cref="IToArrayMethod{TSelf, TSource}.ToArray"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T[] ToArray() => Span.ToArray();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => Span.ToArray().GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<T> IEnumerable<T>.GetEnumerator() => Span.ToArray().AsEnumerable().GetEnumerator();


	/// <summary>
	/// Creates an <see cref="SpanOrderedEnumerable{T}"/> instance via the specified values.
	/// </summary>
	/// <typeparam name="TKey">The type of the key to be compared.</typeparam>
	/// <param name="values">The values to be used.</param>
	/// <param name="keySelector">
	/// The selector method that calculates a <typeparamref name="TKey"/> from each <typeparamref name="T"/> instance.
	/// </param>
	/// <param name="comparer">
	/// A comparable instance that temporarily checks the comparing result of two <typeparamref name="TKey"/> values.
	/// </param>
	/// <param name="descending">A <see cref="bool"/> value indicating whether the creation is for descending comparison rule.</param>
	/// <returns>An <see cref="SpanOrderedEnumerable{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SpanOrderedEnumerable<T> Create<TKey>(ReadOnlySpan<T> values, Func<T, TKey> keySelector, IComparer<TKey>? comparer, bool descending)
	{
		comparer ??= Comparer<TKey>.Default;
		return new(values, (Func<T, T, int>[])[descending ? descendingComparer : ascendingComparer]);


		int ascendingComparer(T left, T right) => comparer.Compare(keySelector(left), keySelector(right));

		int descendingComparer(T left, T right) => -comparer.Compare(keySelector(left), keySelector(right));
	}
}
