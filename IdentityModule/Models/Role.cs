﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityModule.Models
{
    [Table("Roles")]
    public partial class Role : IdentityRole<long>
    {
        public Role(string name) : base(name){}

        public List<User> Users { get; } = new();
        public List<IdentityUserRole<long>> UserRoles { get; } = new();
    }
}
