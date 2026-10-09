using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitAi.Revit.Services;

public class StairsEditScopeFailuresPreprocessor : IFailuresPreprocessor
{
	public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
	{
		IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
		foreach (FailureMessageAccessor item in failureMessages)
		{
			_ = item;
		}
		return (FailureProcessingResult)0;
	}
}
