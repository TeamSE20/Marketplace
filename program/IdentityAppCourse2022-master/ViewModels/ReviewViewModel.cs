using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IdentityAppCourse2022.ViewModels
{
    public class ReviewViewModel : EditImageViewModel
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
        [Display(Name = "Image")]
        public string? ProfilePicture { get; set; }

        public string? Provider { get; set; }

        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        public string? CategorySelected { get; set; }
        public List<Models.Review>? reviewList { get; set; }
        public string? client { get; set; }
        public string? Text { get; set; }
    }
}
