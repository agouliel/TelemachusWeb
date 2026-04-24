using System;

namespace Telemachus.Data.Models.Events
{
    public class ConditionDtoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int VoyageId { get; set; }
        public Guid ConditionKey { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
