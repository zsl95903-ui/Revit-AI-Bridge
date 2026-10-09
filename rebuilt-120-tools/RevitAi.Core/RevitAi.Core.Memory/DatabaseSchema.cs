using System;
using System.Data.Common;
using System.IO;
using RevitAi.Abstractions.Logging;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using ns7;

namespace RevitAi.Core.Memory;

public static class DatabaseSchema
{
	public static string GetDatabasePath()
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RevitAi", "Memory");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return Path.Combine(text, "memory.db");
	}

	public static void Initialize()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		try
		{
			Batteries.Init();
		}
		catch (Exception ex)
		{
			Logger.Error("[DatabaseSchema] 初始化失败: " + ex.Message, ex);
			throw;
		}
		string databasePath = GetDatabasePath();
		SqliteConnection val = new SqliteConnection(((object)new SqliteConnectionStringBuilder
		{
			DataSource = databasePath,
			Mode = (SqliteOpenMode)0
		}).ToString());
		try
		{
			((DbConnection)(object)val).Open();
			SqliteCommand val2 = new SqliteCommand("\n                CREATE TABLE IF NOT EXISTS sessions (\n                    id INTEGER PRIMARY KEY AUTOINCREMENT,\n                    session_id TEXT UNIQUE NOT NULL,\n                    title TEXT,\n                    start_time DATETIME NOT NULL,\n                    end_time DATETIME,\n                    summary TEXT\n                );", val);
			try
			{
				((DbCommand)(object)val2).ExecuteNonQuery();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			SqliteCommand val3 = new SqliteCommand("\n                CREATE TABLE IF NOT EXISTS observations (\n                    id INTEGER PRIMARY KEY AUTOINCREMENT,\n                    session_id TEXT NOT NULL,\n                    type TEXT NOT NULL,\n                    content TEXT NOT NULL,\n                    timestamp DATETIME NOT NULL,\n                    related_files TEXT,\n                    related_components TEXT,\n                    importance INTEGER DEFAULT 5,\n                    FOREIGN KEY (session_id) REFERENCES sessions(session_id)\n                );", val);
			try
			{
				((DbCommand)(object)val3).ExecuteNonQuery();
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			string[] array = new string[4]
			{
				"CREATE INDEX IF NOT EXISTS idx_observations_session_id ON observations(session_id);",
				"CREATE INDEX IF NOT EXISTS idx_observations_type ON observations(type);",
				"CREATE INDEX IF NOT EXISTS idx_observations_timestamp ON observations(timestamp DESC);",
				"CREATE INDEX IF NOT EXISTS idx_sessions_start_time ON sessions(start_time DESC);"
			};
			for (int i = 0; i < array.Length; i++)
			{
				SqliteCommand val4 = new SqliteCommand(array[i], val);
				try
				{
					((DbCommand)(object)val4).ExecuteNonQuery();
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
			Logger.Info("[DatabaseSchema] 数据库初始化完成: " + databasePath);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
