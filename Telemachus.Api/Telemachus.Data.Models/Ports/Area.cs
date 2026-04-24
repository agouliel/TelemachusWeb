using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telemachus.Data.Models.Ports
{
    [Table("PortAreas")]
    public class Area
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime DateModified { get; set; }
        public string BusinessId { get; set; }
        public ICollection<Region> Regions { get; } = new Collection<Region>();
        public ICollection<AreaCoordinate> AreaCoordinates { get; } = new Collection<AreaCoordinate>();
        public ICollection<Port> Ports { get; } = new Collection<Port>();
    }

}
