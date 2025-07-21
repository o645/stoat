using System.Net;
using System.Net.Sockets;
using System.Text;

namespace webServer;

class Program
{
    public static bool KILLSWITCH = false;
    public static string FilePath = "../debugSite";
    static void Main(string[] args)
    {
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80));
        server.Listen();
        Console.WriteLine("Hello, World!");
        int id = 1;
        while (!KILLSWITCH)
        {
            Connection client = new Connection(server.Accept(),id);
            Task.Run(client.Loop);
            id++;
        }
    }
}

public class Connection
{
    private Socket socket;
    private int id;
    private string messagePrefix => $"[{id}] ";

    public Connection(Socket socket, int id)
    {
        Console.WriteLine($"Connection {id} created");
        this.socket = socket;
        this.id = id;
    }

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
                ParseRequest(request);
            }
            else
            {
                socket.Close();
            }
        }
        
    }
    
    public void ParseRequest(string request)
    {
        string[] requestParts = request.Split("\n");
        //type
        string[] typeParts = requestParts[0].Split(' ');
        switch (typeParts[0])
        {
            case "GET":
            case "HEAD":
                GetRequest(requestParts);
                break;
            default:
                SendMessage(Encoding.ASCII.GetBytes("HTTP/1.1 501 Not Implemented\n"));
                break;
        }
    }

    public void GetRequest(string[] request)
    {
        string file = request[0].Split(" ")[1];
        if (file[^1] == '/')
        {
            file += "index.html"; //default directories to their index page.
        }
        string path = $"{Program.FilePath}{file}";
        if (!File.Exists(path))
        {
            SendMessage("HTTP/1.1 404 Not Found\n"u8.ToArray());
        }

        byte[] content;
        string extension = Path.GetExtension(path);
        string type = "";
        switch (extension)
        {
            case ".html":
                type = "text/html";
            break;
            case ".css":
                type = "text/css";
                break;
            case ".png":
                case ".webp":
                case ".jpeg":
                case ".jpg":
                case ".bmp":
                case ".gif":
                    type = $"image/{extension.TrimStart('.')}";
                    break;
            default:
                type = "text/plain";
                break;
        }
        content = File.ReadAllBytes(path);
        string headers = $"HTTP/1.1 200 OK\n" +
                         $"Content-Type: {type}\n" +
                         $"Content-Length: {content.Length}\n" +
                         $"Server: Stoat\n"+
                         "\n"; //break between headers and content.
        byte[] response = Encoding.ASCII.GetBytes(headers).Concat(content).ToArray();
        
        SendMessage(response);
    }
    
    public void SendMessage(byte[] message)
    {
        Console.WriteLine($"{messagePrefix}Sending {Encoding.ASCII.GetString(message).Split("\n\n")[0]}");
        socket.Send(message);
    }
}

