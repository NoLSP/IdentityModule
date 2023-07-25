using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModule.Authorize;
using IdentityModule.Database;
using IdentityModule.Models;
using IdentityModule.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdentityModule.Controllers
{
    [Authorize(Policy = PolicyNames.Developer)]
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
            var userFromDb = _db.Users
                .Include(x => x.Roles)
                .FirstOrDefault(x => x.Id == userId);

            if (userFromDb == null)
            {
                return NotFound();
            }

            var model = new UserViewModel()
            {
                Id = userFromDb.Id,
                Name = userFromDb.Name,
                UserName = userFromDb.UserName!,
                Email = userFromDb.Email!,
                Roles = _db.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = userFromDb.Roles.Contains(x)
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
                var hasChanged = false;
                var objFromDbRolesIds = objFromDb.Roles.Select(x => x.Id).ToList();

                //Add
                foreach(var roleId in user.RoleIds.Where(x => !objFromDbRolesIds.Contains(x)))
                {
                    hasChanged = true;
                    await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(x => x.Id == roleId)!.Name!);
                }

                //Remove
                foreach(var roleId in objFromDbRolesIds.Where(x => !user.RoleIds.Contains(x)))
                {
                    hasChanged = true;
                    await _userManager.RemoveFromRoleAsync(objFromDb, _db.Roles.FirstOrDefault(x => x.Id == roleId)!.Name!);
                }

                //Attributes
                if(objFromDb.UserName != user.UserName)
                {
                    hasChanged = true;
                    objFromDb.UserName = user.UserName;
                }
                if(objFromDb.Email != user.Email)
                {
                    hasChanged = true;
                    objFromDb.Email = user.Email;
                }
                if(objFromDb.Name != user.Name)
                {
                    hasChanged = true;
                    objFromDb.Name = user.Name;
                }

                if(hasChanged)
                {
                    objFromDb.Modified = DateTime.UtcNow;
                    _db.SaveChanges();
                }

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
        public async Task<IActionResult> LockUnlock(long userId)
        {
            var objFromDb = await _db.Users.FindAsync(userId);
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
        public async Task<IActionResult> Delete(long userId)
        {
            var objFromDb = await _db.Users.FindAsync(userId);
            if (objFromDb == null)
            {
                return NotFound();
            }

            _db.Users.Remove(objFromDb);
            _db.SaveChanges();
            
            TempData[SD.Success] = "User deleted successfully.";
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(long userId)
        {
            var user = await _db.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel()
            {
                UserId = userId
            };

            foreach (Claim claim in ClaimStore.ClaimsList)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if (existingUserClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel userClaimsViewModel)
        {
            var user = await _db.Users.FindAsync(userClaimsViewModel.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while removing claims";
                return View(userClaimsViewModel);
            }

            result = await _userManager.AddClaimsAsync(user,
                userClaimsViewModel.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.IsSelected.ToString()))
                );

            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while adding claims";
                return View(userClaimsViewModel);
            }

            TempData[SD.Success] = "Claims updated successfully";
            
            return RedirectToAction(nameof(Index));
        }
    }
}
