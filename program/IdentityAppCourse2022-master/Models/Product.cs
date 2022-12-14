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

        [ForeignKey("providerId")]
        public AppUser provider { get; set; }
        [ForeignKey("categoryId")]
        public Category category { get; set; }
        public string categoryId {get; set;}
        public string providerId {get; set;}
        public IEnumerable<SelectListItem>? CategoryList { get; set; }


    }
}
