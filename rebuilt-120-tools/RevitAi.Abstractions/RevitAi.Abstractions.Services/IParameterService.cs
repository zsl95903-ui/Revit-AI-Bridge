using System.Collections.Generic;

namespace RevitAi.Abstractions.Services;

public interface IParameterService
{
	IEnumerable<object> GetParameters(object element);

	object? GetParameter(object element, string parameterName);

	string? GetParameterValueAsString(object parameter);

	double? GetParameterValueAsDouble(object parameter);

	int? GetParameterValueAsInteger(object parameter);

	string? GetElementParameterValue(object element, string parameterName);

	bool SetParameterValue(object element, string parameterName, string value);

	bool SetParameterValue(object element, string parameterName, double value);

	bool SetParameterValueWithUnit(object element, string parameterName, double value, string unit);

	bool SetParameterValue(object element, string parameterName, int value);

	bool SetParameterValueById(object element, string parameterName, int elementId);

	string? GetParameterName(object parameter);

	string? GetParameterType(object parameter);

	string? GetParameterDefinitionType(object parameter);

	bool IsParameterReadOnly(object parameter);
}
