using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace IdentityModule.Authorize
{
    public static class PolicyNames
    {
        public const string Administrator = "Administrator";
        public const string Developer = "Developer";
        public const string User = "User";
        public const string UserAndAdministrator = "UserAndAdministrator";
        public const string Admin_CreateAccess = "Admin_CreateAccess";
        public const string Admin_Create_Edit_DeleteAccess = "Admin_Create_Edit_DeleteAccess";
        public const string Admin_Create_Edit_DeleteAccess_OR_Developer = "Admin_Create_Edit_DeleteAccess_OR_Developer";
        public const string OnlyDeveloperChecker = "OnlyDeveloperChecker";
        public const string AdminWithMoreThan1000Days = "AdminWithMoreThan1000Days";
        public const string FirstNameAuth = "FirstNameAuth";
    }
}
