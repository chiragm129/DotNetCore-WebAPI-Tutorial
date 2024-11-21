using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPIProject.Repos.Models;

[Table("rolepermission")]
public partial class Rolepermission
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userrole")]
    [StringLength(50)]
    [Unicode(false)]
    public string Userrole { get; set; } = null!;

    [Column("menucode")]
    [StringLength(50)]
    [Unicode(false)]
    public string Menucode { get; set; } = null!;

    [Column("haveview")]
    public bool Haveview { get; set; }

    [Column("haveadd")]
    public bool Haveadd { get; set; }

    [Column("haveedit")]
    public bool Haveedit { get; set; }

    [Column("havedelete")]
    public bool Havedelete { get; set; }
}
