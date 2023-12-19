using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityAppCourse2022.Models
{
    public class AppUser : IdentityUser
    {
        public string? RoleId { get; set; }
        [NotMapped]
        public string? Role { get; set; }
        [NotMapped]

        public IEnumerable<SelectListItem>? RoleList { get; set; }

        public ICollection<Product>? products { get; set; }
    }
}
