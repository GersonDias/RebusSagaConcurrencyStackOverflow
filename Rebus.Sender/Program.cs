using System;
using System.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Rebus.Infra;
using Rebus.Messages;

namespace Rebus.Sender
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PublishTestMessages().Wait();
        }

        private static async Task PublishTestMessages()
        {
            var shippingOrder = new ShippingOrderMessage()
            {
                AddressId = 1,
                AddressLine1 = "The address line one",
                AddressLine2 = "The address line two",
                PackageDescription = "A huge package",
                RouteId = 1,
                Weight = 20.5M
            };

            var shippingOrder2 = new ShippingOrderMessage
            {
                AddressId = 1,
                AddressLine1 = "The address line one",
                AddressLine2 = "The address line two",
                PackageDescription = "A tiny package",
                RouteId = 1,
                Weight = 1.5M
            };

            var shippingOrder3 = new ShippingOrderMessage
            {
                AddressId = 2,
                AddressLine1 = "The address2 line one",
                AddressLine2 = "The address2 line two",
                PackageDescription = "A package for another client, same route id",
                RouteId = 1,
                Weight = 5
            };

            var shippingOrder4 = new ShippingOrderMessage
            {
                AddressId = 1,
                AddressLine1 = "The address line one",
                AddressLine2 = "The address line two",
                PackageDescription = "A package for the same address, but different routeid",
                RouteId = 2,
                Weight = 2
            };

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .Build();

            var publisher = new RabbitMqBusPublisher(
                configuration.GetConnectionString("RabbitMQ"),
                configuration.GetSection("AppSettings")["PublisherQueue"]);

            await publisher.Publish(shippingOrder);
            await publisher.Publish(shippingOrder2);
            await publisher.Publish(shippingOrder3);
            await publisher.Publish(shippingOrder4);
        }
    }
}
