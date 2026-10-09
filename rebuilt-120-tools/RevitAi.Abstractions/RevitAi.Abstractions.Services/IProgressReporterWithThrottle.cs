namespace RevitAi.Abstractions.Services;

public interface IProgressReporterWithThrottle : IProgressReporter
{
	void ReportThrottled(int current, int total, string message);
}
