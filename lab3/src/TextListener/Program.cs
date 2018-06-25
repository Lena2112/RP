using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var channel = RabbitMQ.GetModel())
            {
                RabbitMQ.DeclareQueue("backend-api", channel);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    string redisValue = String.Empty;
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                    IDatabase redisDb = redis.GetDatabase();
                    redisValue = redisDb.StringGet(message);
                                            
                    Console.WriteLine(" [x] Received from redis {0} with key: {1}", redisValue, message);    
                };

                RabbitMQ.ConsumeQueue("backend-api", consumer, channel);

                Console.WriteLine(" Press [enter] to exit.");                
                Console.ReadKey();                              
            }            
        }
    }
}
