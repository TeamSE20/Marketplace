using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityAppCourse2022.Models
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string orderStatus { get; set; }
        public string isPaid { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public AppUser client { get; set; }
    }
}
