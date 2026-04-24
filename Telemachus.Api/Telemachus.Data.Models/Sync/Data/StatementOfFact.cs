using System;

namespace Telemachus.Data.Models.Sync.Data
{
    public class StatementOfFact : EntityBase
    {
        public string Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string LastEventId { get; set; }
        public string FirstEventId { get; set; }
        public bool Completed { get; set; }
        public DateTime? Date { get; set; }
        public string OperationGrade { get; set; }
        public string Voyage { get; set; }
        public Guid? PortId { get; set; }
        public string Remarks { get; set; }
        public string Terminal { get; set; }
        public string CharterParty { get; set; }
        public int DisplayIndex { get; set; }
    }
}
