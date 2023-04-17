using System.ComponentModel.DataAnnotations;

namespace Text_Editor.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Key]
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
