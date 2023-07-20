using System;
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
    [Table("Users")]
    public class User : IdentityUser<long>
    {
        [Required]
        public string Name { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
