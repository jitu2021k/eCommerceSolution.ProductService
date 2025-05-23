﻿using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace eCommerce.ProductsService.BusinessLogicLayer.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        public RabbitMQPublisher(IConfiguration configuration)
        {
            _configuration = configuration; 
            string hostName = _configuration["RabbitMQ_HostName"]!;
            string userName = _configuration["RabbitMQ_UserName"]!;
            string password = _configuration["RabbitMQ_Password"]!;
            string port = _configuration["RabbitMQ_Port"]!;

            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                Port = Convert.ToInt32(port)
            };
            _connection = connectionFactory.CreateConnection();

            _channel = _connection.CreateModel();
        }

        public void Publish<T>(Dictionary<string,object> headers, T message)
        {
            string messageJson = JsonSerializer.Serialize(message);
            byte[] messgaeBodyInBytes = Encoding.UTF8.GetBytes(messageJson);

            //Create exchange
            string exchangeName = _configuration["RabbitMQ_Products_Exchange"]!;
            _channel.ExchangeDeclare(exchangeName,
                                type: ExchangeType.Headers,
                                durable: true);

            // For Header use Dictionary<string,object> in the place of routingKey
            // and pass basicProperties in below var basicProperties = _channel.CreateBasicProperties();
            //Pass routingKey as string.Empty 

            //Publish Message
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.Headers = headers;

            _channel.BasicPublish(exchange: exchangeName,
                                routingKey: string.Empty,
                                basicProperties: basicProperties,
                                body: messgaeBodyInBytes);
        }
        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
