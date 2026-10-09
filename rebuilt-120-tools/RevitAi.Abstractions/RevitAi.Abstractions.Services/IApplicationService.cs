namespace RevitAi.Abstractions.Services;

public interface IApplicationService
{
	(byte red, byte green, byte blue) GetBackgroundColor();

	void SetBackgroundColor(byte red, byte green, byte blue);

	(byte red, byte green, byte blue) ToggleBackgroundColor();
}
