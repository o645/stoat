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
                string request = Encoding.UTF8.GetString(buffer);
                Console.WriteLine($"{messagePrefix}Received Message: {request}");
                byte[] response = Encoding.UTF8.GetBytes(ParseRequest(request));
                socket.Send(response);
                socket.Close();
            }
        }
        
    }
    
    public string ParseRequest(string request)
    {
        string[] requestParts = request.Split("\n");
        //type
        string[] typeParts = requestParts[0].Split(' ');
        switch (typeParts[0])
        {
            case "GET":
            case "HEAD":
                return GetRequest(requestParts);
                break;
            default:
                return "HTTP/1.1 501 Not Implemented\n";
                break;
        }
    }

    public string GetRequest(string[] request)
    {
        string file = request[0].Split(" ")[1];
        if (file[^1] == '/')
        {
            file += "index.html"; //default directories to their index page.
        }
        string path = $"{Program.FilePath}{file}";
        string[] lines = File.ReadAllLines(path);
        
        
        //future: check actual content type to allow txt etc.
        return $"HTTP/1.1 200 OK\nServer: Stoat\n\n{string.Join("\n", lines)}";
    }
}
