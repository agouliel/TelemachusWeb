using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using NetTopologySuite.Geometries;

using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models.Ports
{
    [Table("Ports")]
    public class Port
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        //public Point Point { get; private set; }

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
        public DateTime DateModified { get; set; }
        public string BusinessId { get; set; }
        public ICollection<EventDataModel> Events { get; } = new Collection<EventDataModel>();
        public ICollection<StatementOfFact> StatementOfFacts { get; } = new Collection<StatementOfFact>();
        [NotMapped]
        public string CodeOrName
        {
            get
            {
                return string.IsNullOrEmpty(Code) ? Name : Code;
            }
        }
        public Point Point { get; set; }

        [NotMapped]
        public double? Distance { get; set; }
        public int? IsEuInt { get; set; }

    }

}

