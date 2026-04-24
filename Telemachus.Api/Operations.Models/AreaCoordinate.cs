using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Operations.Models
{
    [Table("AreaCoordinates")]
    public class AreaCoordinate
    {

        //public event PropertyChangedEventHandler PropertyChanged = delegate { };
        [Key]
        [Column("AreaCoordinatesId")]
        public int Id { get; set; }
        [Required]
        public int AreaId { get; set; }
        [Column("XCoord")]
        [Required]
        public double Lng { get; set; }
        [Column("YCoord")]
        [Required]
        public double Lat { get; set; }
        [Required]
        public int PointIndex { get; set; }
        [ForeignKey("AreaId")]
        public Area Area { get; set; }

        public string BusinessId { get; set; }
        [Column("DateModified2")]
        public DateTime DateModified { get; set; }

    }

}

