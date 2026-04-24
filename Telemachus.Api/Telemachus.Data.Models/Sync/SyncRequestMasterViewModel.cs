using System;
using Telemachus.Models;

namespace Telemachus.Data.Models.Sync
{
    public class SyncRequestMasterViewModel
    {
        public string User { get; set; }
        public DateTime Status { get; set; }
        public DateTime Tanks { get; set; }
        public DateTime TankUserSpecs { get; set; }
        public DateTime ReportFieldGroups { get; set; }
        public DateTime ReportFields { get; set; }
        public DateTime ReportTypes { get; set; }
        public DateTime ReportFieldsRelations { get; set; }
        public DateTime Conditions { get; set; }
        public DateTime EventTypes { get; set; }
        public DateTime EventTypesCondition { get; set; }
        public DateTime EventTypePrerequisites { get; set; }
        public DateTime Areas { get; set; }
        public DateTime AreaCoordinates { get; set; }
        public DateTime Regions { get; set; }
        public DateTime Countries { get; set; }
        public DateTime Ports { get; set; }
        public DateTime DocumentTypes { get; set; }
        public DateTime Grades { get; set; }
        public VesselDetails VesselDetails { get; set; }
    }
}
