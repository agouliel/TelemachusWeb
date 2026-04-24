using System;

namespace Telemachus.Business.Models
{
    public class EventConditionBusinessModel
    {
        public int Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Name { get; set; }
    }
}
