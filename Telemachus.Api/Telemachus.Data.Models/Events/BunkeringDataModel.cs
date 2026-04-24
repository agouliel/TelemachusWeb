using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Enums;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Ports;

namespace Telemachus.Data.Models.Events
{
    [Table("bunkering_data")]
    public class BunkeringDataModel : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Bdn { get; set; }
        public string SulphurContent { get; set; }
        public string Density { get; set; }
        public string Viscosity { get; set; }
        public int FuelType { get; set; }
        public string Supplier { get; set; }
        public string TotalAmount { get; set; }
        public string NamedAmount { get; set; }
        [NotMapped]
        public string RobAmount { get; set; }
        [NotMapped]
        public string RobAmountDiff { get; set; }
        [NotMapped]
        public DateTimeOffset RobAmountDiffTimestamp { get; set; }
        public int PortId { get; set; }
        [ForeignKey(nameof(PortId))]
        public Port Port { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string BusinessId { get; set; }
        public ICollection<BunkeringTankDataModel> Tanks { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [Required]
        public User User { get; set; }

        public bool IsVirtual { get; set; } = false;

        public ICollection<EventDataModel> Events { get; set; }
        [JsonIgnore]
        public virtual bool HasFuelTypeVLSFO
        {
            get
            {
                return FuelType == 1;
            }
        }
        [JsonIgnore]
        public virtual bool HasFuelTypeLSMGO
        {
            get
            {
                return FuelType == 2;
            }
        }
        [JsonIgnore]

        public virtual Guid BunkerGroup
        {
            get
            {
                return HasFuelTypeVLSFO ? ReportType.BunkerHfoGroup : ReportType.BunkerMgoGroup;
            }
        }

    }
}
