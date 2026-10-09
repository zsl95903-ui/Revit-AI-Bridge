namespace RevitAi.Abstractions.Services;

public interface IProgressReporter
{
	bool IsCancellationRequested { get; }

	void Report(int current, int total, string message);

	void ReportCategory(string category, int current, int total);
}
