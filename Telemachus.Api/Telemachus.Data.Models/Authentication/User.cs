using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models.Authentication
{
    public class User : IdentityUser
    {
        [Column(TypeName = "varchar(50)")]
        public string Operator { get; set; }
        public double? PitchPropeller { get; set; } = null;
        public double? MainEngineMaxPower { get; set; } = null;
        public int? MainEngineAlarmPower { get; set; } = null;
        public int AvailablePasscodeSlots { get; set; } = 0;
        public string Prefix { get; set; }
        [MaxLength(255)]
        public string RemoteAddress { get; set; }
        [Range(1, 65535)]
        public int RemotePort { get; set; }
        public bool NonPool { get; set; } = false;
        public bool NonHafnia { get; set; } = false;
        public ICollection<EventDataModel> Events { get; set; } = new Collection<EventDataModel>();
        public ICollection<VoyageDataModel> Voyages { get; set; } = new Collection<VoyageDataModel>();
        public ICollection<StatementOfFact> Statements { get; set; } = new Collection<StatementOfFact>();
    }
}
