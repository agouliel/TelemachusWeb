using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class ReportFieldRelation : EntityBase
    {
        public Guid Id { get; set; }
        public Guid ReportFieldId { get; set; }
        public Guid ReportTypeId { get; set; }
        public int DisplayIndex { get; set; }
    }
}
