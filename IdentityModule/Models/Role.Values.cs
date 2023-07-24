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
    public static class RoleNames
    {
        public const string User = "User";
        public const string Administrator = "Administrator";
        public const string Developer = "Developer";
    }

    public partial class Role 
    {
        public static async Task<Role?> Find(RoleManager<Role> roleManager, string name)
        {
            return await roleManager.FindByNameAsync(name);
        }

        public static async Task<Role> Obtain(RoleManager<Role> roleManager, string name)
        {
            var role = await Find(roleManager, name);

            if(role == null)
            {
                role = new Role(name);

                var createResult = await roleManager.CreateAsync(role);
                if(!createResult.Succeeded)
                {
                    throw new Exception(String.Join("\n", createResult.Errors.Select(error => $"{error.Code} : {error.Description}")));
                }
            }

            return role;
        }

        public static async Task<Role> User(RoleManager<Role> roleManager)
        {
            return await Obtain(roleManager, RoleNames.User);
        }

        public static async Task<Role> Administrator(RoleManager<Role> roleManager)
        {
            return await Obtain(roleManager, RoleNames.Administrator);
        }

        public static async Task<Role> Developer(RoleManager<Role> roleManager)
        {
            return await Obtain(roleManager, RoleNames.Developer);
        }
    }
}
