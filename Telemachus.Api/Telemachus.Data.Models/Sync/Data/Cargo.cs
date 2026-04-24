using System;

namespace Telemachus.Data.Models.Sync.Data
{
    public class Cargo : EntityBase
    {
        public string Id { get; set; }
        public Guid GradeId { get; set; }
        public int Parcel { get; set; }
        public DateTimeOffset? StartedOn { get; set; }
        public DateTimeOffset? CompletedOn { get; set; }
        public int DisplayIndex { get; set; }
    }
}
