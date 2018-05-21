using System;
using System.Collections.Generic;
using System.Text;

namespace Rebus.Messages
{
    public class ShippingOrderMessage : IMessageContract
    {
        public long RouteId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public decimal Weight { get; set; }
        public string PackageDescription { get; set; }
        public long AddressId { get; set; }
    }
}
