using Guts.Core.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reciever
{
    public class RecieveQueueJson : IDisposable
    {
        GamePlayDataContext context;
        public RecieveQueueJson()
        {
            context = new GamePlayDataContext();
        }

        public void Start()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "LiveScout",
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
                    if (json != null)
                    {
                        //SaveToFile(json);
                    }
                    var odds = JsonConvert.DeserializeObject<LiveScoutEntity>(json);

                    if (odds != null)
                    {
                        SaveOdds(odds);
                    }

                    Console.WriteLine(" [x] Received {0} {1}", odds.Id, odds.EventType);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "LiveScout",
                                     noAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        public void Stop()
        {

        }
        public void SaveOdds(LiveScoutEntity entity)
        {

            try
            {
                if (entity != null)
                {
                    context.Entry(entity).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public void SaveToFile(string json)
        {
            using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"D:\Guts\WriteJson.txt", true))
            {
                file.WriteLine(json);
            }
        }
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
