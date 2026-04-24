using System.Collections.Generic;

namespace Telemachus.Data.Models.Events
{
    public class EventStatusDataModel : EntityMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<EventDataModel> Events { get; set; }
    }
}
