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
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Login()
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

            var token = await LoginAsync("https://localhost:7272/api/Account/Login", model.Login, model.Password);

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

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return RedirectToAction("Index", "Home");
        }

        private async Task<string> LoginAsync(string url, string login, string password)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsync(url + $"?username={login}&password={password}", null);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }

            return null;
        }
    }
}
