using System;

namespace Rebus.Messages
{
    public class VerifyRoutePlanIsComplete : IMessageContract
    {
        public long RouteId { get; set; }
    }
}