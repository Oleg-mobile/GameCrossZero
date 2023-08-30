using System.ComponentModel.DataAnnotations;

namespace GameApp.Mvc.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Данное поле обязательно")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        public string Password { get; set; }


    }
}
