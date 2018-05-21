using System;
using System.Collections.Generic;
using System.Text;

namespace Rebus.Messages
{
    public static class ShippingActivityExtensions
    {
        public static ShippingActivity AsShippingActivity(this ShippingOrderMessage shippingOrder)
        {
            return new ShippingActivity
            {
                Address = $"{shippingOrder.AddressLine1} {shippingOrder.AddressLine2}",
                Packages = new List<Package>
                {
                    new Package {Description = shippingOrder.PackageDescription, Weight = shippingOrder.Weight}
                },
                RouteId = shippingOrder.RouteId,
                AddressId = shippingOrder.AddressId
            };
        }
    }

    public class ShippingActivity
    {
        public long RouteId { get; set; }
        public string Address { get; set; }
        public long AddressId { get; set; }
        public List<Package> Packages { get; set; }
    }

    public class Package
    {
        public decimal Weight { get; set; }
        public string Description { get; set; }
    }
}
