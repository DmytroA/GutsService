using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Guts.Core.Entities;
using Guts.Reciever;
using Topshelf;
using Reciever;

public class RPCServer : IReceive
{
    //public static GutsLiveScoutEntities context = new GutsLiveScoutEntities();
    public static void Main()
    {
        HostFactory.Run(hostConfigurator =>
        {
            hostConfigurator.Service<RecieveQueue>(serviceConfigurator =>
            {
                serviceConfigurator.ConstructUsing(() => new RecieveQueue());
                serviceConfigurator.WhenStarted(recieveService => recieveService.Start());
                serviceConfigurator.WhenStopped(recieveService => recieveService.Stop());
            });

            hostConfigurator.RunAsLocalSystem();

            hostConfigurator.SetDisplayName("RecieveQueue");
            hostConfigurator.SetDescription("RecieveQueue using Topshelf");
            hostConfigurator.SetServiceName("RecieveQueue");
        });

    }
    public class RecieveQueue
    {
        public void Start()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "sendEntity",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var json = Encoding.UTF8.GetString(body);

                    var odds = JsonConvert.DeserializeObject<Guts.Core.Entities.IGamePlay>(json);

                    if (odds != null)
                    {
                        SaveOdds(odds);
                    }

                    Console.WriteLine(" [x] Received {0}", json);

                    int dots = json.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "sendEntity",
                                     noAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

        }
        public void Stop()
        {

        }
    }

    public static void SaveOdds<T>(T entity) where T : class, IGamePlay
    {
        using (GamePlayDataContext context = new GamePlayDataContext())
        {
            try
            {
                if (entity != null)
                {
                    context.Entry<T>(entity).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}