using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Enums;

namespace Telemachus.Business.Models.Reports
{

    public class ReportExportTankViewModel
    {
        public int GroupId { get; set; }
        public Guid GroupBusinessId { get; set; }
        public int? DisplayOrder { get; set; }
        public string TankName { get; set; }
        public string BdnNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double? SulphurContent { get; set; }
        public double? Sounding { get; set; }
        public double? TapeReading { get; set; }
        public double? BobReading { get; set; }
        public double? Ullage { get; set; }
        public double? TankTemp { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? Volume { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? Density { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? Vcf { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? Gsv { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? Wcf { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? Weight { get; set; }
        public double? Viscosity { get; set; }
        public bool IsSettOrServ { get; set; }
        public virtual bool isHfo
        {
            get
            {
                if (!Viscosity.HasValue)
                {
                    return false;
                }
                return GroupBusinessId == ReportType.RobHfoPoolGroup && Viscosity > 80;
            }
        }
        public virtual bool isLfo
        {
            get
            {
                if (!Viscosity.HasValue)
                {
                    return false;
                }
                return GroupBusinessId == ReportType.RobHfoPoolGroup && Viscosity <= 80;
            }
        }
        public virtual bool isMdoMgo
        {
            get
            {
                if (!Viscosity.HasValue)
                {
                    return false;
                }
                return GroupBusinessId == ReportType.RobMgoPoolGroup;
            }
        }
    }

    public class ReportExportViewModel
    {
        public List<ReportExportTankViewModel> Tanks { get; set; } = new List<ReportExportTankViewModel>();
        [Display(Name = "Rev")]
        public int? Revision { get; set; } = 3;
        public string Document { get; set; } = "FORM MRV 01";
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Rank")]
        public string Rank { get; set; }
        public string VesselName { get; set; }
        public string OperatorName { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Timestamp { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
        [Display(Name = "Weather")]
        public string Weather { get; set; }
        [Display(Name = "Sea Condition")]
        public string SeaCondition { get; set; }
        [Display(Name = "Draft Fwd(m)")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double? DraftFwd { get; set; }
        [Display(Name = "Draft Aft(m)")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double? DraftAft { get; set; }
        public double? VesselTrim { get; set; }
        [Display(Name = "Vessel's current list (°Deg)")]
        public double? VesselCurrentList { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? HfoPerGradesTov { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? HfoPerGradesWeight { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? LfoPerGradesTov { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? LfoPerGradesWeight { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? MdoMgoPerGradesTov { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? MdoMgoPerGradesWeight { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? NsResidualPerGradesTov { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? NsResidualPerGradesWeight { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? NsDestilatePerGradesTov { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? NsDestilatePerGradesWeight { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? TotalFuelPerGradesTov { get; set; }
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? TotalFuelPerGradesWeight { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? PerSulphurHighTov { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? PerSulphurHighWeight { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? PerSulphurMidTov { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? PerSulphurMidWeight { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? PerSulphurLowTov { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}")]
        public double? PerSulphurLowWeight { get; set; }

        // Consumptions

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsHfoMe { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsHfoGe { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsHfoBoiler { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsLfoMe { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsLfoGe { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsLfoBoiler { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsMgoMe { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsMgoGe { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsMgoBoiler { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsMgoInc { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsMgoIgg { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsMgoEmCy { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double? ConsMgoCargoCumm { get; set; } = 0;
    }
}
