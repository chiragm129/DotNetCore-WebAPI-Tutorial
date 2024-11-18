using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPIProject.Repos.Models;

[Table("customer")]
public partial class Customer
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
}
