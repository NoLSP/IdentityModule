using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModule.Database;
using IdentityModule.Models;
using IdentityModule.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdentityModule.Controllers
{
    public class UserController : Controller
    {
        private readonly IdentityDataContext _db;
        private readonly UserManager<User> _userManager;

        public UserController(IdentityDataContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userList = _db.Users
                .Include(x => x.Roles)
                .ToList();

            return View(userList);
        }

        public IActionResult Edit(long userId)
        {
            var objFromDb = _db.Users
                .Include(x => x.Roles)
                .FirstOrDefault(u=> u.Id == userId);

            if (objFromDb == null)
            {
                return NotFound();
            }

            var model = new UserViewModel()
            {
                Id = objFromDb.Id,
                Name = objFromDb.Name,
                UserName = objFromDb.UserName,
                Email = objFromDb.Email,
                Roles = _db.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = objFromDb.Roles.Contains(x)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel user)
        {
            var objFromDb = _db.Users
                .Include(x => x.Roles)
                .FirstOrDefault(u => u.Id == user.Id);
            if (objFromDb == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var objFromDbRolesIds = objFromDb.Roles.Select(x => x.Id).ToList();

                //Add
                foreach(var roleId in user.RoleIds.Where(x => !objFromDbRolesIds.Contains(x)))
                {
                    await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(x => x.Id == roleId)!.Name!);
                }

                //Remove
                foreach(var roleId in objFromDbRolesIds.Where(x => !user.RoleIds.Contains(x)))
                {
                    await _userManager.RemoveFromRoleAsync(objFromDb, _db.Roles.FirstOrDefault(x => x.Id == roleId)!.Name!);
                }

                //Attributes
                if(objFromDb.UserName != user.UserName)
                    objFromDb.UserName = user.UserName;
                if(objFromDb.Email != user.Email)
                    objFromDb.Email = user.Email;
                if(objFromDb.Name != user.Name)
                    objFromDb.Name = user.Name;
                _db.SaveChanges();
                TempData[SD.Success] = "User has been edited successfully.";
                return RedirectToAction(nameof(Index));
            }

            user.Roles = _db.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = objFromDb.Roles.Contains(x)
                }).ToList();

            return View(user);
        }

        [HttpPost]
        public IActionResult LockUnlock(long userId)
        {
            var objFromDb = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            if(objFromDb.LockoutEnd!=null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is locked and will remain locked untill lockoutend time
                //clicking on this action will unlock them
                objFromDb.LockoutEnd = DateTime.UtcNow;
                TempData[SD.Success] = "User unlocked successfully.";
            }
            else
            {
                //user is not locked, and we want to lock the user
                objFromDb.LockoutEnd = DateTime.UtcNow.AddYears(1000);
                TempData[SD.Success] = "User locked successfully.";
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public IActionResult Delete(long userId)
        {
            var objFromDb = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            _db.Users.Remove(objFromDb);
            _db.SaveChanges();
            TempData[SD.Success] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // [HttpGet]
        // public async Task<IActionResult> ManageUserClaims(long userId)
        // {
        //     IdentityUser user = await _userManager.FindByIdAsync(userId);

        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     var existingUserClaims = await _userManager.GetClaimsAsync(user);

        //     var model = new UserClaimsViewModel()
        //     {
        //         UserId = userId
        //     };

        //     foreach(Claim claim in ClaimStore.claimsList)
        //     {
        //         UserClaim userClaim = new UserClaim
        //         {
        //             ClaimType = claim.Type
        //         };
        //         if (existingUserClaims.Any(c => c.Type == claim.Type))
        //         {
        //             userClaim.IsSelected = true;
        //         }
        //         model.Claims.Add(userClaim);
        //     }

        //     return View(model);
        // }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel userClaimsViewModel)
        // {
        //     IdentityUser user = await _userManager.FindByIdAsync(userClaimsViewModel.UserId);

        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     var claims = await _userManager.GetClaimsAsync(user);
        //     var result = await _userManager.RemoveClaimsAsync(user,claims);

        //     if (!result.Succeeded)
        //     {
        //         TempData[SD.Error] = "Error while removing claims";
        //         return View(userClaimsViewModel);
        //     }

        //     result = await _userManager.AddClaimsAsync(user,
        //         userClaimsViewModel.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.IsSelected.ToString()))
        //         );

        //     if (!result.Succeeded)
        //     {
        //         TempData[SD.Error] = "Error while adding claims";
        //         return View(userClaimsViewModel);
        //     }

        //     TempData[SD.Success] = "Claims updated successfully";
        //     return RedirectToAction(nameof(Index));
        // }
    }
}
