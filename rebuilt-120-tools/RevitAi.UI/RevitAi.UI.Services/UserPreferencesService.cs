using System;
using System.IO;
using System.Text.Json;
using RevitAi.UI.Models;

namespace RevitAi.UI.Services;

public sealed class UserPreferencesService : IUserPreferencesService
{
	private readonly string _preferencesFilePath;

	private UserPreferences _currentPreferences;

	public UserPreferences CurrentPreferences => _currentPreferences;

	public UserPreferencesService()
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		_preferencesFilePath = Path.Combine(text, "user_preferences.json");
		_currentPreferences = LoadPreferences();
	}

	public UserPreferences LoadPreferences()
	{
		try
		{
			if (File.Exists(_preferencesFilePath))
			{
				UserPreferences userPreferences = JsonSerializer.Deserialize<UserPreferences>(File.ReadAllText(_preferencesFilePath));
				if (userPreferences != null)
				{
					return userPreferences;
				}
			}
		}
		catch (Exception)
		{
		}
		return new UserPreferences();
	}

	public void SavePreferences(UserPreferences preferences)
	{
		try
		{
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				WriteIndented = true
			};
			string contents = JsonSerializer.Serialize(preferences, options);
			File.WriteAllText(_preferencesFilePath, contents);
			_currentPreferences = preferences;
		}
		catch (Exception)
		{
		}
	}
}
