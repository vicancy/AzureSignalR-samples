using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace CSharpClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var accessToken = "";
            var hubName = "chat";
            var endpoint = "lianwei-aspnet-3.servicedev.signalr.net/client/";
            var connection = new HubConnectionBuilder().WithUrl(
                $"wss://{endpoint}?hub={hubName}&asrs.op=%2F{hubName}"
                , HttpTransportType.WebSockets,
                s =>
                {
                    s.AccessTokenProvider = () => Task.FromResult(accessToken);
                    s.SkipNegotiation = true;
                }
                ).ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            connection.Closed += async (error) =>
            {
                Console.WriteLine(error.Message);
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<string, string>("broadcastMessage", (name, message) =>
            {
                Console.WriteLine(name + ":" + message);
            });

            await connection.StartAsync();
            Console.ReadKey();
        }
    }
}
