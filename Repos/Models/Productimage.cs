﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPIProject.Repos.Models;

[Table("productimage")]
public partial class Productimage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("productcode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Productcode { get; set; }

    [Column("productimage", TypeName = "image")]
    public byte[]? Productimage1 { get; set; }
}
