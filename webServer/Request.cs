using static webServer.Helper;

namespace webServer;

public class Request
{
	public RequestType type;
	public string requestedContent;
	public List<string> headers;
	public string body;

	public Request(string rawRequest)
	{
		string[] requestParts = rawRequest.Split("\n");
		string[] typeParts = requestParts[0].Split(' ');
		type = (RequestType)Enum.Parse(typeof(RequestType), typeParts[0]);
		requestedContent = typeParts[1];
		headers = new List<string>();
		for (int i = 1; i < requestParts.Length; i++)
		{
			headers.Add(requestParts[i]);
		}
		body = string.Join("\n", requestParts.Skip(headers.Count + 1));
	}
}

