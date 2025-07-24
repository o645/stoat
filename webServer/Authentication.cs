namespace webServer;

public static class Authentication
{
	public static int CheckIfFileRequiresAuth(string path, string FilePath)
	{
		if (!CheckIfFileIsInWWW(path, FilePath))
		{
			return -1;
		}
		//todo: auth system.
		return 1;
	}

	private static bool CheckIfFileIsInWWW(string requestedPath, string siteRootPath)
	{
		string req = Path.GetFullPath(requestedPath);
		string site = Path.GetFullPath(siteRootPath);
		return req.StartsWith(site);
	}
}