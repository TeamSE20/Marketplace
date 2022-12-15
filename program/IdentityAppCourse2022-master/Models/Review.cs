using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityAppCourse2022.Models
{
    public class Review
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Text { get; set; }
        public string clientId { set; get; }
        public string productId { set; get; }
        public DateTime dateReview { set; get; }
    }
}
