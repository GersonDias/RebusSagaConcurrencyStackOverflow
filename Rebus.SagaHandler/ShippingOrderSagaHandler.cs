using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rebus.Handlers;
using Rebus.Infra;
using Rebus.Messages;
using Rebus.Sagas;

namespace Rebus.SagaHandler
{
    public class ShippingOrderSagaHandler : Saga<ShippingOrderSagaData>,
        IAmInitiatedBy<ShippingOrderMessage>,
        IHandleMessages<VerifyRoutePlanIsComplete>
    {
        private readonly RabbitMqBusSubscriber _subscriber;

        public ShippingOrderSagaHandler(RabbitMqBusSubscriber subscriber)
        {
            _subscriber = subscriber;
        }

        protected override void CorrelateMessages(ICorrelationConfig<ShippingOrderSagaData> config)
        {
            config.Correlate<ShippingOrderMessage>(x => x.RouteId, y => y.RouteId);
            config.Correlate<VerifyRoutePlanIsComplete>(x => x.RouteId, y => y.RouteId);
        }

        public async Task Handle(ShippingOrderMessage message)
        {
            Data.AddShippingActivity(message.AsShippingActivity());

            if (IsNew)
            {
                await _subscriber.DeferLocal(TimeSpan.FromSeconds(60),
                    new VerifyRoutePlanIsComplete()
                    {
                        RouteId = message.RouteId
                    });
            }
        }

        public async Task Handle(VerifyRoutePlanIsComplete message)
        {
            var groupedTasks = Data.ShippingActivities
                .GroupBy(x => x.RouteId)
                .SelectMany(routeList =>
                {
                    var firstActivity = routeList.First();
                    var activitiesForEachAddress = routeList.GroupBy(x => x.AddressId).Select(group =>
                    {
                        var groupedActivities = new ShippingActivity
                        {
                            Address = firstActivity.Address,
                            AddressId = firstActivity.AddressId,
                            Packages = group.SelectMany(x => x.Packages).ToList(),
                            RouteId = firstActivity.RouteId
                        };

                        return groupedActivities;
                    });

                    return activitiesForEachAddress;
                });

            await SomeOtherAsyncOperation(groupedTasks);

            MarkAsComplete();
        }

        private async Task SomeOtherAsyncOperation(IEnumerable<ShippingActivity> groupedTasks)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}
