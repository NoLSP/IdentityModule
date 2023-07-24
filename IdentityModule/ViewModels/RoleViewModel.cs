using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityModule.ViewModels
{
    public class RoleViewModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
    }
}
