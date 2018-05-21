using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Messages;

namespace Rebus.Infra
{
    public class RabbitMqBusPublisher : IDisposable
    {
        private readonly BuiltinHandlerActivator _activator;
        public RabbitMqBusPublisher(string rabbitMqConnectionString, string inputQueueName)
        {
            _activator = new BuiltinHandlerActivator();

            Configure.With(_activator)
                .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, inputQueueName))
                .Logging(l => l.ColoredConsole())
                .Start();
        }

        public async Task Publish(IMessageContract message)
        {
            await _activator.Bus.Publish(message);
        }

        public void Dispose()
        {
            _activator?.Bus?.Dispose();
            _activator?.Dispose();
        }
    }
}