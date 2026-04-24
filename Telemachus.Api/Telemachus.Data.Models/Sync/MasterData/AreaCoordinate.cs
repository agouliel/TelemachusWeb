using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class AreaCoordinate
    {
        public string Id { get; set; }
        public string AreaId { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public int PointIndex { get; set; }
        public DateTime DateModified { get; set; }
    }
}
