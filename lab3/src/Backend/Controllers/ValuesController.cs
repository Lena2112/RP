﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Backend.Dto;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDatabase _redisDb;
        private readonly IConfiguration _configuration;

        public ValuesController(IConfiguration configuration)
        {
             _configuration = configuration;             
             ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
             _redisDb = redis.GetDatabase();
        }        

        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            string value = _redisDb.StringGet(id);
            
            return  value;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody] DataTransferDto dataTransfer)
        {
            if(dataTransfer == null)
            {
                return null;
            }

            var id = Guid.NewGuid().ToString();

            _redisDb.StringSet(id, dataTransfer.Data);
            Publish(id);

            return id;
        }

        private void Publish(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    /*channel.exchange_declare(exchange="backend-api",
                         exchange_type='fanout')*/
                    channel.QueueDeclare(queue: "backend-api",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "backend-api",
                                        routingKey: "",
                                        basicProperties: null,
                                        body: body);

                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }
       }
    }    
}
