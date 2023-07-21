using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityModule.Models;
using IdentityModule.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityModule.Controllers
{
    [Authorize]
    [Area("Identity")]
    public class IdentityController : Controller
    {
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        public IdentityController(RoleManager<IdentityRole<long>> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Install()
        {
            Task.Run(async () =>
            {
                //Create Roles
                if(!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole<long>("Admin"));
                }
                if(!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole<long>("User"));
                }
            });

            return View();
        }
    }
}
