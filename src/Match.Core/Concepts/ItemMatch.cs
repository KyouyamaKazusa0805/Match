namespace Match.Concepts;

/// <summary>
/// Represents a match of item pair.
/// </summary>
/// <param name="Start">Indicates the start.</param>
/// <param name="End">Indicates the end.</param>
/// <param name="Interims">Indicates the interim coordinates.</param>
public sealed record ItemMatch(Coordinate Start, Coordinate End, params Coordinate[] Interims) : IEqualityOperators<ItemMatch, ItemMatch, bool>
{
	/// <summary>
	/// Indicates the number of turning.
	/// </summary>
	public int TurningCount => Interims.Length;


	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append($"{nameof(Start)} = ");
		builder.Append(Start);
		builder.Append($", {nameof(End)} = ");
		builder.Append(End);
		builder.Append($", {nameof(Interims)} = [");
		for (var i = 0; i < Interims.Length; i++)
		{
			builder.Append(Interims[i].ToString());
			if (i != Interims.Length - 1)
			{
				builder.Append(", ");
			}
		}
		builder.Append(']');
		return true;
	}
}
