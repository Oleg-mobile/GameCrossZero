using GameApp.HttpClient;
using GameApp.Mvc.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameApp.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly GameAppHttpClient _httpClient;

        public AccountController(GameAppHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Login()  // TODO нужен?
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)  //  TODO возможна ли множественная авторизация?
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = await _httpClient.LoginAsync(model.Login, model.Password);

            if (token == null)
            {
                ViewBag.ValidationMessage = "Не правильно введён логин и/или пароль";

                return View(model);
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, model.Login));
            //if (obj.Role != null)
            //{
            //    identity.AddClaim(new Claim(ClaimTypes.Role, obj.Role));
            //}
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            Response.Cookies.Append("token", token.Token, new CookieOptions
            {
                Expires = token.Expires
            });

            return RedirectToAction("Index", "Rooms");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // var user = await _httpClient.GetAllAsync(model.Login);
            //if (user is not null)
            //{
            //    return View(model);
            //}

            await _httpClient.RegisterAsync(model.Login, model.Password);

            var token = await _httpClient.LoginAsync(model.Login, model.Password);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, model.Login));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            Response.Cookies.Append("token", token.Token, new CookieOptions
            {
                Expires = token.Expires
            });

            return RedirectToAction("Index", "Rooms");
        }

        public async Task<IActionResult> Logout()
        {
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("token");
            return RedirectToAction("Login");
		}
    }
}
