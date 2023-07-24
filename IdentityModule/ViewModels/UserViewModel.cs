using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityModule.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
        [Required]
        public long[] RoleIds { get; set; }
    }
}
