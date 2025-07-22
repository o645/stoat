using System.Text;
using static webServer.Helper;

namespace webServer;


public class Response
{
	/// <summary>
	/// Status Code.
	/// </summary>
	private statusCode code;

	/// <summary>
	/// MIME Content type.
	/// </summary>
	public string? contentType;
	private byte[]? content;

	public Response(statusCode Code, string? ContentType, byte[]? Content)
	{
		code = Code;
		contentType = ContentType;
		content = Content;
	}
	
	public Response(statusCode Code) => new Response(Code, null, null);

	public byte[] ToBytes()
	{
		string header = $"HTTP/1.1 {(int)code} {code}\n" +
		                $"Server: Stoat\n" +
		                $"Date: {DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss GMT")}\n";
		if (content != null)
		{
			header += $"Content-Type: {contentType}\n" +
			          $"Content-Length: {content.Length}\n";
		}
		header += "\n";
		return content != null ? Encoding.ASCII.GetBytes(header).Concat(content).ToArray() : Encoding.ASCII.GetBytes(header);
	}
}