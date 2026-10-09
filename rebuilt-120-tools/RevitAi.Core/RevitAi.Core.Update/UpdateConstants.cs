namespace RevitAi.Core.Update;

public static class UpdateConstants
{
	public const string UPDATE_DIR = "RevitAi\\Updates";

	public const string PENDING_UPDATE_FILE = "RevitAi\\Updates\\pending_update.json";

	public const string PACKAGE_FILE_NAME_FORMAT = "RevitAi_v{0}.zip";

	public const int PRESIGNED_URL_EXPIRES = 3600;

	public const int REQUEST_TIMEOUT_SECONDS = 30;
}
