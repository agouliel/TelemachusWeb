using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telemachus.Data.Models.Ports
{
    [Table("PortSeaRegions")]
    public class Region
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? AreaId { get; set; }
        [ForeignKey("AreaId")]
        public Area Area { get; set; }
        public DateTime DateModified { get; set; }
        public ICollection<Port> Ports { get; } = new Collection<Port>();
        public ICollection<Country> Countries { get; } = new Collection<Country>();
        public string BusinessId { get; set; }
    }

}
