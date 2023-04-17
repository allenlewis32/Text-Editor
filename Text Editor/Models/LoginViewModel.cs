using System.ComponentModel.DataAnnotations;

namespace Text_Editor.Models
{
    public class LoginViewModel
    {
        [Key]
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
