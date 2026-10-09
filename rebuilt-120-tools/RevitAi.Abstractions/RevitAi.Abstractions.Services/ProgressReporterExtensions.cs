namespace RevitAi.Abstractions.Services;

public static class ProgressReporterExtensions
{
	public static void ReportThrottled(this IProgressReporter reporter, int current, int total, string message)
	{
		if (reporter is IProgressReporterWithThrottle progressReporterWithThrottle)
		{
			progressReporterWithThrottle.ReportThrottled(current, total, message);
		}
		else
		{
			reporter.Report(current, total, message);
		}
	}
}
