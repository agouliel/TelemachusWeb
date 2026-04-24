using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telemachus.Data.Models.Ports
{
    [Table("PortAreaCoordinates")]
    public class AreaCoordinate
    {

        //public event PropertyChangedEventHandler PropertyChanged = delegate { };
        [Key]
        public int Id { get; set; }
        [Required]
        public int AreaId { get; set; }
        [Required]
        public double Lng { get; set; }
        [Required]
        public double Lat { get; set; }
        [Required]
        public int PointIndex { get; set; }
        [ForeignKey("AreaId")]
        public Area Area { get; set; }
        public DateTime DateModified { get; set; }
        public string BusinessId { get; set; }

    }

}

