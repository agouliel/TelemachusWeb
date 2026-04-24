using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telemachus.Data.Models.Authentication
{
    [Table("UserPasscodes")]
    public class UserPasscode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [Required]
        public string Passcode { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
    }
}
