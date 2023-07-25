using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityModule.ViewModels
{
    public class UserViewModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
        public long[] RoleIds { get; set; }
    }
}
