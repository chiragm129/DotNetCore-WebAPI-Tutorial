﻿using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIProject.Model
{
    public class Menupermission
    {
        public string code {  get; set; }
        public string Name { get; set; }
        public bool Haveview { get; set; }
        public bool Haveadd { get; set; }
        public bool Haveedit { get; set; }
        public bool Havedelete { get; set; }
    }
}
