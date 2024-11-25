namespace Match.Analytics;

/// <summary>
/// Represents a type that stores the result of a analysis operation.
/// </summary>
/// <param name="grid">The grid to be checked.</param>
public sealed class AnalysisResult(Grid grid) :
	IEquatable<AnalysisResult>,
	IEqualityOperators<AnalysisResult, AnalysisResult, bool>
{
	/// <summary>
	/// Indicates whether the puzzle is fully solved.
	/// </summary>
	[MemberNotNullWhen(true, nameof(InterimMatches))]
	public required bool IsSolved { get; init; }

	/// <summary>
	/// Indicates the failed reason.
	/// </summary>
	public FailedReason FailedReason { get; init; }

	/// <summary>
	/// Indicates the matches found during the analysis.
	/// </summary>
	public ReadOnlySpan<ItemMatch> Matches => InterimMatches;

	/// <summary>
	/// Indicates the elapsed time.
	/// </summary>
	public TimeSpan ElapsedTime { get; init; }

	/// <summary>
	/// Indicates the base grid.
	/// </summary>
	public Grid Grid { get; } = grid;

	/// <summary>
	/// Indicates the exception encountered.
	/// </summary>
	public Exception? UnhandledException { get; init; }

	/// <summary>
	/// Indicates the matches.
	/// </summary>
	internal ItemMatch[]? InterimMatches { get; init; }


	/// <inheritdoc/>
	public override bool Equals(object? obj) => Equals(obj as AnalysisResult);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] AnalysisResult? other)
		=> other is not null && IsSolved == other.IsSolved && Grid == other.Grid;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Grid, IsSolved);

	/// <inheritdoc/>
	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine("Puzzle:");
		sb.AppendLine(Grid.ToString());
		sb.AppendLine("---");

		if (IsSolved)
		{
			sb.AppendLine("Steps:");
			foreach (var step in InterimMatches)
			{
				sb.AppendLine(step.ToFullString());
			}
			sb.AppendLine("---");
			sb.AppendLine("Puzzle is solved.");
			sb.AppendLine($@"Elapsed time: {ElapsedTime:hh\:mm\:ss\.fff}");
		}
		else
		{
			sb.AppendLine($"Puzzle isn't solved. Reason code: '{FailedReason}'.");
			if (UnhandledException is not null)
			{
				sb.AppendLine($"Unhandled exception: {UnhandledException}");
			}
		}
		return sb.ToString();
	}


	/// <inheritdoc/>
	public static bool operator ==(AnalysisResult? left, AnalysisResult? right)
		=> (left, right) switch { (not null, not null) => left.Equals(right), (null, null) => true, _ => false };

	/// <inheritdoc/>
	public static bool operator !=(AnalysisResult? left, AnalysisResult? right) => !(left == right);
}
