using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Rebus.Infra;
using Rebus.Messages;
using Rebus.SagaHandler;

namespace Rebus.SagaSubscriber
{
    public class Program
    {
        private static ShippingOrderSagaHandler _sagaHandler;
        private static RabbitMqBusSubscriber _subscriber;

        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .Build();

            _subscriber = new RabbitMqBusSubscriber(
                configuration.GetConnectionString("RabbitMQ"),
                configuration.GetConnectionString("Database"),
                configuration.GetSection("AppSettings")["SubscriberQueue"],
                configuration.GetSection("AppSettings")["PublisherQueue"]);


            _sagaHandler = new ShippingOrderSagaHandler(_subscriber);

            _subscriber.Subscribe<ShippingOrderMessage>(_sagaHandler);
            _subscriber.Subscribe<VerifyRoutePlanIsComplete>(_sagaHandler);
        }
    }
}
