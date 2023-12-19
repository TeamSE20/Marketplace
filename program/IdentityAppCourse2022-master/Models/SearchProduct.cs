using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityAppCourse2022.Models
{
    public class SearchProduct
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int Quantity { get; set; } = 10;
        public int Price { get; set; }
        [Display(Name = "Image")]
        public string? DisplayImage { get; set; }

        public string? StoreName { get; set; }
    }
}
