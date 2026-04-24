using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Operations.Models
{
    [Table("SeaRegions")]
    public class Region
    {
        [Key]
        [Column("SeaRegionId")]
        public int Id { get; set; }
        [Column("SeaRegionName")]
        public string Name { get; set; }
        public int? AreaId { get; set; }
        [ForeignKey("AreaId")]
        public Area Area { get; set; }
        public ICollection<Port> Ports { get; } = new Collection<Port>();
        public ICollection<Country> Countries { get; } = new Collection<Country>();
        public string BusinessId { get; set; }
        [Column("DateModified2")]
        public DateTime DateModified { get; set; }
    }

}
