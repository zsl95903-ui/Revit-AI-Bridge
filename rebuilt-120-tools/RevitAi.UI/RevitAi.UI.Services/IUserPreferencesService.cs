using RevitAi.UI.Models;

namespace RevitAi.UI.Services;

public interface IUserPreferencesService
{
	UserPreferences CurrentPreferences { get; }

	UserPreferences LoadPreferences();

	void SavePreferences(UserPreferences preferences);
}
