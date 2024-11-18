using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebAPIProject.Model
{
    public class Customermodel
    {
        [Key]
        public int Code { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string Email { get; set; } = null!;

        public int? Phone { get; set; }

        public int? Creditlimit { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? Address { get; set; }

        public bool? IsActive { get; set; }

        public string Statusname { get; set; }
    }
}
