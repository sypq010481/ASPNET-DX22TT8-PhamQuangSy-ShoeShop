using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,15}$")]
        public string Password { get; set; }        
        [DefaultValue(1)]
        public int Actived { get; set; }        
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;
        [NotMapped]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,15}$")]
        [Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp!")]
        public string? ConfirmPassword { get; set; }
    }
}
