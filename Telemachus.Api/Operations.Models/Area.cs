using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Operations.Models
{
    [Table("Areas")]
    public class Area
    {
        [Key]
        [Column("AreaId")]
        public int Id { get; set; }
        [Column("AreaName")]
        [Required]
        public string Name { get; set; }
        [Column("AreaCode")]
        public string Code { get; set; }
        public ICollection<Region> Regions { get; } = new Collection<Region>();
        public ICollection<AreaCoordinate> AreaCoordinates { get; } = new Collection<AreaCoordinate>();
        public ICollection<Port> Ports { get; } = new Collection<Port>();
        public string BusinessId { get; set; }
        [Column("DateModified2")]
        public DateTime DateModified { get; set; }

    }

}
