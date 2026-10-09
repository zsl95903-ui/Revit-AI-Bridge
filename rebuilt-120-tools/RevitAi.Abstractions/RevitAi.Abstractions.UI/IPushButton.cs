namespace RevitAi.Abstractions.UI;

public interface IPushButton
{
	string Text { get; set; }

	string? ToolTip { get; set; }

	string? LongDescription { get; set; }

	string? IconPath { get; set; }

	bool Enabled { get; set; }

	string? AvailabilityClassName { get; set; }

	bool Visible { get; set; }
}
