using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models.Reports
{
    public class ReportContextDataModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ReportId { get; set; }

        [ForeignKey(nameof(ReportId))]
        public ReportDataModel Report { get; set; }
        public int? PrevContextId { get; set; }
        [ForeignKey(nameof(PrevContextId))]
        public ReportContextDataModel PrevContext { get; set; }
        public int TankId { get; set; }
        [ForeignKey(nameof(TankId))]
        public TankDataModel Tank { get; set; }
        public int? BunkeringId { get; set; }
        [ForeignKey(nameof(BunkeringId))]
        public BunkeringDataModel Bunkering { get; set; }
        public ICollection<ReportFieldValueDataModel> ReportFieldValues { get; set; }

        [Required]
        public DateTime DateModified { get; set; }

    }
}
