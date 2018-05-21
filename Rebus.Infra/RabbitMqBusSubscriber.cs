using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
using Rebus.Messages;
using Rebus.Routing.TypeBased;
using Rebus.Sagas.Exclusive;

namespace Rebus.Infra
{
    public class RabbitMqBusSubscriber : IDisposable
    {
        private readonly BuiltinHandlerActivator _activator;

        public RabbitMqBusSubscriber(string rabbitMqConnectionString, string databaseConnectionString, 
            string inputQueueName, string publisherQueueName)
        {
            _activator = new BuiltinHandlerActivator();

            Configure.With(_activator)
                .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, inputQueueName))
                .Logging(l => l.ColoredConsole(LogLevel.Info))
                .Routing(r => r.TypeBased().MapAssemblyOf<IMessageContract>(publisherQueueName))
                .Sagas(s =>
                {
                    s.StoreInSqlServer(databaseConnectionString, "Sagas", "SagaIndex");
                    s.EnforceExclusiveAccess();
                })
                .Timeouts(t => { t.StoreInSqlServer(databaseConnectionString, "Timeouts"); })
                .Start();
        }

        public async Task Subscribe<T>(IHandleMessages<T> handler)
        {
            _activator.Register((bus, context) => handler);
            await _activator.Bus.Subscribe<T>();
        }

        public async Task DeferLocal(TimeSpan delay, IMessageContract message)
        {
            await _activator.Bus.DeferLocal(delay, message);
        }

        public void Dispose()
        {
            _activator?.Bus?.Dispose();
            _activator?.Dispose();
        }
    }
}
