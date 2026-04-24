
using System.Collections.Generic;
using Telemachus.Data.Models.Events;
using DesignModels = Telemachus.Business.Models.Reports.Design;

namespace Telemachus.Business.Models.Events.Design
{
    public class EventDesignViewModel
    {
        public List<DesignModels.EventTypeBusinessModel> EventTypes { get; set; }
        public List<EventConditionDataModel> Conditions { get; set; }
        public List<string> CustomEvents { get; set; }
    }
}
