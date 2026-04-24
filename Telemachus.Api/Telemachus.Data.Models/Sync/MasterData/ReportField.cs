using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class ReportField : EntityBase
    {
        public Guid Id { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? TankId { get; set; }
        public string Name { get; set; }
        public bool? IsSubgroupMain { get; set; }
        public string ValidationKey { get; set; }
        public string Description { get; set; }
        public int DisplayIndex { get; set; }
    }
}
