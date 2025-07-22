using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static webServer.Helper;

namespace webServer;

/// <summary>
/// A single connection between server and client.
/// </summary>
public class Connection : IDisposable
{
	/// <summary>
	/// Connection socket.
	/// </summary>
	private Socket socket;
	/// <summary>
	/// Connection ID.
	/// </summary>
	private int id;
	/// <summary>
	/// Prefix for logging.
	/// </summary>
	private string messagePrefix => $"[{id}] ";

	private string FilePath;

	/// <summary>
	/// Create a new connection.
	/// </summary>
	/// <param name="socket">Connection Socket</param>
	/// <param name="id">Connection ID</param>
	public Connection(Socket socket, int id, string FilePath)
	{
		Console.WriteLine($"Connection {id} created");
		this.socket = socket;
		this.id = id;
		this.FilePath = FilePath;
	}

	/// <summary>
	/// Handles the communication loop for a connection.
	/// </summary>
	public void Loop()
	{
		while (socket.Connected)
		{
			var buffer = new byte[1024];
			var received = socket.Receive(buffer);
			if (received > 0)
			{
				string request = Encoding.ASCII.GetString(buffer);
				Console.WriteLine($"{messagePrefix}Received Message: {request}");
				Request parsedRequest = new Request(request);
				SendMessage(HandleRequest(parsedRequest).ToBytes());
				
			}
			else
			{
				break;
			}
		}
        Dispose();
	}
    
	/// <summary>
	/// Parse a single request sent to the connection, and responds.
	/// </summary>
	/// <param name="request">Request as a string</param>
	public Response HandleRequest(Request request)
	{
		switch (request.type)
		{
			case RequestType.GET:
				case RequestType.HEAD:
				return GetResponse(request);
			default:
				return new Response(statusCode.NOT_IMPLEMENTED);
		}
	}

	public Response GetResponse(Request request)
	{
		Console.WriteLine("GET/HEAD request!");
		string file = request.requestedContent;
		if (file[^1] == '/')
		{
			Console.WriteLine("Directory requested, redirecting to index.html");
			file += "index.html"; //default directories to their index page.
		}
		string path = $"{FilePath}{file}";
		if (!File.Exists(path))
		{
			Console.WriteLine($"File not found. Path was {path}");
			return new Response(statusCode.NOT_FOUND, null, null);
		}
		int auth = Authentication.CheckIfFileRequiresAuth(path, FilePath);
		switch (auth)
		{
			case 0:
				//not authorized
				return new Response(statusCode.UNAUTHORIZED, null, null);
			case -1:
				//forbidden
				return new Response(statusCode.FORBIDDEN, null, null);
				break;
			case 1:
				//allowed, so just continue.
				break;
		}

		var content = File.ReadAllBytes(path);
		Console.WriteLine($"File found! Size is {content.Length} bytes");
		var contentType = GetContentTypeFromExtension(Path.GetExtension(path));
		Console.WriteLine($"Content type is {contentType}");
		return new Response(statusCode.OK, contentType, content);
	}
	

	public void SendMessage(byte[] message)
	{
		Console.WriteLine($"{messagePrefix}Sending {Encoding.ASCII.GetString(message).Split("\n\n")[0]}");
		socket.Send(message);
		socket.Close();
	}

	public void Dispose()
	{
		socket.Close();
		socket.Dispose();
	}
}