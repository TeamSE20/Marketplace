using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityAppCourse2022.Models
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        [Display(Name = "Image")]
        public string? ProfilePicture { get; set; }
        
        public AppUser provider { get; set; }
        public Category category { get; set; }

        public IEnumerable<SelectListItem>? CategoryList { get; set; }


    }
}
