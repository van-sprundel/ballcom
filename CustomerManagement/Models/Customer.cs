using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Models {

    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string Email { get; set; }
    }
}