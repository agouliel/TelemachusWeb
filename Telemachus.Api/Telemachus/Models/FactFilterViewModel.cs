using System;
using System.Collections.Generic;

namespace Telemachus.Models
{
    public class FactFilterViewModel
    {
        public List<int> EventTypeIds { get; set; } = new List<int>();
        public List<int> EventStatuses { get; set; } = new List<int>();
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<int> EventIdsToSkip { get; set; } = new List<int>();
    }
}
