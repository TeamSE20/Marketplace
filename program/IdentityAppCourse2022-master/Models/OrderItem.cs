// OrderItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityAppCourse2022.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public string ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
