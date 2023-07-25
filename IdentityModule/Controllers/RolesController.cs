using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using IdentityModule;
using IdentityModule.Authorize;
using IdentityModule.Database;
using IdentityModule.Models;
using IdentityModule.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityModule.Controllers
{
    [Authorize(Policy = PolicyNames.Developer)]
    public class RolesController : Controller
    {
        private readonly IdentityDataContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;


        public RolesController(IdentityDataContext db, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(long? id)
        {
            if(id == null)
                return View();

            var model = new RoleViewModel();

            var objFromDb = await _db.Roles.FindAsync(id);
            if(objFromDb != null)
            {
                model.Id = objFromDb.Id;
                model.Name = objFromDb.Name!;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(RoleViewModel roleObj)
        {
            if(await _roleManager.RoleExistsAsync(roleObj.Name))
            { 
                //error
                TempData[SD.Error] = "Role already exists.";
                return RedirectToAction(nameof(Index));
            }
            if (roleObj.Id == null)
            {
                //create
                await _roleManager.CreateAsync(new Role(roleObj.Name));
                TempData[SD.Success] = "Role created successfully";
            }
            else
            {
                //update
                var objRoleFromDb = _db.Roles.FirstOrDefault(u => u.Id == roleObj.Id);
                if (objRoleFromDb == null)
                {
                    TempData[SD.Error] = "Role not found.";
                    return RedirectToAction(nameof(Index));
                }
                objRoleFromDb.Name = roleObj.Name;
                objRoleFromDb.NormalizedName = roleObj.Name.ToUpper();
                var result = await _roleManager.UpdateAsync(objRoleFromDb);
                TempData[SD.Success] = "Role updated successfully";
            }
            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var objFromDb = _db.Roles.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                TempData[SD.Error] = "Role not found.";
                return RedirectToAction(nameof(Index));
            }
            var userRolesForThisRole = _db.UserRoles.Where(u => u.RoleId == id).Count();
            if (userRolesForThisRole > 0)
            {
                TempData[SD.Error] = "Cannot delete this role, since there are users assigned to this role.";
                return RedirectToAction(nameof(Index));
            }
            await _roleManager.DeleteAsync(objFromDb);
            TempData[SD.Success] = "Role deleted successfully.";
            return RedirectToAction(nameof(Index));

        }

    }
}
