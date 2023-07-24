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

    public static class Role 
    {

        public static async Task<IdentityRole<long>?> Find(RoleManager<IdentityRole<long>> roleManager, string name)
        {
            return await roleManager.FindByNameAsync(name);
        }

        public static async Task<IdentityRole<long>> Obtain(RoleManager<IdentityRole<long>> roleManager, string name)
        {
            var role = await Find(roleManager, name);

            if(role == null)
            {
                role = new IdentityRole<long>(name);

                var createResult = await roleManager.CreateAsync(role);
                if(!createResult.Succeeded)
                {
                    throw new Exception(String.Join("\n", createResult.Errors.Select(error => $"{error.Code} : {error.Description}")));
                }
            }

            return role;
        }

        public static async Task<IdentityRole<long>> User(RoleManager<IdentityRole<long>> roleManager)
        {
            return await Obtain(roleManager, RoleNames.User);
        }

        public static async Task<IdentityRole<long>> Administrator(RoleManager<IdentityRole<long>> roleManager)
        {
            return await Obtain(roleManager, RoleNames.Administrator);
        }

        public static async Task<IdentityRole<long>> Developer(RoleManager<IdentityRole<long>> roleManager)
        {
            return await Obtain(roleManager, RoleNames.Developer);
        }
    }
}
