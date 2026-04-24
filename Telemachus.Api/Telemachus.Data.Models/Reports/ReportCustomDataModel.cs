using System;
using System.Collections.Generic;

using Telemachus.Data.Models.Cargo;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models.Reports
{
    public class ReportCustomDataModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int EventId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int StatusId { get; set; }
        public string EventTypeName { get; set; }
        public Guid EventTypeBusinessId { get; set; }
        public int? ReportTypeId { get; set; }
        public string EventConditionName { get; set; }
        public string PortName { get; set; }
        public int ReportId { get; set; }
        public string PortBusinessId { get; set; }
        public int? PortIsEuInt { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public int VoyageId { get; set; }
        public BunkeringDataModel BunkeringData { get; set; }
        public List<ReportFieldValueDataModel> FieldValues { get; set; }
        public List<CargoModel> Cargoes;
    }
}
