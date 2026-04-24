using System;
using System.Collections.Generic;
using Telemachus.Business.Models.Events;

namespace Telemachus.Business.Models.Cargo
{
    public class CargoDetailBusinessModel
    {
        public int Id { get; set; }
        public int CargoId { get; set; }
        public CargoBusinessModel Cargo { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public int? Quantity { get; set; }
        public int? QuantityLimit { get; set; }
        public string BusinessId { get; set; }
        public string PortName { get; set; }
        public string PortCountry { get; set; }
        public List<EventBusinessModel> Events { get; set; }
        public string EventTypeId { get; set; }
    }
}
