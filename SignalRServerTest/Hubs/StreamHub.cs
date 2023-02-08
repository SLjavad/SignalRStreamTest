using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;

namespace SignalRServerTest.Hubs
{
    public class StreamHub : Hub
    {
        public async Task ReplayFromAdminToServer(ChannelReader<int> replay)
        {
            try
            {
                while (await replay.WaitToReadAsync())
                {
                    while (replay.TryRead(out var item))
                    {
                        Console.WriteLine(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("END OF ReplayFromAdminToServer");
        }

        public async Task<bool> StreamHandshake(string data)
        {
            if (data == "stream_handshake")
            {
                try
                {
                    var token = new CancellationTokenSource();
                    var res = await Clients.Caller.InvokeAsync<bool>("stream_handshake", "some-data", token.Token);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EX in StreamHandshake");
                    Console.WriteLine(ex.Message);
                    return false;
                }

            }
            return false;
        }
    }
}
