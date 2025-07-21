using System.Net;
using System.Net.Sockets;

namespace webServer;

class Program
{
    static void Main(string[] args)
    {
        string FilePath = "../debugSite";
        string IP = "127.0.0.1";
        int Port = 80;

        if (args.Length > 0)
        {
            FilePath = args[0];
            Console.WriteLine($"Site located at {Path.GetFullPath(FilePath)}.");
            if (args.Length > 1)
            {
                IP = args[1];
                if (args.Length > 2)
                {
                    Port = int.Parse(args[2]);
                }
            }
        }
        
        StartServer(FilePath, IP,Port);
    }

    public static void StartServer(string FilePath, string IP, int Port)
    {
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Parse(IP), Port));
        server.Listen();
        Console.WriteLine("Hello, World!");
        int id = 1;
        while (true)
        {
            Connection client = new Connection(server.Accept(),id, FilePath);
            Task.Run(client.Loop);
            id++;
        }
    }
}