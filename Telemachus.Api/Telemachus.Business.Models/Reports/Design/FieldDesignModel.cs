using System.Collections.Generic;

namespace Telemachus.Business.Models.Reports.Design
{
    public class FieldDesignModel
    {
        public string Name { get; set; }
        public string ValidationKey { get; set; }
        public string Description { get; set; }
        public bool? HasValues { get; set; }
        public bool? Hidden { get; set; }
        public List<int> ReportTypes { get; set; } = new List<int>();
        public List<int> Groups { get; set; } = new List<int>();
    }
}
