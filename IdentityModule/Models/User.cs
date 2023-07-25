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
        public string? Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public List<IdentityUserRole<long>> UserRoles { get; } = new();
        public List<Role> Roles { get; } = new();

        public async Task<bool> IsAdministrator(UserManager<User> userManager)
        {
            return await userManager.IsInRoleAsync(this, RoleNames.Administrator);
        }
    }
}
