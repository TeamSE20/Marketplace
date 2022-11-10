using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IdentityAppCourse2022.ViewModels
{
    public class ProductViewModel
    {
        public string? Id { get; set; }
        [Required]
        [Display(Name = "ProductName")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "ProductPrice")]

        [Required]
        public int Price { get; set; }

        public string? Provider { get; set; }

        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        public string? CategorySelected { get; set; }
    }
}
