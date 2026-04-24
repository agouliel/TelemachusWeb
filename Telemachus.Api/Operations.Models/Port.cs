using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Operations.Models
{
    [Table("PortsV2")]
    public class Port
    {
        [Key]
        [Column("PortId")]
        public int Id { get; set; }

        [Column("PortName")]
        public string Name { get; set; }

        [Column("LatNum")]
        public double? Lat { get; set; }
        [Column("LongNum")]
        public double? Lng { get; set; }

        [Column("Unctad")]
        public string Code { get; set; }

        [Required]
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }
        [Required]
        public int RegionId { get; set; }

        [ForeignKey("RegionId")]
        public Region Region { get; set; }

        [Required]
        public int AreaId { get; set; }

        [ForeignKey("AreaId")]
        public Area Area { get; set; }
        public double? TimeZone { get; set; }
        public string BusinessId { get; set; }
        [Column("DateModified2")]
        public DateTime DateModified { get; set; }
        [Column("isEUint")]
        public int? IsEuInt { get; set; }
        [NotMapped]
        public string CodeOrName
        {
            get
            {
                return string.IsNullOrEmpty(Code) ? Name : Code;
            }
        }

    }

}

