using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Operations.Models
{

    [Table("Countries")]
    public class Country
    {
        [Key]
        [Column("CountryId")]
        public int Id { get; set; }
        public string Numerical { get; set; }
        [Column("CountryName")]
        public string Name { get; set; }
        public string Alpha2 { get; set; }
        public string Alpha3 { get; set; }
        public string Nationality { get; set; }
        public int? RegionId { get; set; }
        public string LloydsCode { get; set; }
        public string PhoneCode { get; set; }
        [ForeignKey("RegionId")]
        public Region Region { get; set; }
        public ICollection<Port> Ports { get; } = new Collection<Port>();
        public string BusinessId { get; set; }
        [Column("DateModified2")]
        public DateTime DateModified { get; set; }

    }

}
