using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Ports;

namespace Telemachus.Data.Models
{
    [Table("statement_of_facts")]
    public class StatementOfFact : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [Required]
        public DateTime FromDate { get; set; }
        [Required]
        public DateTime ToDate { get; set; }
        public int? LastEventId { get; set; }
        public int? FirstEventId { get; set; }
        [Required]
        public bool Completed { get; set; } = false;
        public DateTime? Date { get; set; }
        public string OperationGrade { get; set; }
        public string Voyage { get; set; }
        public int? PortId { get; set; }
        [ForeignKey("PortId")]
        public Port Port { get; set; }
        public string Remarks { get; set; }
        public string Terminal { get; set; }
        public string CharterParty { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string BusinessId { get; set; }

    }
}
