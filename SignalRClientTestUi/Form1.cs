using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace SignalRClientTestUi
{
    public partial class Form1 : Form
    {
        HubConnection connection;
        public Form1()
        {
            InitializeComponent();
            connection = new HubConnectionBuilder().WithUrl("http://localhost:5000/streamhub", opt =>
            {
                opt.UseDefaultCredentials = true;
                opt.HttpMessageHandlerFactory = (msg) =>
                {
                    if (msg is HttpClientHandler clientHandler)
                    {
                        // bypass SSL certificate
                        clientHandler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, sslPolicyErrors) => { return true; };
                    }

                    return msg;
                };
            })
            .WithAutomaticReconnect()
            .AddMessagePackProtocol()
            .Build();

            connection.On<string, bool>("stream_handshake", async (input) =>
            {
                return true;
            });
        }

        public async Task StreamFromAiToServer()
        {
            var channel = Channel.CreateUnbounded<int>();
            await connection.SendAsync("ReplayFromAdminToServer", channel.Reader);
            for (int i = 0; i < 1000; i++)
            {
                await Task.Delay(100);
                await channel.Writer.WriteAsync(i);
                richTextBox1.AppendText(i.ToString()+"\r\n");
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            await connection.StartAsync();
            richTextBox1.AppendText("signalR server started\r\n");
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await StreamFromAiToServer();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
             
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.ScrollToCaret();
        }

        private async void button3_Click(object sender, EventArgs e)
        {

            var res = await connection.InvokeAsync<bool>("StreamHandshake", "stream_handshake");
            if (res)
            {
                await connection.SendAsync("ImageMessage", "Hello");
            }
        }
    }
}