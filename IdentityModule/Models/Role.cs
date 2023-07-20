using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityModule.Models
{
    public class Role : IdentityRole<long>
    {
        public Role(string name) : base(name)
        {
            
        }
    }
}
