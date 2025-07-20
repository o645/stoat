using System.Net;
using System.Net.Sockets;
using System.Text;

namespace webServer;

class Program
{
    public static bool KILLSWITCH = false;
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
            }
        }
    }
}