using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Models
{
    public class ContactModel
    {
        [Key]
        public int Id { get; set; }       
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
    }
}
