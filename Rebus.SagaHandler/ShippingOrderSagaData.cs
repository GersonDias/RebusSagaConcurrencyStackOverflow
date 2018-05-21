using System;
using System.Collections.Generic;
using Rebus.Messages;
using Rebus.Sagas;

namespace Rebus.SagaHandler
{
    public class ShippingOrderSagaData : ISagaData
    {
        public Guid Id { get; set; }
        public int Revision { get; set; }

        public List<ShippingActivity> ShippingActivities = new List<ShippingActivity>();

        public long RouteId { get; set; }
        
        public void AddShippingActivity(ShippingActivity shippingActivity)
        {
            ShippingActivities.Add(shippingActivity);
        }
    }
}