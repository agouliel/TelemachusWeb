using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class ReportFieldGroup : EntityBase
    {
        public Guid Id { get; set; }
        public string FieldGroupName { get; set; }
        public int DisplayIndex { get; set; }
    }
}
