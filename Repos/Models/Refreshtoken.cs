using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPIProject.Repos.Models;

[Table("refreshtoken")]
public partial class Refreshtoken
{
    [Key]
    [Column("userid")]
    [StringLength(50)]
    [Unicode(false)]
    public string Userid { get; set; } = null!;

    [Column("tokenid")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Tokenid { get; set; }

    [Column("refreshtoken")]
    [Unicode(false)]
    public string? Refreshtoken1 { get; set; }
}
