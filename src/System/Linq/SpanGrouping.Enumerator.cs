namespace System.Linq;

public partial struct SpanGrouping<TSource, TKey>
{
	/// <summary>
	/// Represents an enumerator type that can iterate on each <typeparamref name="TSource"/> elements.
	/// </summary>
	/// <param name="values">The values.</param>
	public ref struct Enumerator(ReadOnlySpan<TSource> values) : IEnumerator<TSource>
	{
		/// <summary>
		/// Indicates the backing values.
		/// </summary>
		private readonly ReadOnlySpan<TSource> _values = values;

		/// <summary>
		/// Indicates the current pointer position.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly ref readonly TSource Current => ref _values[_index];

		/// <inheritdoc/>
		readonly object? IEnumerator.Current => Current;

		/// <inheritdoc/>
		readonly TSource IEnumerator<TSource>.Current => Current;


		/// <inheritdoc/>
		public bool MoveNext() => ++_index < _values.Length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
