using GameApp.HttpClient;
using GameApp.Mvc.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
        public async Task<IActionResult> Login(LoginViewModel model)
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
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = token.Expires
            });

            return RedirectToAction("Index", "Home");
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

            return View();
            //return RedirectToAction("Login", "Account");
        }
    }
}
