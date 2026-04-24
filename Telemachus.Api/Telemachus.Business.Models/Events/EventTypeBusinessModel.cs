using System;

namespace Telemachus.Business.Models.Events
{
    public class EventTypeBusinessModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Available { get; set; }
        public bool IsPairedEvent { get; set; }
        public bool Relevant { get; set; }
        public bool Suggested { get; set; }
        public string Comment { get; set; }
        public Guid BusinessId { get; set; }
        public Guid? NextConditionId { get; set; }
        public Guid? PairedEventNextConditionId { get; set; }
        public string NextConditionName { get; set; }
        public string PairedEventNextConditionName { get; set; }


    }
}

