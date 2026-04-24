using System;
using System.Collections.Generic;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models
{
    public class ImportReportViewModel
    {
        public int Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public DateTimeOffset? ConditionStartedDate { get; set; }
        public Guid? CurrentVoyageConditionKey { get; set; }
        public int? PortId { get; set; }
        public int ConditionId { get; set; }
        public string PortName { get; set; }
        public string TimeZone { get; set; }
        public int EventTypeId { get; set; }
        public Guid EventTypeBusinessId { get; set; }
        public string EventTypeName { get; set; }
        public double? InstructedSpeed { get; set; }
        public double? RobHfo { get; set; }
        public double? RobMgo { get; set; }
        public double? DistanceToGo { get; set; }
        public double? DistanceOverGround { get; set; }
        public double? SteamingTimeHfo { get; set; }
        public double? SteamingTimeMgo { get; set; }
        public double? MeConsHfo { get; set; }
        public double? MeConsMgo { get; set; }
        public double? OutOfPerformance { get; set; }
        public double? Slip { get; set; }
        public double? SpeedOverGround { get; set; }
        public double? Rpm { get; set; }
        public double? TotalMeCons { get; set; }
    }
    public class ImportViewModel
    {
        public string VesselId { get; set; }
        public List<ImportReportViewModel> Reports { get; set; }
        public VoyageDataModel Voyage { get; set; }
    }
}
