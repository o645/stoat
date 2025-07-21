using System.Net.Sockets;
using System.Text;
using static webServer.Helper;

namespace webServer;

/// <summary>
/// A single connection between server and client.
/// </summary>
public class Connection
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
				SendMessage(ParseRequest(request).ToBytes());
			}
			else
			{
				socket.Close();
			}
		}
        
	}
    
	/// <summary>
	/// Parse a single request sent to the connection, and responds.
	/// </summary>
	/// <param name="request">Request as a string</param>
	public Response ParseRequest(string request)
	{
		string[] requestParts = request.Split("\n");
		//type
		string[] typeParts = requestParts[0].Split(' ');
		switch (typeParts[0])
		{
			case "GET":
			case "HEAD":
				return GetRequest(requestParts);
			default:
				return new Response(statusCode.NOT_IMPLEMENTED, null, null);
		}
	}

	public Response GetRequest(string[] request)
	{
		Console.WriteLine("GET/HEAD request!");
		string file = request[0].Split(" ")[1];
		if (file[^1] == '/')
		{
			Console.WriteLine("Directory requested, redirecting to index.html");
			file += "index.html"; //default directories to their index page.
		}
		
		string path = $"{FilePath}{file}";
		string extension = Path.GetExtension(path);
		
		if (!File.Exists(path))
		{
			Console.WriteLine($"File not found. Path was {path}");
			return new Response(statusCode.NOT_FOUND, null, null);
		}
	
		byte[] content;
		string contentType;
		switch (extension)
		{
			case ".html":
				contentType = "text/html";
				break;
			case ".css":
				contentType = "text/css";
				break;
			case ".png":
			case ".webp":
			case ".jpeg":
			case ".jpg":
			case ".bmp":
			case ".gif":
				contentType = $"image/{extension.TrimStart('.')}";
				break;
			case ".js":
				contentType = "application/javascript";
				break;
			case ".ico":
				contentType = "image/x-icon";
				break;
			default:
				contentType = "text/plain"; //fallback to just text.
				break;
		}
		Console.WriteLine($"Sending {contentType}");
		content = File.ReadAllBytes(path);
		Console.WriteLine($"File found! Size is {content.Length} bytes");
		return new Response(statusCode.OK, contentType, content);
	}
    
	public void SendMessage(byte[] message)
	{
		Console.WriteLine($"{messagePrefix}Sending {Encoding.ASCII.GetString(message).Split("\n\n")[0]}");
		socket.Send(message);
		socket.Close();
	}
}