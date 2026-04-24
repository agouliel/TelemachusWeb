using System.Collections.Generic;
using Telemachus.Models;

namespace Telemachus.Data.Models.Sync
{
    public class SyncResponseMasterViewModel
    {
        public List<MasterData.Status> Status { get; set; }
        public List<MasterData.Tank> Tanks { get; set; }
        public List<MasterData.TankUserSpecs> TankUserSpecs { get; set; }
        public List<MasterData.ReportFieldGroup> ReportFieldGroups { get; set; }
        public List<MasterData.ReportField> ReportFields { get; set; }
        public List<MasterData.ReportType> ReportTypes { get; set; }
        public List<MasterData.ReportFieldRelation> ReportFieldsRelations { get; set; }
        public List<MasterData.Condition> Conditions { get; set; }
        public List<MasterData.EventType> EventTypes { get; set; }
        public List<MasterData.EventTypeCondition> EventTypeConditions { get; set; }
        public List<MasterData.EventTypePrerequisite> EventTypePrerequisites { get; set; }
        public List<MasterData.Area> Areas { get; set; }
        public List<MasterData.AreaCoordinate> AreaCoordinates { get; set; }
        public List<MasterData.Region> Regions { get; set; }
        public List<MasterData.Country> Countries { get; set; }
        public List<MasterData.Port> Ports { get; set; }
        public List<MasterData.DocumentType> DocumentTypes { get; set; }
        public List<MasterData.Grade> Grades { get; set; }
        public long LengthInBytes { get; set; } = 0;
        public VesselDetails VesselDetails { get; set; }

    }
}
