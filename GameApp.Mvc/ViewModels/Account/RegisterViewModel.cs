using System.ComponentModel.DataAnnotations;

namespace GameApp.Mvc.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Данное поле обязательно")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z])\S{6,}$", ErrorMessage = "Не безопасный пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
