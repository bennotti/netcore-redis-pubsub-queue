using ConsoleRedisPub.Queue;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleRedisPub
{
    class Program
    {
        private const string RedisConnectionString = "localhost,port: 6379,password=123456";
        private static ConnectionMultiplexer connection =
          ConnectionMultiplexer.Connect(RedisConnectionString);

        private const string PubSubChannel =
                "pubsub-simple-channel"; // Can be anything we want.
        private static string name = string.Empty;
        private static EmailTaskQueue fila = new EmailTaskQueue();

        static void Main(string[] args)
        {
            Console.Write("Ideal rodar 2 instancias para melhor visualização funcionando!");
            Console.Write("Name: ");
            name = Console.ReadLine();

            // Create pub/sub
            var pubsub = connection.GetSubscriber();

            // Subscriber subscribes to a channel
            // le do redis
            pubsub.Subscribe(PubSubChannel, (channel, message) => MessageAction(message));

            // Notify subscriber(s) if you're joining
            pubsub.Publish(PubSubChannel, $"'{name}' joined.");

            // thread rodando em paralelo para processar a fila
            _ = Task.Run(async () => {
                while (true)
                {
                    var job = await fila.ObterAsync(CancellationToken.None);
                    if (job != null)
                    {
                        Console.WriteLine("processou: " + job.Valor);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2), CancellationToken.None);
                }
            });

            // Comunicação com o redis.. tudo que for digitado no console vai se enviado para os inscritos no canal
            while (true) {
                // publica no redis
                pubsub.Publish(PubSubChannel, $"{name}: {Console.ReadLine()}  " +
                  $"({DateTime.Now.Hour}:{DateTime.Now.Minute})");
            }
        }

        private static void MessageAction(RedisValue message)
        {
            // We'll implement it later, to show the message.
            int initialCursorTop = Console.CursorTop;
            int initialCursorLeft = Console.CursorLeft;

            Console.MoveBufferArea(0, initialCursorTop, Console.WindowWidth,
                                   1, 0, initialCursorTop + 1);
            Console.CursorTop = initialCursorTop;
            Console.CursorLeft = 0;

            // Print the message here
            Console.WriteLine(message);
            fila.Adicionar(new EmailJobDTO { Valor = message });

            Console.CursorTop = initialCursorTop + 1;
            Console.CursorLeft = initialCursorLeft;
        }
    }
}
