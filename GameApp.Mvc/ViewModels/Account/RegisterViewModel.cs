using System.ComponentModel.DataAnnotations;

namespace GameApp.Mvc.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Данное поле обязательно")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        public string ConfirmPassword { get; set; }
    }
}
