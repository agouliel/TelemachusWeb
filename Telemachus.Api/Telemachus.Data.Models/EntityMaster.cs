using System;
using System.ComponentModel.DataAnnotations;

namespace Telemachus.Data.Models
{
    public class EntityMaster : EntityBase
    {
        [Required]
        public Guid BusinessId { get; set; } = Guid.NewGuid();
    }
}
