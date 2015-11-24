using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Pos.Test.Helper
{
    public static class TestHelper
    {
        public static void Post(string data, string hostname = "localhost", int port = 6740)
        {
            using (var client = new TcpClient())
            {

                client.Connect(hostname, port);

                using (var writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
                {
                    writer.WriteLine(data);
                }
            }
        }
    }
}