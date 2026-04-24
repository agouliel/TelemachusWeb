using System;
using System.ComponentModel.DataAnnotations;

namespace Telemachus.Data.Models
{
    public class EntityBase
    {
        [Required]
        public DateTime DateModified { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}
